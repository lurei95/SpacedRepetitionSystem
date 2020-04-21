using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Pages
{
  public abstract class SearchPageBase<TEntity, TViewModel> : ComponentBase
    where TViewModel : SearchViewModelBase<TEntity>
    where TEntity : IEntity
  {
    [Inject]
    public TViewModel ViewModel { get; set; }
  }
}