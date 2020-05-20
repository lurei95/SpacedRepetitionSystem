using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Dialogs;
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
    protected override async Task<List<CardTemplate>> SearchCore()
      => (await ApiConnector.GetAsync <CardTemplate>(new Dictionary<string, object>())).Result;

    ///<inheritdoc/>
    protected override async Task DeleteEntity(CardTemplate entity)
    {
      ModalDialogManager.ShowDialog(Messages.DeleteCardTemplateDialogTitle,
        Messages.DeleteCardTemplateDialogText.FormatWith(entity.Title), DialogButtons.YesNo, async (result) =>
      {
        if (result == DialogResult.Yes)
          await base.DeleteEntity(entity);
      });
      await Task.FromResult<object>(null);
    }
  }
}