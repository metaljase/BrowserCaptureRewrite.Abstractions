using Microsoft.Extensions.Logging;
using Moq;

using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;
using Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Resilience;

public class ResiliencePolicyBuilderTests
{
    [Fact]
    public async Task BuildTransportRetryPolicy_InvokesLoggerOnRetry()
    {
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var url = new Uri("https://example.com");
        var delays = new List<TimeSpan> { TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(20) };
        int attempts = 0;
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.NotConnectivityRelated);
        var probe = new Mock<IConnectivityProbe>();
        probe.Setup(p => p.HasGeneralConnectivityAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);
        var policy = builder.BuildTransportRetryPolicy<object>(
            url,
            delays,
            ex => true,
            CancellationToken.None);

        await Assert.ThrowsAsync<Exception>(() => policy.ExecuteAsync(() =>
        {
            attempts++;
            return Task.FromException<object>(new Exception("fail"));
        }));

        // Should log warning for each attempt
        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error fetching URL")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(delays.Count));

        // Should log info for retry/final retry
        logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrying") || v.ToString()!.Contains("FINAL retry")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(delays.Count));
    }

    [Fact]
    public async Task BuildTimeoutRetryPolicy_LogsTimeoutAndTaskCanceled()
    {
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var url = new Uri("https://example.com");
        var delays = new List<TimeSpan> { TimeSpan.FromMilliseconds(10) };
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.NotConnectivityRelated);
        var probe = new Mock<IConnectivityProbe>();
        probe.Setup(p => p.HasGeneralConnectivityAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var timeout = TimeSpan.FromSeconds(5);
        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);
        var policy = builder.BuildTimeoutRetryPolicy<object>(url, delays, timeout, CancellationToken.None);

        // TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new TimeoutException())));
        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Timeout exceeded")),
            It.IsAny<TimeoutException>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce());

        // TaskCanceledException
        logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);
        policy = builder.BuildTimeoutRetryPolicy<object>(
            url, delays, timeout, CancellationToken.None);
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new TaskCanceledException())));
        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Timeout exceeded")),
            It.IsAny<TaskCanceledException>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task BuildTimeoutRetryPolicy_DoesNotLogForUserCancelledTask()
    {
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var url = new Uri("https://example.com");
        var delays = new List<TimeSpan> { TimeSpan.FromMilliseconds(10) };
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.NotConnectivityRelated);
        var probe = new Mock<IConnectivityProbe>();
        probe.Setup(p => p.HasGeneralConnectivityAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var timeout = TimeSpan.FromSeconds(5);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);
        var policy = builder.BuildTimeoutRetryPolicy<object>(url, delays, timeout, cts.Token);

        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new TaskCanceledException())));

        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<TaskCanceledException>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never());
    }

    [Fact]
    public async Task BuildFallbackPolicy_ThrowsConnectivityException_ForLocalEnvironment()
    {
        var url = new Uri("https://example.com");
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.ConnectivityRelated(ConnectivityScope.LocalEnvironment));
        var probe = new Mock<IConnectivityProbe>();
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);

        var policy = builder.BuildFallbackPolicy<object>(
            url,
            ex => true,
            CancellationToken.None);

        var ex = await Assert.ThrowsAsync<ConnectivityException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new Exception("fail"))));
        Assert.Equal(ConnectivityScope.LocalEnvironment, ex.Scope);
        Assert.Equal(url.ToString(), ex.Data["Url"]);
    }

    [Fact]
    public async Task BuildFallbackPolicy_ThrowsHttpRequestException_ForRemoteSite()
    {
        var url = new Uri("https://example.com");
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.ConnectivityRelated(ConnectivityScope.RemoteSite));
        var probe = new Mock<IConnectivityProbe>();
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);

        var policy = builder.BuildFallbackPolicy<object>(
            url,
            ex => true,
            CancellationToken.None);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new Exception("fail"))));
        Assert.Equal(url.ToString(), ex.Data["Url"]);
    }

    [Fact]
    public async Task BuildFallbackPolicy_RethrowsOriginalException_WhenNotConnectivityRelated()
    {
        var url = new Uri("https://example.com");
        var classifier = new Mock<IConnectivityClassifier>();
        classifier.Setup(c => c.ClassifyException(It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(ConnectivityClassificationResult.NotConnectivityRelated);
        var probe = new Mock<IConnectivityProbe>();
        var logger = new Mock<ILogger<ResiliencePolicyBuilder>>();
        var builder = new ResiliencePolicyBuilder(logger.Object, classifier.Object, probe.Object);

        var policy = builder.BuildFallbackPolicy<object>(
            url,
            ex => true,
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            policy.ExecuteAsync(() => Task.FromException<object>(new InvalidOperationException("fail"))));
    }

    [Theory]
    [InlineData(0.5, "500 ms")]
    [InlineData(2, "2 secs")]
    [InlineData(9.5, "9.5 secs")]
    [InlineData(30, "30 secs")]
    [InlineData(90, "1.5 mins")]
    [InlineData(600, "10 mins")]
    [InlineData(3600, "1 hr")]
    [InlineData(7200, "2 hrs")]
    [InlineData(86400, "1 day")]
    [InlineData(172800, "2 days")]
    public void FormatDuration_ReturnsExpectedString(double seconds, string expected)
    {
        var value = TimeSpan.FromSeconds(seconds);
        var result = HumanizeHelper.FormatDuration(value);
        Assert.Equal(expected, result!);
    }
}
