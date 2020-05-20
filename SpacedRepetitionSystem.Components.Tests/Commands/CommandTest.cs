using Blazorise;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Commands;
using System;

namespace SpacedRepetitionSystem.Components.Tests.Commands
{
  /// <summary>
  /// Testclass for <see cref="Command"/>
  /// </summary>
  [TestClass]
  public sealed class CommandTest
  {
    /// <summary>
    /// Tests <see cref="Command.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteTest()
    {
      bool wasExceuted = false;
      object param1 = new Card();
      void action(object param)
      {
        wasExceuted = true;
        Assert.AreSame(param1, param);
      }
      Command command = new Command() { ExecuteAction = action };
      command.ExecuteCommand(param1);
      Assert.IsTrue(wasExceuted);
    }
  }
}