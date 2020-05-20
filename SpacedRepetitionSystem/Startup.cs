using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpacedRepetitionSystem.Entities;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.ViewModels.Cards;
using SpacedRepetitionSystem.ViewModels;
using SpacedRepetitionSystem.ViewModels.Identity;
using SpacedRepetitionSystem.ViewModels.Statistics;

namespace SpacedRepetitionSystem
{
  /// <summary>
  /// Startup class
  /// </summary>
  public class Startup
  {
    /// <summary>
    /// The configuration
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public Startup(IConfiguration configuration)
    { Configuration = configuration; }

    /// <summary>
    /// Configures DI
    /// </summary>
    /// <param name="services">Service collection</param>
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

    /// <summary>
    /// Confiugures the HTTP pipeline
    /// </summary>
    /// <param name="app">app builder</param>
    /// <param name="env">Host environment</param>
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