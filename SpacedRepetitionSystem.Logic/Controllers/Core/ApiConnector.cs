using SpacedRepetitionSystem.Entities.Entities.Cards;
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
    
    public ApiConnector(EntityControllerBase<Card> cardsController, 
      EntityControllerBase<Deck> decksController, 
      EntityControllerBase<CardTemplate> cardTemplatesController)
    {
      controllers.Add(typeof(Card), cardsController);
      controllers.Add(typeof(CardTemplate), cardTemplatesController);
      controllers.Add(typeof(Deck), decksController);
    }

    ///<inheritdoc/>
    public TEntity Get<TEntity>(object id) 
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Get(id);

    ///<inheritdoc/>
    public Task<List<TEntity>> Get<TEntity>(IDictionary<string, object> searchParameters) 
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Get(searchParameters);

    ///<inheritdoc/>
    public void Put<TEntity>(TEntity entity) 
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Put(entity);

    ///<inheritdoc/>
    public void Delete<TEntity>(TEntity entity)
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Delete(entity);

    ///<inheritdoc/>
    public void Post<TEntity>(TEntity entity) 
      => (controllers[typeof(TEntity)] as EntityControllerBase<TEntity>).Post(entity);
  }
}
