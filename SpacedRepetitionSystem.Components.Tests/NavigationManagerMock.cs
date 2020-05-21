using Microsoft.AspNetCore.Components;

namespace SpacedRepetitionSystem.Components.Tests
{
  /// <summary>
  /// Mock implementation for <see cref="NavigationManager"/>
  /// </summary>
  public sealed class NavigationManagerMock : NavigationManager
  {
    /// <summary>
    /// The uri to which the app was navigated to 
    /// </summary>
    public string NavigatedUri { get; set; }

    /// <summary>
    /// If the reload was forced
    /// </summary>
    public bool WasForcedReload { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public NavigationManagerMock()
      : base() => this.Initialize("http://localhost:2112/", "http://localhost:2112/test");

    ///<inheritdoc/>
    protected override void NavigateToCore(string uri, bool forceLoad)
    {
      NavigatedUri = uri;
      WasForcedReload = forceLoad;
    }

    /// <summary>
    /// Sets the uri
    /// </summary>
    /// <param name="uri">The uri</param>
    public void SetUri(string uri) => Uri = uri;
  }
}