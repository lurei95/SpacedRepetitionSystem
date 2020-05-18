using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Decks;
using SpacedRepetitionSystem.WebAPI.Core;

namespace SpacedRepetitionSystem.WebAPI
{
  /// <summary>
  /// Startup
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
      //EF Core
      services.AddControllers();
      services.AddDbContext<SpacedRepetionSystemDBContext>(
        options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
      services.AddScoped<DbContext, SpacedRepetionSystemDBContext>();

      services.AddMvc(option =>option.EnableEndpointRouting = false)
        .SetCompatibilityVersion(CompatibilityVersion.Latest)
        .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      //Authentication
      IConfigurationSection jwtSection = Configuration.GetSection("JWTSettings");
      services.Configure<JWTSettings>(jwtSection);
      JWTSettings jwtSettings = jwtSection.Get<JWTSettings>();
      byte[] key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          ClockSkew = TimeSpan.Zero
        };
      });

      //Validators
      services.AddScoped(typeof(CommitValidatorBase<>), typeof(CommitValidatorBase<>));
      services.AddScoped(typeof(DeleteValidatorBase<>), typeof(DeleteValidatorBase<>));
      services.AddValidator<CommitValidatorBase<Card>, CardCommitValidator>();
      services.AddValidator<CommitValidatorBase<Deck>, DeckCommitValidator>();
      services.AddValidator<CommitValidatorBase<User>, UserCommitValidator>();
      services.AddValidator<CommitValidatorBase<CardTemplate>, CardTemplateCommitValidator>();
      services.AddValidator<DeleteValidatorBase<CardTemplate>, CardTemplateDeleteValidator>();
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
      app.UseMiddleware(typeof(ErrorHandlingMiddleware));
      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}