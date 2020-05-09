using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
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
    protected override async Task<List<CardTemplate>> SearchCore() => await ApiConnector.Get<CardTemplate>(null);

    ///<inheritdoc/>
    protected override void DeleteEntity(CardTemplate entity)
    {
      ModalDialogManager.ShowDialog(Messages.DeleteCardTemplateDialogTitle,
        Messages.DeleteCardTemplateDialogText.FormatWith(entity.Title), DialogButtons.YesNo, (result) =>
      {
        if (result == DialogResult.Yes)
          base.DeleteEntity(entity);
      });
    }
  }
}