using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpacedRepetitionSystem.Components.Edits
{
  /// <summary>
  /// Proxy for an entity property with validation
  /// </summary>
  public sealed class PropertyProxy : INotifyPropertyChanged
  {
    private readonly Func<string> getter;
    private readonly Action<string> setter;
    private string errorText;
    private Func<string, object, string> validator;

    /// <summary>
    /// Entity which is bound
    /// </summary>
    public object Entity { get; set; }

    /// <summary>
    /// Name of the property
    /// </summary>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Validator
    /// </summary>
    public Func<string, object, string> Validator 
    {
      get => validator;
      set
      {
        validator = value;
        if (value != null)
          ErrorText = Validator.Invoke(Value, Entity);
      }
    }

    /// <summary>
    /// Error Text
    /// </summary>
    public string ErrorText
    {
      get => errorText;
      set
      {
        if (ErrorText != value)
        {
          errorText = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Value
    /// </summary>
    public string Value
    {
      get => getter.Invoke();
      set
      {
        if (Value != value)
        {
          ErrorText = null;
          if (Validator != null)
            ErrorText = Validator.Invoke(value, Entity);
          setter.Invoke(value);
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Constructors
    /// </summary>
    /// <param name="getter">getter</param>
    /// <param name="setter">setter</param>
    /// <param name="propertyName">Name of the property</param>
    public PropertyProxy(Func<string> getter, Action<string> setter, string propertyName, object entity)
    {
      PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
      this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
      this.setter = setter ?? throw new ArgumentNullException(nameof(setter));
      this.Entity = entity;
    }

    /// <summary>
    /// Manually sets the error text
    /// </summary>
    /// <param name="error"></param>
    public void SetError(string error) => ErrorText = error;

    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName]string propertyName = "")
    { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
  }
}