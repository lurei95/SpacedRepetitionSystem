using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Pages
{
  public abstract class EditPageBase<TEntity, TViewModel> : ComponentBase 
    where TViewModel : EditViewModelBase<TEntity> 
    where TEntity : IEntity
  {
    public bool IsLoading { get; private set; } = true;

    [Inject]
    public TViewModel ViewModel { get; set; }

    [Parameter]
    public object Id { set => ViewModel.Id = value; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        IsLoading = true;
        StateHasChanged();
        await base.OnAfterRenderAsync(firstRender);
        await ViewModel.InitializeAsync();
        IsLoading = false;
        StateHasChanged();
      }
    }
  }
}