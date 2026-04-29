using Moq;

using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Services;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Services;

public class DefaultBrowserDomCaptureServiceTests
{
    private readonly DefaultBrowserDomCaptureService _service = new();
    private readonly Mock<IBrowserSession> _sessionMock = new();
    private readonly NavigationOptions _navOptions = new(new Uri("https://example.com"));
    private readonly CaptureSpec _spec = new(
        _ => true,
        (_, _) => Task.FromResult<CapturedResource?>(null),
        null);

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnNullSession()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(null!, _navOptions, _spec, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnNullNavOptions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, null!, _spec, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnNullSpec()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, _navOptions, null!, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ReturnsResult()
    {
        var expected = new PageCaptureResult(
            ResponseHtml: "<html>initial</html>",
            RenderedHtml: "<html>final</html>",
            Resources: [new(new Uri("https://example.com/a"), null, null, null, null, null)],
            PageLoadStatus: null,
            CaptureStatus: null);

        _sessionMock
            .Setup(s => s.NavigateAndCaptureResultAsync(
                PageCaptureParts.ResponseHtml | PageCaptureParts.RenderedHtml | PageCaptureParts.Resources,
                _navOptions,
                _spec,
                null,
                It.IsAny<CaptureTimingOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, _navOptions, _spec, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnIncompleteCapture_StatusUrlChanged()
    {
        _sessionMock
            .Setup(s => s.NavigateAndCaptureResultAsync(
                It.IsAny<PageCaptureParts>(),
                It.IsAny<NavigationOptions>(),
                It.IsAny<CaptureSpec>(),
                It.IsAny<RewriteSpec?>(),
                It.IsAny<CaptureTimingOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, [], null, CaptureStatus.UrlChangedBeforeCompletion));

        await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, _navOptions, _spec, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnIncompleteCapture_StatusTimeout()
    {
        _sessionMock
            .Setup(s => s.NavigateAndCaptureResultAsync(
                It.IsAny<PageCaptureParts>(),
                It.IsAny<NavigationOptions>(),
                It.IsAny<CaptureSpec>(),
                It.IsAny<RewriteSpec?>(),
                It.IsAny<CaptureTimingOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, [], null, CaptureStatus.CaptureTimeoutExceeded));

        await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, _navOptions, _spec, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureHtmlAndResourcesAsync_ThrowsOnIncompleteCapture_PageLoadTimeout()
    {
        _sessionMock
            .Setup(s => s.NavigateAndCaptureResultAsync(
                It.IsAny<PageCaptureParts>(),
                It.IsAny<NavigationOptions>(),
                It.IsAny<CaptureSpec>(),
                It.IsAny<RewriteSpec?>(),
                It.IsAny<CaptureTimingOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(
                null,
                null,
                [],
                PageLoadStatus.NetworkIdleTimeoutExceeded,
                CaptureStatus.CriteriaSatisfied));

        var ex = await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureHtmlAndResourcesResultAsync(_sessionMock.Object, _navOptions, _spec, CancellationToken.None));

        Assert.Equal(PageLoadStatus.NetworkIdleTimeoutExceeded, ex.PageLoadStatus);
    }
}
