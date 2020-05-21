using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for a view model
  /// </summary>
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Invokes the property changed event
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
    { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
  }
}