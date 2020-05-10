using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Data
{
  /// <summary>
  /// Baseclass for a chart component
  /// </summary>
  /// <typeparam name="TItem"></typeparam>
  public abstract class ChartComponentBase<TItem> : ComponentBase
  {
    bool isDrawing;

    /// <summary>
    /// Whether the chart is visible
    /// </summary>
    [Parameter]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Background colors for the data
    /// </summary>
    [Parameter]
    public List<string> BackgroundColors { get; set; }

    /// <summary>
    /// Border colors for the data
    /// </summary>
    [Parameter]
    public List<string> BorderColors { get; set; }

    /// <summary>
    /// Data labels
    /// </summary>
    [Parameter]
    public string[] Labels { get; set; }

    /// <summary>
    /// Label for the chart
    /// </summary>
    [Parameter]
    public string Label { get; set; }

    /// <summary>
    /// Redraws the chart
    /// </summary>
    /// <returns></returns>
    public abstract Task Redraw();

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
      if (firstRender)
        await Redraw();
    }
  }
}