using Microsoft.AspNetCore.Components;
using System;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Command for performing a navigation
  /// </summary>
  public sealed class NavigationCommand : CommandBase
  {
    private readonly NavigationManager navigationManager;

    /// <summary>
    /// The target uri
    /// </summary>
    public string TargetUri { get; set; }

    /// <summary>
    /// Whether the target uri is relative from the current uri
    /// </summary>
    public bool IsRelative { get; set; }

    /// <summary>
    /// Factory to construct the target uri
    /// </summary>
    public Func<object, string> TargetUriFactory { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager</param>
    public NavigationCommand(NavigationManager navigationManager)
    { this.navigationManager = navigationManager; }

    ///<inheritdoc/>
    public override void ExecuteCommand(object param = null)
    {
      string uri;
      if (TargetUriFactory != null)
        uri = TargetUriFactory.Invoke(param);
      else
        uri = TargetUri;
      if (IsRelative)
        navigationManager.NavigateTo(navigationManager.Uri + uri);
      else
        navigationManager.NavigateTo(uri);
    }
  }
}