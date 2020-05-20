using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Components.Pages
{
  /// <summary>
  /// Baseclass for all SearchPages
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  /// <typeparam name="TViewModel">ViewModel-Type</typeparam>
  public abstract class SearchPageBase<TEntity, TViewModel> : PageBase<TViewModel>
    where TViewModel : SearchViewModelBase<TEntity>
    where TEntity : class, IRootEntity, new()
  { }
}