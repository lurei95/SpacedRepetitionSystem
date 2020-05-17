using System;
using System.Runtime.Serialization;

namespace SpacedRepetitionSystem.Utility.Notification
{
  /// <summary>
  /// A exception which can be notified via <see cref="NotificationMessageProvider"/>
  /// </summary>
  [Serializable]
  public class NotifyException : Exception
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public NotifyException()
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">Exception message</param>
    public NotifyException(string message) : base(message)
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public NotifyException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Streaming context</param>
    public NotifyException(SerializationInfo info, StreamingContext context) 
      : this(info.GetString(nameof(Message)))
    { }
  }
}