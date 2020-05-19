using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.WebAPI.Core;
using System;

namespace SpacedRepetitionSystem.WebApi.Tests
{
  /// <summary>
  /// baseclass for all tests using an ef core Context
  /// </summary>
  public abstract class EntityFrameWorkTestBase
  {
    private static DbContextOptions<SpacedRepetionSystemDBContext> contextOptions;

    /// <summary>
    /// Returns whether a clean db should be created before each test
    /// </summary>
    protected virtual bool CleanupDatabaseBeforeEachTest => false;

    /// <summary>
    /// Constructor
    /// </summary>
    public EntityFrameWorkTestBase()
    {
      contextOptions = new DbContextOptionsBuilder<SpacedRepetionSystemDBContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;
    }

    /// <summary>
    /// Initializes the state before each test
    /// </summary>
    [TestInitialize]
    public virtual void TestInitialize()
    { AssureCleanDatabase(); }

    /// <summary>
    /// Creates a new context
    /// </summary>
    /// <returns></returns>
    protected DbContext CreateContext() => new SpacedRepetionSystemDBContext(contextOptions);

    /// <summary>
    /// Performs the action to create new data
    /// </summary>
    /// <param name="CreateAction">Action to create the data</param>
    protected void CreateData(Action<DbContext> CreateAction)
    {
      using DbContext context = CreateContext();
      CreateAction.Invoke(context);
      context.SaveChanges();
    }

    private void AssureCleanDatabase()
    {
      using DbContext context = CreateContext();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
    }
  }
}