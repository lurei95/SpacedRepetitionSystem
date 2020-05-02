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

    private void ReportPracticeResult(PracticeResultKind result)
    {
      //Report
      Next();
    }

    private void ShowSolution()
    {
      IsShowingSolution = true;

    }

    private void Next()
    {
      if (currentIndex < PracticeFields.Count - 1)
      {
        currentIndex++;
        Current = PracticeFields[currentIndex];
        IsShowingSolution = false;
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
  }
}
