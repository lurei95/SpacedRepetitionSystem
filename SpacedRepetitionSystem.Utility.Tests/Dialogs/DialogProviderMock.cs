using SpacedRepetitionSystem.Utility.Dialogs;
using System;

namespace SpacedRepetitionSystem.Utility.Tests.Dialogs
{
  /// <summary>
  /// Mock implementation of <see cref="IDialogProvider"/>
  /// </summary>
  public sealed class DialogProviderMock : IDialogProvider
  {
    /// <summary>
    /// Dialog title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Dialog text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The buttons
    /// </summary>
    public DialogButtons Buttons { get; set; }

    /// <summary>
    /// The callback
    /// </summary>
    public Action<DialogResult> Callback { get; set; }

    ///<inheritdoc/>
    public void ShowModal(string title, string text, DialogButtons buttons, Action<DialogResult> callback)
    {
      Title = title;
      Text = text;
      Buttons = buttons;
      Callback = callback;
    }
  }
}