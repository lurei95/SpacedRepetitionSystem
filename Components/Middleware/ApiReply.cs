using System.Net;

namespace SpacedRepetitionSystem.Components.Middleware
{
  /// <summary>
  /// Class for an api reply
  /// </summary>
  public class ApiReply
  {
    /// <summary>
    /// Result message
    /// </summary>
    public string ResultMessage { get; set; }

    /// <summary>
    /// StatusCode
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool WasSuccessful { get; set; }
  }

  /// <summary>
  /// Class for an api reply with a result
  /// </summary>
  /// <typeparam name="TResult">Type of the result</typeparam>
  public sealed class ApiReply<TResult> : ApiReply
  {
    /// <summary>
    /// The result of the operation
    /// </summary>
    public TResult Result { get; set; }
  }
}