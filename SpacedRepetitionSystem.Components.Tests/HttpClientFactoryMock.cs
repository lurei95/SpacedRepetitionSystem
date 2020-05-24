using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests
{
  /// <summary>
  /// Mock for <see cref="IHttpClientFactory"/>
  /// </summary>
  public sealed class HttpClientFactoryMock : IHttpClientFactory
  {
    /// <summary>
    /// The response to return
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; set; }

    /// <summary>
    /// The request that was sent
    /// </summary>
    public HttpRequestMessage RequestMessage { get; set; }

    /// <summary>
    /// The Baseaddress
    /// </summary>
    public string BaseAddress => "https://www.test.com/";

    ///<inheritdoc/>
    public HttpClient CreateClient(string name)
    {
      Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
      mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
          .Callback<HttpRequestMessage, CancellationToken>((message, token) => RequestMessage = message)
          .ReturnsAsync(ResponseMessage);
      HttpClient client = new HttpClient(mockHttpMessageHandler.Object)
      { BaseAddress = new System.Uri(BaseAddress) };
      return client;
    }
  }
}