using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Pages
{
  public abstract class EditPageBase<TEntity, TViewModel> : ComponentBase 
    where TViewModel : EditViewModelBase<TEntity> 
    where TEntity : IEntity
  {
    [Inject]
    public TViewModel ViewModel { get; set; }

    [Parameter]
    public object Id { set => ViewModel.LoadOrCreateEntity(value); }
  }
}