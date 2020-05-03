using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
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
    private PracticeField current;
    string inputText;
    private bool? wasInputCorrect;
    private bool isSummary = true;

    private readonly Dictionary<long, CardPracticeResult> practiceResults = new Dictionary<long, CardPracticeResult>();

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
    public string CurrentFieldName => Current?.Field?.FieldName;

    /// <summary>
    /// The solution
    /// </summary>
    public string Solution => Current?.Field?.Value;

    /// <summary>
    /// Name of the displayed field
    /// </summary>
    public string DisplayedFieldName => DisplayedCardField?.FieldName;

    /// <summary>
    /// Value of the displayed field
    /// </summary>
    public string DisplayedFieldValue => DisplayedCardField?.Value;

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
    public PracticeField Current
    {
      get => current;
      set
      {
        if (current != value)
        {
          current = value;
          SelectRandomDisplayField();
        }
      }
    }

    /// <summary>
    /// The fields to practice
    /// </summary>
    public List<PracticeField> PracticeFields { get; private set; }

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
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public PracticeDeckViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(context, navigationManager, apiConnector)
    {
      ShowSolutionCommand = new Command()
      {
        CommandText = Messages.Show,
        ExecuteAction = (param) => ShowSolution()
      };

      DifficultResultCommand = new Command()
      {
        CommandText = Messages.Difficult,
        ExecuteAction = (param) => ReportPracticeResult(PracticeResultKind.Hard)
      };

      EasyResultCommand = new Command()
      {
        CommandText = Messages.Easy,
        ExecuteAction = (param) => ReportPracticeResult(PracticeResultKind.Easy)
      };

      DoesNotKnowResultCommand = new Command()
      {
        CommandText = Messages.DoesNotKnow,
        ExecuteAction = (param) => ReportPracticeResult(PracticeResultKind.Failed)
      };

      NextCommand = new Command()
      {
        CommandText = Messages.Next,
        ExecuteAction = (param) => Next()
      };
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    public void LoadEntity(object id) => Deck = ApiConnector.Get<Deck>(id);

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await Context.Entry(Deck)
        .Collection(deck => deck.PracticeFields)
        .LoadAsync();

      bool isActivePractice = Deck.PracticeFields.Any(field => field.IsDue);
      if (isActivePractice)
        PracticeFields = Deck.PracticeFields.Where(field => field.IsDue).ToList();
      else
      {
        PracticeFields = new List<PracticeField>();
        PracticeFields.AddRange(Deck.PracticeFields);
      }
      PracticeFields.Shuffle();
      if (PracticeFields.Count > 0)
      {
        foreach (PracticeField field in PracticeFields)
        {
          await Context.Entry(field).Reference(field => field.Card).LoadAsync();
          await Context.Entry(field.Card).Collection(card => card.Fields).LoadAsync();
          await Context.Entry(field).Reference(field => field.Field).LoadAsync();
          await Context.Entry(field.Field).Reference(field => field.CardFieldDefinition).LoadAsync();
        }
        Current = PracticeFields[0];
      }
    }

    /// <summary>
    /// The input has finished
    /// </summary>
    public void OnInputFinished()
    {
      if (InputText == Solution)
        ReportPracticeResult(PracticeResultKind.Easy);
      else
        ReportPracticeResult(PracticeResultKind.Failed);
    }

    private void ReportPracticeResult(PracticeResultKind result)
    {
      AddResult(result);
      if (result == PracticeResultKind.Failed) // if failed insert at random position again 
      {
        int i = random.Next(currentIndex + 1, PracticeFields.Count);
        PracticeFields.Insert(i, Current);
      }

      if (Current.Field.CardFieldDefinition.ShowInputForPractice)
        ShowSolution();
      else
        Next();
    }

    private void ShowSolution()
    {
      if (Current.Field.CardFieldDefinition.ShowInputForPractice)
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
        NavigationManager.NavigateTo("/");
    }

    private void SelectRandomDisplayField()
    {
      int current = 0;
      int index = random.Next(Current.Card.Fields.Count - 1);
      for (int i = 0; i < Current.Card.Fields.Count; i++)
      {
        if (Current.Field.FieldName != Current.Card.Fields[i].FieldName)
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
      if (practiceResults.ContainsKey(Current.CardId))
      {
        cardResult = practiceResults[current.CardId];
        if (cardResult.FieldResults.ContainsKey(current.FieldName))
        {
          switch (resultKind)
          {
            case PracticeResultKind.Easy:
              cardResult.FieldResults[current.FieldName].Correct++;
              break;
            case PracticeResultKind.Hard:
              cardResult.FieldResults[current.FieldName].Difficult++;
              break;
            case PracticeResultKind.Failed:
              cardResult.FieldResults[current.FieldName].Wrong++;
              break;
            default:
              break;
          }
        }
        else
          AddNewFieldResult(cardResult, resultKind);
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
      }
      practiceResults.Add(current.CardId, cardResult);
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
