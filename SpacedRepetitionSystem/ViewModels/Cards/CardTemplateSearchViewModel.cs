﻿using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateSearchViewModel : SearchViewModelBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public CardTemplateSearchViewModel(NavigationManager navigationManager, IApiConnector apiConnector)
      : base(navigationManager, apiConnector)
    { }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      DeleteCommand.DeleteDialogTitle = Messages.DeleteCardTemplateDialogTitle;
      DeleteCommand.DeleteDialogTextFactory = (entity) => Messages.DeleteCardTemplateDialogText.FormatWith(entity.Title);
      return true;
    }

    ///<inheritdoc/>
    protected override async Task<ApiReply<List<CardTemplate>>> SearchCore()
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(SearchText))
        parameters.Add(nameof(SearchText), SearchText);
      return await ApiConnector.GetAsync<CardTemplate>(parameters);
    }
  }
}