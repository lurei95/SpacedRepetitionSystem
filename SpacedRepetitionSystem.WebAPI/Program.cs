using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SpacedRepetitionSystem.WebAPI
{
  /// <summary>
  /// Pragram
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">arguuemnts</param>
    public static void Main(string[] args)
    { CreateHostBuilder(args).Build().Run(); }

    /// <summary>
    /// Creates the hots builder
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}