using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Components.Pages
{
  /// <summary>
  /// Baseclass for all EditPages
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  /// <typeparam name="TViewModel">ViewModel-Type</typeparam>
  public abstract class EditPageBase<TEntity, TViewModel> : PageBase<TViewModel> 
    where TViewModel : EditViewModelBase<TEntity> 
    where TEntity : class, IRootEntity, new()
  {
    /// <summary>
    /// Id of the entity to edit
    /// </summary>
    [Parameter]
    public object Id { set => ViewModel.Id = value; }
  }
}