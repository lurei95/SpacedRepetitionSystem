using System;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Command for a button
  /// </summary>
  public class Command : CommandBase
  {
    /// <summary>
    /// Action performed when the command is executed
    /// </summary>
    public virtual Action<object> ExecuteAction { private get; set; }

    ///<inheritdoc/>
    public override void ExecuteCommand(object param = null) => ExecuteAction.Invoke(param);
  }
}