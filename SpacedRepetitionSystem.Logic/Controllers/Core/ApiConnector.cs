using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Logic.Controllers.Identity;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Core
{
  /// <summary>
  /// Implementation of <see cref="IApiConnector"/>
  /// </summary>
  public sealed class ApiConnector : IApiConnector
  {
    readonly Dictionary<Type, object> controllers = new Dictionary<Type, object>();

    /// <summary>
    /// DbContext
    /// </summary>
    public DbContext Context { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="cardsController">CardsController (Injected)</param>
    /// <param name="decksController">DecksController (Injected)</param>
    /// <param name="cardTemplatesController">CardTemplatesController (Injected)</param>
    /// <param name="practiceHistoryEntryController">PracticeHistoryEntryController (Injected)</param>
    public ApiConnector(DbContext context, EntityControllerBase<Card> cardsController, 
      EntityControllerBase<Deck> decksController, 
      EntityControllerBase<CardTemplate> cardTemplatesController,
      EntityControllerBase<PracticeHistoryEntry> practiceHistoryEntryController,
      EntityControllerBase<User> userController)
    {
      Context = context;
      cardsController.Context = userController.Context = decksController.Context 
        = cardTemplatesController.Context = practiceHistoryEntryController.Context = context;
      controllers.Add(typeof(Card), cardsController);
      controllers.Add(typeof(CardTemplate), cardTemplatesController);
      controllers.Add(typeof(Deck), decksController);
      controllers.Add(typeof(PracticeHistoryEntry), practiceHistoryEntryController);
      controllers.Add(typeof(User), userController);
    }

    ///<inheritdoc/>
    public TEntity Get<TEntity>(object id) where TEntity : IEntity
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Get(id);

    ///<inheritdoc/>
    public Task<List<TEntity>> Get<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IEntity
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Get(searchParameters);

    ///<inheritdoc/>
    public bool Put<TEntity>(TEntity entity) where TEntity : IEntity
    {
      try
      {
        (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Put(entity);
        return true;
      }
      catch (NotifyException ex)
      { NotificationMessageProvider.ShowErrorMessage(ex.Message); }
      return false;
    }

    ///<inheritdoc/>
    public bool Delete<TEntity>(TEntity entity) where TEntity : IEntity
    {
      try
      {
        (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Delete(entity);
        return true;
      }
      catch (NotifyException ex)
      { NotificationMessageProvider.ShowErrorMessage(ex.Message); }
      return false;
    }

    ///<inheritdoc/>
    public bool Post<TEntity>(TEntity entity) where TEntity : IEntity
    {
      try
      {
        (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Post(entity);
        return true;
      }
      catch (NotifyException ex)
      { NotificationMessageProvider.ShowErrorMessage(ex.Message); }
      return false;
    }

    ///<inheritdoc/>
    public async Task<User> Login(string userId, string password)
    { return await (controllers[typeof(User)] as UsersController).Login(userId, password); }

    ///<inheritdoc/>
    public async Task<User> GetUserByAccessTokenAsync(string token)
    { return await (controllers[typeof(User)] as UsersController).GetUserByAccessToken(token); }
  }
}