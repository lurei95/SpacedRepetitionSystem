﻿using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.SmartCards
{
  /// <summary>
  /// SearchViewModel for <see cref="SmartCardDefinition"/>
  /// </summary>
  public sealed class SmartCardDefinitionSearchViewModel : SearchViewModelBase<SmartCardDefinition>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="controller">Controller (Injected)</param>
    public SmartCardDefinitionSearchViewModel(DbContext context, NavigationManager navigationManager, EntityControllerBase<SmartCardDefinition> controller)
      : base(context, navigationManager, controller)
    { }

    ///<inheritdoc/>
    protected override async Task<List<SmartCardDefinition>> SearchCore() => await Controller.Get(null);
  }
}