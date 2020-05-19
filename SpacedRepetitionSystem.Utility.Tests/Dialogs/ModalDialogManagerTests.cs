using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Utility.Dialogs;
using System;

namespace SpacedRepetitionSystem.Utility.Tests.Dialogs
{
  /// <summary>
  /// Testclass for <see cref="ModalDialogManager"/>
  /// </summary>
  [TestClass]
  public sealed class ModalDialogManagerTests 
  {
    /// <summary>
    /// Test for <see cref="ModalDialogManager.ShowDialog(string, string, DialogButtons, Action{DialogResult})"/>
    /// </summary>
    [TestMethod]
    public void ShowDialogTest()
    {
      DialogProviderMock mock = new DialogProviderMock();
      Action<DialogResult> callback = result => { };
      ModalDialogManager.Initialize(mock);
      ModalDialogManager.ShowDialog("title", "text", DialogButtons.OkayCancel, callback);
      Assert.AreEqual("title", mock.Title);
      Assert.AreEqual("text", mock.Text);
      Assert.AreEqual(DialogButtons.OkayCancel, mock.Buttons);
      Assert.AreSame(callback, mock.Callback);
    }
  }
}