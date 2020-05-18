using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SpacedRepetitionSystem
{
  /// <summary>
  /// Program
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">Arguments</param>
    public static void Main(string[] args)
    { CreateHostBuilder(args).Build().Run(); }

    /// <summary>
    /// Creates the host builder
    /// </summary>
    /// <param name="args">args</param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
      {  webBuilder.UseStartup<Startup>();  });
    }
  }
}