using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// ViewModel for practicing a deck
  /// </summary>
  public sealed class PracticeDeckViewModel : SingleEntityViewModelBase<Deck>
  {
    private int currentIndex = 0;
    private bool isShowingSolution = false;
    private CardField displayedCardField;
    private static readonly Random random = new Random();
    private CardField current;
    string inputText;
    private bool? wasInputCorrect;
    private bool isSummary = false;
    private bool isLoading = false;
    private bool isActivePractice;

    /// <summary>
    /// Whether data is being loaded
    /// </summary>
    public bool IsLoading
    {
      get => isLoading;
      set
      {
        if (isLoading != value)
        {
          isLoading = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Whether the results should be shown 
    /// </summary>
    public bool IsShowingSolution
    {
      get => isShowingSolution;
      set
      {
        if (value != IsShowingSolution)
        {
          isShowingSolution = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The practice results
    /// </summary>
    public Dictionary<long, CardPracticeResult> PracticeResults = new Dictionary<long, CardPracticeResult>();

    /// <summary>
    /// Whether the input was correct
    /// </summary>
    public bool? WasInputCorrect
    {
      get => wasInputCorrect;
      set
      {
        if (value != wasInputCorrect)
        {
          wasInputCorrect = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Whether the practice summary is shown 
    /// </summary>
    public bool IsSummary
    {
      get => isSummary;
      set
      {
        isSummary = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Title of the page
    /// </summary>
    public override string Title 
      => Entity != null ? IsSummary ? Messages.PracticePageSummaryTitle : Messages.PracticePageTitle.FormatWith(Entity.Title) : null;

    /// <summary>
    /// Class for validation
    /// </summary>
    public string ValidationClass 
      => WasInputCorrect == true ? "right" : WasInputCorrect == false ? "wrong" : null;

    /// <summary>
    /// Text of the Input
    /// </summary>
    public string InputText
    {
      get => inputText;
      set
      {
        if (value != inputText)
        {
          inputText = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Fieldname of the currently practiced field
    /// </summary>
    public string CurrentFieldName => Current?.FieldName;

    /// <summary>
    /// The solution
    /// </summary>
    public string Solution => Current?.Value;

    /// <summary>
    /// Name of the displayed field
    /// </summary>
    public string DisplayedFieldName => DisplayedCardField?.FieldName;

    /// <summary>
    /// Value of the displayed field
    /// </summary>
    public string DisplayedFieldValue => DisplayedCardField?.Value;

    /// <summary>
    /// The displayed card field
    /// </summary>
    public CardField DisplayedCardField
    {
      get => displayedCardField;
      set
      {
        if (value != displayedCardField)
        {
          displayedCardField = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The current Field
    /// </summary>
    public CardField Current
    {
      get => current;
      set
      {
        if (current != value)
        {
          current = value;
          SelectRandomDisplayField();
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The fields to practice
    /// </summary>
    public List<CardField> PracticeFields { get; private set; }

    /// <summary>
    /// Command for the result difficult
    /// </summary>
    public Command DifficultResultCommand { get; private set; }

    /// <summary>
    /// Command for the result easy
    /// </summary>
    public Command EasyResultCommand { get; private set; }

    /// <summary>
    /// Command for the result don't know
    /// </summary>
    public Command DoesNotKnowResultCommand { get; private set; }

    /// <summary>
    /// Command to show the solution
    /// </summary>
    public Command ShowSolutionCommand { get; private set; }

    /// <summary>
    /// Command for going to the next entry
    /// </summary>
    public Command NextCommand { get; set; }

    /// <summary>
    /// Command for closing the summary
    /// </summary>
    public NavigationCommand CloseSummaryCommand { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public PracticeDeckViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
      ShowSolutionCommand = new Command()
      {
        CommandText = Messages.Show,
        ToolTip = Messages.PracticeShowResultCommandToolTip,
        ExecuteAction = (param) => ShowSolution()
      };
      DifficultResultCommand = new Command()
      {
        CommandText = Messages.Difficult,
        ToolTip = Messages.PracticeDifficultCommandToolTip,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Hard)
      };
      EasyResultCommand = new Command()
      {
        CommandText = Messages.Easy,
        ToolTip = Messages.PracticeEasyCommandToolTip,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Easy)
      };
      DoesNotKnowResultCommand = new Command()
      {
        CommandText = Messages.DoesNotKnow,
        ToolTip = Messages.PracticeDoesNotKnowToolTip,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Wrong)
      };
      NextCommand = new Command()
      {
        CommandText = Messages.Next,
        ToolTip = Messages.PracticeNextCommandToolTip,
        ExecuteAction = (param) => Next()
      };
      CloseSummaryCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.Close,
        ToolTip = Components.Messages.CloseCommandToolTip,
        TargetUri = "/"
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync() && await LoadEntityAsync();
      if (!result)
        return false;

      isActivePractice = Entity.Cards.SelectMany(card => card.Fields).Any(field => field.IsDue && !string.IsNullOrEmpty(field.Value));
      if (isActivePractice)
        PracticeFields = Entity.Cards.SelectMany(card => card.Fields).Where(field => field.IsDue && !string.IsNullOrEmpty(field.Value)).ToList();
      else
      {
        PracticeFields = new List<CardField>();
        PracticeFields.AddRange(Entity.Cards.SelectMany(card => card.Fields).Where(field => !string.IsNullOrEmpty(field.Value)));
      }
      PracticeFields.Shuffle();

      //Restore circular references lost due to json serialization 
      foreach (Card card in Entity.Cards)
      {
        card.Deck = Entity;
        foreach (CardField field in card.Fields)
          field.Card = card;
      }

      if (PracticeFields.Count > 0)
        Current = PracticeFields[0];
      return true;
    }

    /// <summary>
    /// The input has finished
    /// </summary>
    public async void OnInputFinished()
    {
      if (InputText == Solution)
        await ReportPracticeResult(PracticeResultKind.Easy);
      else
        await ReportPracticeResult(PracticeResultKind.Wrong);
    }

    private async Task ReportPracticeResult(PracticeResultKind result)
    {
      IsLoading = true;
      AddResult(result);
      if (result == PracticeResultKind.Wrong) // if failed insert at random position again 
      {
        int i = random.Next(currentIndex + 1, PracticeFields.Count);
        PracticeFields.Insert(i, Current);
      }

      if (isActivePractice)
      {
        PracticeHistoryEntry entry = new PracticeHistoryEntry()
        {
          PracticeDate = DateTime.Today,
          CardId = Current.CardId,
          DeckId = Entity.DeckId,
          FieldId = Current.FieldId,
          CorrectCount = result == PracticeResultKind.Easy ? 1 : 0,
          HardCount = result == PracticeResultKind.Hard ? 1 : 0,
          WrongCount = result == PracticeResultKind.Wrong ? 1 : 0
        };

        ApiReply reply = await ApiConnector.PostAsync(entry);
        if (!reply.WasSuccessful)
          throw new NotifyException(reply.ResultMessage);
      }

      if (Current.CardFieldDefinition.ShowInputForPractice)
        ShowSolution();
      else
        Next();
      IsLoading = false;
    }

    private void ShowSolution()
    {
      if (Current.CardFieldDefinition.ShowInputForPractice)
        WasInputCorrect = InputText == Solution;
      IsShowingSolution = true;
    }

    private void Next()
    {
      if (currentIndex < PracticeFields.Count - 1)
      {
        currentIndex++;
        Current = PracticeFields[currentIndex];
        IsShowingSolution = false;
        InputText = string.Empty;
        WasInputCorrect = null;
      }
      else
        IsSummary = true;
    }

    private void SelectRandomDisplayField()
    {
      List<CardField> fields = Current.Card.Fields
        .Where(field => field.FieldName != Current.FieldName && !string.IsNullOrEmpty(field.Value))
        .ToList();
      fields.Shuffle();
      int index = random.Next(fields.Count - 1);
      DisplayedCardField = fields[index];
    }

    private void AddResult(PracticeResultKind resultKind)
    {
      CardPracticeResult cardResult;
      if (PracticeResults.ContainsKey(Current.CardId))
      {
        cardResult = PracticeResults[current.CardId];
        if (!cardResult.FieldResults.ContainsKey(current.FieldId))
          cardResult.FieldResults.Add(current.FieldId, new PracticeResult());
        switch (resultKind)
        {
          case PracticeResultKind.Easy:
            cardResult.Correct++;
            cardResult.FieldResults[current.FieldId].Correct++;
            break;
          case PracticeResultKind.Hard:
            cardResult.Difficult++;
            cardResult.FieldResults[current.FieldId].Difficult++;
            break;
          case PracticeResultKind.Wrong:
            cardResult.Wrong++;
            cardResult.FieldResults[current.FieldId].Wrong++;
            break;
          default:
            break;
        }
      }
      else
      {
        cardResult = new CardPracticeResult()
        {
          Correct = resultKind == PracticeResultKind.Easy ? 1 : 0,
          Difficult = resultKind == PracticeResultKind.Hard ? 1 : 0,
          Wrong = resultKind == PracticeResultKind.Wrong ? 1 : 0
        };
        AddNewFieldResult(cardResult, resultKind);
        PracticeResults.Add(current.CardId, cardResult);
      }
    }

    private void AddNewFieldResult(CardPracticeResult cardPracticeResult, PracticeResultKind resultKind)
    {
      PracticeResult result = new PracticeResult()
      {
        Correct = resultKind == PracticeResultKind.Easy ? 1 : 0,
        Difficult = resultKind == PracticeResultKind.Hard ? 1 : 0,
        Wrong = resultKind == PracticeResultKind.Wrong ? 1 : 0
      };
      cardPracticeResult.FieldResults.Add(current.FieldId, result);
    }
  }
}