using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Edits
{
  /// <summary>
  /// Baseclass for all edits
  /// </summary>
  public abstract class EditBase : CustomComponentBase
  {
    /// <summary>
    /// Js runtime
    /// </summary>
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// The width of the caption
    /// </summary>
    [Parameter]
    public int CaptionWidth { get; set; } = 100;

    /// <summary>
    /// Determines whether the edit is enabled
    /// </summary>
    [Parameter]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Property proxy for binding
    /// </summary>
    [Parameter]
    public PropertyProxy Property { get; set; }

    /// <summary>
    /// Caption of the edit
    /// </summary>
    [Parameter]
    public string Caption { get; set; }

    /// <summary>
    /// If the edit should be focued
    /// </summary>
    [Parameter]
    public bool IsFocued { get; set; }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
      if (IsFocued)
        await JSRuntime.InvokeVoidAsync("utilities.focusElement", Id + "-input");
    }
  }
}
