using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Core
{
  /// <summary>
  /// Middleware for handlingf exceptions occuring in the controllers
  /// </summary>
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate next;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Request Delegate</param>
    public ErrorHandlingMiddleware(RequestDelegate next) { this.next = next; }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context">Context</param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
      var code = HttpStatusCode.InternalServerError;
      var result = JsonConvert.SerializeObject(ex, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)code;
      await context.Response.WriteAsync(result);
    }
  }
}
