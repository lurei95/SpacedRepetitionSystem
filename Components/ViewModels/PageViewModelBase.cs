using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class PageViewModelBase : ViewModelBase
  {
    /// <summary>
    /// Navigation Manager
    /// </summary>
    protected NavigationManager NavigationManager { get; private set; }

    /// <summary>
    /// Command for Saving the changes
    /// </summary>
    public Command CloseCommand { get; set; }

    /// <summary>
    /// Performs initialization tasks async when the page is shown
    /// </summary>
    /// <returns></returns>
    public virtual async Task InitializeAsync() { }

    public PageViewModelBase(NavigationManager navigationManager)
    {
      NavigationManager = navigationManager;
      CloseCommand = new Command()
      {
        Icon = "oi oi-x",
        IsEnabled = true,
        ExecuteAction = (param) => Close()
      };
    }

    /// <summary>
    /// navigates away from the current page to the home page
    /// </summary>
    protected virtual void Close() { NavigationManager.NavigateTo("/Home"); }
  }
}
