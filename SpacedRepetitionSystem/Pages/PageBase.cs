using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Pages
{
  /// <summary>
  /// Baseclass for all pages
  /// </summary>
  /// <typeparam name="TViewModel">ViewModel-Type</typeparam>
  public abstract class PageBase<TViewModel> : ComponentBase where TViewModel : PageViewModelBase
  {
    /// <summary>
    /// The ViewModel
    /// </summary>
    [Inject]
    public TViewModel ViewModel { get; set; }

    /// <summary>
    /// Whether the page is currenty still loading data
    /// </summary>
    public bool IsLoading { get; private set; } = true;

    ///<inheritdoc/>
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
