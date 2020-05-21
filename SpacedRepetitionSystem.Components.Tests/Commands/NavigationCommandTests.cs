using Blazorise;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpacedRepetitionSystem.Components.Tests.Commands
{
  /// <summary>
  /// Testclass for <see cref="NavigationCommand"/>
  /// </summary>
  [TestClass]
  public sealed class NavigationCommandTests
  {
    private static NavigationCommand command;
    private static readonly NavigationManagerMock mock = new NavigationManagerMock();

    /// <summary>
    /// Method for initializing the test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    { 
      command = new NavigationCommand(mock);
      mock.SetUri("http://localhost:2112/base/");
    }

    /// <summary>
    /// Tests <see cref="NavigationCommand.ExecuteCommand(object)"/> with <see cref="NavigationCommand.TargetUri"/> set
    /// </summary>
    [TestMethod]
    public void ExecuteWithTargetUriFactoryTest()
    {
      Card card = new Card();
      command.TargetUriFactory = (param) =>
      {
        Assert.AreSame(param, card);
        return "test";
      };
      command.ExecuteCommand(card);
      Assert.AreEqual("test", mock.NavigatedUri);
    }

    /// <summary>
    /// Tests <see cref="NavigationCommand.ExecuteCommand(object)"/> with <see cref="NavigationCommand.TargetUri"/> set
    /// </summary>
    [TestMethod]
    public void ExecuteWithTargetUriTest()
    {
      command.TargetUri = "test";
      command.ExecuteCommand();
      Assert.AreEqual("test", mock.NavigatedUri);
    }

    /// <summary>
    /// Tests <see cref="NavigationCommand.ExecuteCommand(object)"/> with <see cref="NavigationCommand.TargetUri"/> 
    /// and <see cref="NavigationCommand.IsRelative"/> set
    /// </summary>
    [TestMethod]
    public void ExecuteWithRelativeTargetUriTest()
    {
      command.TargetUri = "test";
      command.IsRelative = true;
      command.ExecuteCommand();
      Assert.AreEqual("http://localhost:2112/base/test", mock.NavigatedUri);
    }

    /// <summary>
    /// Tests <see cref="NavigationCommand.ExecuteCommand(object)"/> with <see cref="NavigationCommand.TargetUri"/> set
    /// </summary>
    [TestMethod]
    public void ExecuteWithTargetUriFactoryRelativeTest()
    {
      Card card = new Card();
      command.IsRelative = true;
      command.TargetUriFactory = (param) =>
      {
        Assert.AreSame(param, card);
        return "test";
      };
      command.ExecuteCommand(card);
      Assert.AreEqual("http://localhost:2112/base/test", mock.NavigatedUri);
    }
  }
}
