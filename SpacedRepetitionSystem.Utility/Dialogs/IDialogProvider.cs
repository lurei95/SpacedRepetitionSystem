using System;

namespace SpacedRepetitionSystem.Utility.Dialogs
{
	/// <summary>
	/// Interface for a class that providers the possibleity to display a modal dialog
	/// </summary>
  public interface IDialogProvider
  {
		/// <summary>
		/// Shows the dialog as a modal
		/// </summary>
		/// <param name="title">Title of the dialog</param>
		/// <param name="text">Text of the dialog</param>
		/// <param name="buttons"> buttons of the dialog</param>
		/// <param name="callback">Callback when the dialog is closed</param>
		void ShowModal(string title, string text, DialogButtons buttons, Action<DialogResult> callback);
	}
}