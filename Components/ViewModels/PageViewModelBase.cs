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
    /// Command for closing the page
    /// </summary>
    public NavigationCommand CloseCommand { get; set; }

    /// <summary>
    /// Performs initialization tasks async when the page is shown
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> InitializeAsync() => await Task.FromResult(true);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    public PageViewModelBase(NavigationManager navigationManager)
    {
      NavigationManager = navigationManager;
      CloseCommand = new NavigationCommand(navigationManager)
      {
        Icon = "oi oi-x",
        IsEnabled = true,
        TargetUri = "/"
      };
    }
  }
}