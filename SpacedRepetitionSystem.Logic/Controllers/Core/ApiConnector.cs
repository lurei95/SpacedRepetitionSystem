using SpacedRepetitionSystem.Entities.Entities.SmartCards;
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
    
    public ApiConnector(EntityControllerBase<SmartCard> smartCardController, 
      EntityControllerBase<PracticeSet> practiceSetController, 
      EntityControllerBase<SmartCardDefinition> smartCardDefinitionController)
    {
      controllers.Add(typeof(SmartCard), smartCardController);
      controllers.Add(typeof(SmartCardDefinition), smartCardDefinitionController);
      controllers.Add(typeof(PracticeSet), practiceSetController);
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
