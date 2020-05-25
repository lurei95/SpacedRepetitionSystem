using Microsoft.AspNetCore.Components;

namespace SpacedRepetitionSystem.Components
{
  /// <summary>
  /// Baseclass for all custom razor components
  /// </summary>
  public abstract class CustomComponentBase : ComponentBase
  {
    /// <summary>
    /// Style of the component
    /// </summary>
    [Parameter]
    public virtual string Style { get; set; }

    /// <summary>
    /// The classes of the component
    /// </summary>
    [Parameter]
    public virtual string Class { get; set; }

    /// <summary>
    /// Id of the component
    /// </summary>
    [Parameter]
    public virtual string Id { get; set; }

    /// <summary>
    /// Method for notifying that the state has changed
    /// </summary>
    protected virtual void NotifyStateChanged() => InvokeAsync(() => StateHasChanged());
  }
}