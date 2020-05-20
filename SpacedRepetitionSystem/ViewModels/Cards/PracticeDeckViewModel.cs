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
  public sealed class PracticeDeckViewModel : EntityViewModelBase<Deck>
  {
    private int currentIndex = 0;
    private bool isShowingSolution = false;
    private CardField displayedCardField;
    private static readonly Random random = new Random();
    private CardField current;
    string inputText;
    private bool? wasInputCorrect;
    private bool isSummary = false;

    /// <summary>
    /// The Id of the entity
    /// </summary>
    public object Id { get; set; }

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
    public string Title => IsSummary ? Messages.PracticePageSummaryTitle : Messages.PracticePageTitle.FormatWith(Deck.Title);

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
    /// The deck to practice
    /// </summary>
    public Deck Deck { get; set; }

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
    public Command CloseSummaryCommand { get; set; }

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
        ExecuteAction = (param) => ShowSolution()
      };

      DifficultResultCommand = new Command()
      {
        CommandText = Messages.Difficult,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Hard)
      };

      EasyResultCommand = new Command()
      {
        CommandText = Messages.Easy,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Easy)
      };

      DoesNotKnowResultCommand = new Command()
      {
        CommandText = Messages.DoesNotKnow,
        ExecuteAction = async (param) => await ReportPracticeResult(PracticeResultKind.Failed)
      };

      NextCommand = new Command()
      {
        CommandText = Messages.Next,
        ExecuteAction = (param) => Next()
      };

      CloseSummaryCommand = new Command()
      {
        CommandText = Messages.Close,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/")
      };
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    public async Task LoadEntityAsync() => Deck = (await ApiConnector.GetAsync<Deck>(Id)).Result;

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await LoadEntityAsync();
      await base.InitializeAsync();

      bool isActivePractice = Deck.Cards.SelectMany(card => card.Fields).Any(field => field.IsDue);
      if (isActivePractice)
        PracticeFields = Deck.Cards.SelectMany(card => card.Fields).Where(field => field.IsDue).ToList();
      else
      {
        PracticeFields = new List<CardField>();
        PracticeFields.AddRange(Deck.Cards.SelectMany(card => card.Fields));
      }
      PracticeFields.Shuffle();
      if (PracticeFields.Count > 0)
        Current = PracticeFields[0];
    }

    /// <summary>
    /// The input has finished
    /// </summary>
    public async void OnInputFinished()
    {
      if (InputText == Solution)
        await ReportPracticeResult(PracticeResultKind.Easy);
      else
        await ReportPracticeResult(PracticeResultKind.Failed);
    }

    private async Task ReportPracticeResult(PracticeResultKind result)
    {
      AddResult(result);
      if (result == PracticeResultKind.Failed) // if failed insert at random position again 
      {
        int i = random.Next(currentIndex + 1, PracticeFields.Count);
        PracticeFields.Insert(i, Current);
      }

      PracticeHistoryEntry entry = new PracticeHistoryEntry()
      {
        PracticeDate = DateTime.Today,
        CardId = Current.CardId,
        DeckId = Deck.DeckId,
        FieldName = Current.FieldName,
        CorrectCount = result == PracticeResultKind.Easy ? 1 : 0,
        HardCount = result == PracticeResultKind.Hard ? 1 : 0,
        WrongCount = result == PracticeResultKind.Failed ? 1 : 0
      };
     
      ApiReply reply = await ApiConnector.PostAsync(entry);
      if (!reply.WasSuccessful)
        throw new NotifyException(reply.ResultMessage);

      if (Current.CardFieldDefinition.ShowInputForPractice)
        ShowSolution();
      else
        Next();
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
      int current = 0;
      int index = random.Next(Current.Card.Fields.Count - 1);
      for (int i = 0; i < Current.Card.Fields.Count; i++)
      {
        if (Current.FieldName != Current.Card.Fields[i].FieldName)
        {
          if (index == current)
            DisplayedCardField = Current.Card.Fields[i];
          current++;
        }
      }
    }

    private void AddResult(PracticeResultKind resultKind)
    {
      CardPracticeResult cardResult;
      if (PracticeResults.ContainsKey(Current.CardId))
      {
        cardResult = PracticeResults[current.CardId];
        if (!cardResult.FieldResults.ContainsKey(current.FieldName))
          cardResult.FieldResults.Add(current.FieldName, new PracticeResult());
        switch (resultKind)
        {
          case PracticeResultKind.Easy:
            cardResult.Correct++;
            cardResult.FieldResults[current.FieldName].Correct++;
            break;
          case PracticeResultKind.Hard:
            cardResult.Difficult++;
            cardResult.FieldResults[current.FieldName].Difficult++;
            break;
          case PracticeResultKind.Failed:
            cardResult.Wrong++;
            cardResult.FieldResults[current.FieldName].Wrong++;
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
          Wrong = resultKind == PracticeResultKind.Failed ? 1 : 0
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
        Wrong = resultKind == PracticeResultKind.Failed ? 1 : 0
      };
      cardPracticeResult.FieldResults.Add(current.FieldName, result);
    }
  }
}