using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// ViewModel for practicing a deck
  /// </summary>
  public sealed class PracticeDeckViewModel : EntityViewModelBase<Deck>
  {
    private bool isActivePractice;

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
    }
  }
}
