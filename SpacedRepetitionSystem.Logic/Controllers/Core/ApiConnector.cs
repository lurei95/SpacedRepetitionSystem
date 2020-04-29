using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
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
    /// Constructor
    /// </summary>
    /// <param name="cardsController">CardsController</param>
    /// <param name="decksController">DecksController</param>
    /// <param name="cardTemplatesController">CardTemplatesController</param>
    public ApiConnector(EntityControllerBase<Card> cardsController, 
      EntityControllerBase<Deck> decksController, 
      EntityControllerBase<CardTemplate> cardTemplatesController)
    {
      controllers.Add(typeof(Card), cardsController);
      controllers.Add(typeof(CardTemplate), cardTemplatesController);
      controllers.Add(typeof(Deck), decksController);
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
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(entity.GetDisplayName()));
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
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(entity.GetDisplayName()));
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
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(entity.GetDisplayName()));
        return true;
      }
      catch (NotifyException ex)
      { NotificationMessageProvider.ShowErrorMessage(ex.Message); }
      return false;
    }
  }
}
