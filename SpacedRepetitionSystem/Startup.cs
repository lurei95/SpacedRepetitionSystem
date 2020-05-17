using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Core;
using SpacedRepetitionSystem.Components.ViewModels.Cards;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Entities.Validation.Decks;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using SpacedRepetitionSystem.Components.ViewModels.Statistics;
using SpacedRepetitionSystem.Components.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Components.ViewModels.Identity;
using Blazored.LocalStorage;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Components.Middleware;

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

      services.AddBlazorise(options => { options.ChangeTextOnKeyPress = true; })
        .AddBootstrapProviders()
        .AddFontAwesomeIcons();

      services.AddSingleton<IApiConnector, ApiConnector>();

      //Authentication
      services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

      //Storage
      services.AddBlazoredLocalStorage();

      //ViewModels
      services.AddTransient<HomeViewModel>();
      services.AddTransient<CardEditViewModel>();
      services.AddTransient<CardTemplateEditViewModel>();
      services.AddTransient<DeckEditViewModel>();
      services.AddTransient<CardSearchViewModel>();
      services.AddTransient<CardTemplateSearchViewModel>();
      services.AddTransient<DeckSearchViewModel>();
      services.AddTransient<PracticeDeckViewModel>();
      services.AddTransient<CardStatisticsViewModel>();
      services.AddTransient<DeckStatisticsViewModel>();
      services.AddTransient<SignupViewModel>();
      services.AddTransient<LoginViewModel>();

      services.AddHttpClient("Default", client => client.BaseAddress = new System.Uri(Configuration.GetConnectionString("Default")));

      //Validators
      services.AddCardTemplatePropertyValidator();
      services.AddCardPropertyValidator();
      services.AddDecksPropertyValidator();
      services.AddUserPropertyValidator();
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
      app.UseAuthentication();
      app.UseAuthentication();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
      });
    }
  }
}