using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Core;
using SpacedRepetitionSystem.Components.ViewModels.Cards;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Logic.Controllers.Cards;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Entities.Validation.Decks;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;

namespace SpacedRepetitionSystem
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    { Configuration = configuration; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddRazorPages();
      services.AddServerSideBlazor();

      services.AddDbContext<SpacedRepetionSystemDBContext>(
        options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
      services.AddScoped<DbContext, SpacedRepetionSystemDBContext>();

      services.AddTransient<CardEditViewModel>();
      services.AddTransient<CardTemplateEditViewModel>();
      services.AddTransient<DeckEditViewModel>();
      services.AddTransient<CardSearchViewModel>();
      services.AddTransient<CardTemplateSearchViewModel>();
      services.AddTransient<DeckSearchViewModel>();
      services.AddTransient<PracticeDeckViewModel>();

      services.AddScoped<EntityControllerBase<Card>, CardsController>();
      services.AddScoped<EntityControllerBase<Deck>, DecksController>();
      services.AddScoped<EntityControllerBase<CardTemplate>, CardTemplatesController>();
      services.AddScoped<IApiConnector, ApiConnector>();

      services.AddScoped(typeof(CommitValidatorBase<>), typeof(CommitValidatorBase<>));
      services.AddScoped(typeof(DeleteValidatorBase<>), typeof(DeleteValidatorBase<>));
      services.AddValidator<CommitValidatorBase<Card>, CardCommitValidator>();
      services.AddValidator<CommitValidatorBase<Deck>, DeckCommitValidator>();
      services.AddValidator<CommitValidatorBase<CardTemplate>, CardTemplateCommitValidator>();
      services.AddValidator<DeleteValidatorBase<CardTemplate>, CardTemplateDeleteValidator>();
      services.AddCardTemplatePropertyValidator();
      services.AddCardPropertyValidator();
      services.AddDecksPropertyValidator();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
      });
    }
  }
}