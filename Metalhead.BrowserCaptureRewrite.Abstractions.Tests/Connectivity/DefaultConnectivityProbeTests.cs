using System.Net;

using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Connectivity;

public class DefaultConnectivityProbeTests
{
    [Fact]
    public async Task HasGeneralConnectivityAsync_Status200_ReturnsTrue()
    {
        var handler = new TestHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        using var client = new HttpClient(handler);
        var probe = new DefaultConnectivityProbe(client, new ConnectivityProbeOptions());

        var result = await probe.HasGeneralConnectivityAsync(TestContext.Current.CancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task HasGeneralConnectivityAsync_NonSuccessStatus_ReturnsFalse()
    {
        var handler = new TestHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        using var client = new HttpClient(handler);
        var probe = new DefaultConnectivityProbe(client, new ConnectivityProbeOptions());

        var result = await probe.HasGeneralConnectivityAsync(TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    public async Task HasGeneralConnectivityAsync_RequestThrows_ReturnsFalse()
    {
        var handler = new TestHandler((_, _) => throw new HttpRequestException("boom"));
        using var client = new HttpClient(handler);
        var probe = new DefaultConnectivityProbe(client, new ConnectivityProbeOptions());

        var result = await probe.HasGeneralConnectivityAsync(TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    private sealed class TestHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => handler(request, cancellationToken);
    }
}
