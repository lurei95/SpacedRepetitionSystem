using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.ViewModels;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Pages
{
  /// <summary>
  /// Baseclass for all pages
  /// </summary>
  /// <typeparam name="TViewModel">ViewModel-Type</typeparam>
  public abstract class PageBase<TViewModel> : ComponentBase where TViewModel : PageViewModelBase
  {
    /// <summary>
    /// NavigationManager
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; }

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
      await base.OnAfterRenderAsync(firstRender);
      if (firstRender)
      {
        bool result = await ViewModel.InitializeAsync();
        if (!result)
          NavigationManager.NavigateTo("/");
        else
        {
          IsLoading = false;
          StateHasChanged();
        }
      }
    }
  }
}