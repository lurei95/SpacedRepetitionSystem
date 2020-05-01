using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Extensions;
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

    /// <summary>
    /// Whether the results should be shown 
    /// </summary>
    public bool ShowResults { get; set; } = false;

    /// <summary>
    /// The current Field
    /// </summary>
    public PracticeField Current { get; set; }

    /// <summary>
    /// The fields to practice
    /// </summary>
    public List<PracticeField> PracticeFields { get; private set; }

    /// <summary>
    /// The deck to practice
    /// </summary>
    public Deck Deck { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public PracticeDeckViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(context, navigationManager, apiConnector)
    { }

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
        Current = PracticeFields[0];
    }
  }
}
