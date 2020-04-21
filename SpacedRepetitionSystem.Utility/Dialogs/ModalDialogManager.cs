using System;

namespace SpacedRepetitionSystem.Utility.Dialogs
{
	/// <summary>
	/// Manager class for showing modal dialogs
	/// </summary>
	public static class ModalDialogManager
	{
		private static IDialogProvider dialogProvider;

		/// <summary>
		/// Initializes the manager class with a dialog provider
		/// </summary>
		/// <param name="provider">The dialog provider</param>
		public static void Initialize(IDialogProvider provider) { dialogProvider = provider; }

		/// <summary>
		/// Displays a modal dialog
		/// </summary>
		/// <param name="title">Title of the dialog</param>
		/// <param name="text">Text of the dialog</param>
		/// <param name="buttons">Buttons of the dialog</param>
		/// <param name="callback">Callback for when diialog is closed</param>
		public static void ShowDialog(string title, string text, DialogButtons buttons, Action<DialogResult> callback)
		{
			_ = dialogProvider ?? throw new ArgumentNullException(nameof(dialogProvider));
			dialogProvider.ShowModal(title, text, buttons, callback);
		}
	}
}