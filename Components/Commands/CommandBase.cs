using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Baseclass for a command
  /// </summary>
  public abstract class CommandBase : INotifyPropertyChanged
  {
    private bool isEnabled = true;
    private bool isVisible = true;
    private string commandText;
    private string toolTip;
    private string icon;

    /// <summary>
    /// PropertyChanged Event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Whether the button is enabled
    /// </summary>
    public bool IsEnabled
    {
      get => isEnabled;
      set
      {
        if (IsEnabled != value)
        {
          isEnabled = value;
          OnPropertyChanged(nameof(IsEnabled));
        }
      }
    }


    /// <summary>
    /// Whether the button is enabled
    /// </summary>
    public Func<object, bool> IsEnabledFunction { get; set; }

    /// <summary>
    /// The icon of the button
    /// </summary>
    public string Icon
    {
      get => icon;
      set
      {
        if (Icon != value)
        {
          icon = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Ehether the button is visible
    /// </summary>
    public bool IsVisible
    {
      get => isVisible;
      set
      {
        if (IsVisible != value)
        {
          isVisible = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Text of the button
    /// </summary>
    public string CommandText
    {
      get => commandText;
      set
      {
        if (CommandText != value)
        {
          commandText = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Tooltip of the button
    /// </summary>
    public string ToolTip
    {
      get => toolTip;
      set
      {
        if (ToolTip != value)
        {
          toolTip = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="param">Parameter for executing the command</param>
    public abstract void ExecuteCommand(object param = null);

    /// <summary>
    /// Invokes the property change event
    /// </summary>
    /// <param name="propertyName">The property name</param>
    protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
    { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
  }
}