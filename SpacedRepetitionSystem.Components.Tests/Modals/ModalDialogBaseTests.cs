using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Modals;
using SpacedRepetitionSystem.Utility.Dialogs;

namespace SpacedRepetitionSystem.Components.Tests.Modals
{
  /// <summary>
  /// Testclass for <see cref="ModalDialogBase"/>
  /// </summary>
  [TestClass]
  public sealed class ModalDialogBaseTests
  {
    private sealed class TestModalDialog : ModalDialogBase
    {
      public void SetButtons(DialogButtons buttons) => Buttons = buttons;

      public bool GetSecondButtonVisible() => SecondButtonVisible;

      public bool GetThirdButtonVisible() => ThirdButtonVisible;

      public string GetFirstButtonText() => FirstButtonText;

      public string GetSecondButtonText() => SecondButtonText;

      public string GetThirdButtonText() => ThirdButtonText;

      public DialogResult CallCalculateDialogResult(DialogButton result) => CalculateDialogResult(result);
    }

    /// <summary>
    /// Tests the visibility of the buttons
    /// </summary>
    [TestMethod]
    public void ButtonVisibilityTest()
    {
      TestModalDialog dialog = new TestModalDialog();
      dialog.SetButtons(DialogButtons.Okay);
      Assert.IsFalse(dialog.GetSecondButtonVisible());
      Assert.IsFalse(dialog.GetThirdButtonVisible());

      dialog.SetButtons(DialogButtons.OkayCancel);
      Assert.IsTrue(dialog.GetSecondButtonVisible());
      Assert.IsFalse(dialog.GetThirdButtonVisible());

      dialog.SetButtons(DialogButtons.YesNo);
      Assert.IsTrue(dialog.GetSecondButtonVisible());
      Assert.IsFalse(dialog.GetThirdButtonVisible());

      dialog.SetButtons(DialogButtons.YesNoCancel);
      Assert.IsTrue(dialog.GetSecondButtonVisible());
      Assert.IsTrue(dialog.GetThirdButtonVisible());
    }

    /// <summary>
    /// Tests thetext of the buttons
    /// </summary>
    [TestMethod]
    public void ButtonTextTest()
    {
      TestModalDialog dialog = new TestModalDialog();
      dialog.SetButtons(DialogButtons.Okay);
      Assert.AreEqual(Messages.Okay, dialog.GetFirstButtonText());
      Assert.IsTrue(string.IsNullOrEmpty(dialog.GetSecondButtonText()));
      Assert.IsTrue(string.IsNullOrEmpty(dialog.GetThirdButtonText()));

      dialog.SetButtons(DialogButtons.OkayCancel);
      Assert.AreEqual(Messages.Okay, dialog.GetFirstButtonText());
      Assert.AreEqual(Messages.Cancel, dialog.GetSecondButtonText());
      Assert.IsTrue(string.IsNullOrEmpty(dialog.GetThirdButtonText()));

      dialog.SetButtons(DialogButtons.YesNo);
      Assert.AreEqual(Messages.Yes, dialog.GetFirstButtonText());
      Assert.AreEqual(Messages.No, dialog.GetSecondButtonText());
      Assert.IsTrue(string.IsNullOrEmpty(dialog.GetThirdButtonText()));

      dialog.SetButtons(DialogButtons.YesNoCancel);
      Assert.AreEqual(Messages.Yes, dialog.GetFirstButtonText());
      Assert.AreEqual(Messages.No, dialog.GetSecondButtonText());
      Assert.AreEqual(Messages.Cancel, dialog.GetThirdButtonText());
    }

    /// <summary>
    /// Tests <see cref="ModalDialogBase.CalculateDialogResult(DialogButton)"/>
    /// </summary>
    [TestMethod]
    public void CallculateDialogResultTest()
    {
      TestModalDialog dialog = new TestModalDialog();
      dialog.SetButtons(DialogButtons.Okay);
      Assert.AreEqual(DialogResult.Okay, dialog.CallCalculateDialogResult(DialogButton.Button1));

      dialog.SetButtons(DialogButtons.OkayCancel);
      Assert.AreEqual(DialogResult.Okay, dialog.CallCalculateDialogResult(DialogButton.Button1));
      Assert.AreEqual(DialogResult.Cancel, dialog.CallCalculateDialogResult(DialogButton.Button2));

      dialog.SetButtons(DialogButtons.YesNo);
      Assert.AreEqual(DialogResult.Yes, dialog.CallCalculateDialogResult(DialogButton.Button1));
      Assert.AreEqual(DialogResult.No, dialog.CallCalculateDialogResult(DialogButton.Button2));

      dialog.SetButtons(DialogButtons.YesNoCancel);
      Assert.AreEqual(DialogResult.Yes, dialog.CallCalculateDialogResult(DialogButton.Button1));
      Assert.AreEqual(DialogResult.No, dialog.CallCalculateDialogResult(DialogButton.Button2));
      Assert.AreEqual(DialogResult.Cancel, dialog.CallCalculateDialogResult(DialogButton.Button3));
    }
  }
}