using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Utility.Dialogs;
using System;

namespace SpacedRepetitionSystem.Components.Modals
{
	/// <summary>
	/// Base Class for a modal dialog
	/// </summary>
	public class ModalDialogBase : ComponentBase, IDialogProvider
	{
		/// <summary>
		/// Whether the dialog is visible 
		/// </summary>
		protected bool IsVisible { get; set; }

		/// <summary>
		/// The dialog text
		/// </summary>
		protected string Text { get; set; }

		/// <summary>
		/// Title of the dialog
		/// </summary>
		protected string Title { get; set; }

		/// <summary>
		/// Is the third button visible
		/// </summary>
		protected bool ThirdButtonVisible => Buttons == DialogButtons.YesNoCancel;

		/// <summary>
		/// Is the second button visible
		/// </summary>
		protected bool SecondButtonVisible => Buttons != DialogButtons.Okay;

		/// <summary>
		/// Text of the first button
		/// </summary>
		protected string FirstButtonText => (Buttons) switch
		{
			var x when x == DialogButtons.YesNo || x == DialogButtons.YesNoCancel => Messages.Yes,
			var x when x == DialogButtons.OkayCancel || x == DialogButtons.Okay => Messages.Okay,
			_ => null
		};

		/// <summary>
		/// Text of the second button
		/// </summary>
		protected string SecondButtonText => (Buttons) switch
		{
			var x when x == DialogButtons.YesNo || x == DialogButtons.YesNoCancel => Messages.No,
			DialogButtons.OkayCancel => Messages.Cancel,
			_ => null
		};

		/// <summary>
		/// Text of the third button
		/// </summary>
		protected string ThridButtonText => Buttons == DialogButtons.YesNoCancel ? Messages.Cancel : null;

		/// <summary>
		/// The buttons of the dialog
		/// </summary>
		protected DialogButtons Buttons { get; private set; }

		/// <summary>
		/// Callback when the dialog is closed
		/// </summary>
		protected Action<DialogResult> Callback { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public ModalDialogBase() { ModalDialogManager.Initialize(this); }

		///<inheritdoc>
		public void ShowModal(string title, string text, DialogButtons buttons, Action<DialogResult> callback)
		{
			Callback = callback;
			IsVisible = true;
			Title = title;
			Text = text;
			Buttons = buttons;
			StateHasChanged();
		}

		/// <summary>
		/// Closes the dialog an executed the callback action
		/// </summary>
		/// <param name="result">The result with which the dialog was closed</param>
		protected void CloseModal(DialogButton result)
		{
			IsVisible = false;
			Title = Text = null;
			StateHasChanged();
			Callback.Invoke(CalculateDialogResult(result));
			Callback = null;
		}

		private DialogResult CalculateDialogResult(DialogButton result) =>
			(result, Buttons) switch
			{
				(DialogButton.None, DialogButtons.OkayCancel) => DialogResult.Cancel,
				(DialogButton.None, DialogButtons.YesNoCancel) => DialogResult.Cancel,
				(DialogButton.None, DialogButtons.YesNo) => DialogResult.No,
				(DialogButton.Button1, DialogButtons.Okay) => DialogResult.Okay,
				(DialogButton.Button1, DialogButtons.OkayCancel) => DialogResult.Okay,
				(DialogButton.Button1, DialogButtons.YesNo) => DialogResult.Yes,
				(DialogButton.Button1, DialogButtons.YesNoCancel) => DialogResult.Yes,
				(DialogButton.Button2, DialogButtons.OkayCancel) => DialogResult.Cancel,
				(DialogButton.Button2, DialogButtons.YesNo) => DialogResult.No,
				(DialogButton.Button2, DialogButtons.YesNoCancel) => DialogResult.Cancel,
				(_, _) => DialogResult.Cancel,
			};
	}
}