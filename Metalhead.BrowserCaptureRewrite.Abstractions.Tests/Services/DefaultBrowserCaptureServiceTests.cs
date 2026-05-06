using Moq;

using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Services;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Services;

public class DefaultBrowserCaptureServiceTests
{
    private readonly DefaultBrowserCaptureService _service = new();
    private readonly Mock<IBrowserSession> _sessionMock = new();
    private readonly Uri _testUrl = new("https://example.com");
    private readonly string[] _extensions = [".js"];
    private readonly NavigationOptions _navOptions;
    private readonly CaptureSpec _spec;
    private readonly CancellationToken _token = CancellationToken.None;

    public DefaultBrowserCaptureServiceTests()
    {
        _navOptions = new NavigationOptions(_testUrl, null, null);
        _spec = new CaptureSpec(
            _ => true,
            (_, _) => Task.FromResult<CapturedResource?>(null),
            shouldCompleteCapture: null);
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByFileExtensionAsync_ThrowsOnNullSession()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByFileExtensionAsync(null!, _navOptions, _extensions, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByFileExtensionAsync_ThrowsOnNullNavOptions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByFileExtensionAsync(_sessionMock.Object, null!, _extensions, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByFileExtensionAsync_ThrowsOnNullExtensions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByFileExtensionAsync(_sessionMock.Object, _navOptions, (string[])null!, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnNullSession()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesAsync(null!, _navOptions, _spec, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnNullNavOptions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, null!, _spec, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnNullSpec()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, (CaptureSpec)null!, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnIncompleteCapture_StatusUrlChanged()
    {
        _sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            It.IsAny<PageCaptureParts>(), It.IsAny<NavigationOptions>(), It.IsAny<CaptureSpec>(), It.IsAny<RewriteSpec?>(), It.IsAny<CaptureTimingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, [], null, CaptureStatus.UrlChangedBeforeCompletion));

        await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, _spec, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnIncompleteCapture_StatusTimeout()
    {
        _sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            It.IsAny<PageCaptureParts>(), It.IsAny<NavigationOptions>(), It.IsAny<CaptureSpec>(), It.IsAny<RewriteSpec?>(), It.IsAny<CaptureTimingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, [], null, CaptureStatus.CaptureTimeoutExceeded));

        await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, _spec, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ReturnsResources_WhenNoStatus()
    {
        var expected = new List<CapturedResource> {
            new(_testUrl, null, null, null, null, null)
        };
        _sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            It.IsAny<PageCaptureParts>(), It.IsAny<NavigationOptions>(), It.IsAny<CaptureSpec>(), It.IsAny<RewriteSpec?>(), It.IsAny<CaptureTimingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, expected, null, null));

        var result = await _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, _spec, _token);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ReturnsResources_WhenStatusOtherThanIncomplete()
    {
        var expected = new List<CapturedResource> {
            new(_testUrl, null, null, null, null, null)
        };
        _sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            It.IsAny<PageCaptureParts>(), It.IsAny<NavigationOptions>(), It.IsAny<CaptureSpec>(), It.IsAny<RewriteSpec?>(), It.IsAny<CaptureTimingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, expected, null, CaptureStatus.CriteriaSatisfied));

        var result = await _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, _spec, _token);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnIncompleteCapture_PageLoadTimeout()
    {
        _sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            It.IsAny<PageCaptureParts>(), It.IsAny<NavigationOptions>(), It.IsAny<CaptureSpec>(), It.IsAny<RewriteSpec?>(), It.IsAny<CaptureTimingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, null, [], PageLoadStatus.NetworkIdleTimeoutExceeded, CaptureStatus.CriteriaSatisfied));

        var ex = await Assert.ThrowsAsync<PageCaptureIncompleteException>(() =>
            _service.NavigateAndCaptureResourcesAsync(_sessionMock.Object, _navOptions, _spec, _token));
        Assert.Equal(PageLoadStatus.NetworkIdleTimeoutExceeded, ex.PageLoadStatus);
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnNullSession()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByContentTypeAsync(null!, _navOptions, ["application/json"], _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnNullNavOptions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByContentTypeAsync(_sessionMock.Object, null!, ["application/json"], _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnNullContentTypes()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.NavigateAndCaptureResourcesByContentTypeAsync(_sessionMock.Object, _navOptions, null!, _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnEmptyContentTypes()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.NavigateAndCaptureResourcesByContentTypeAsync(_sessionMock.Object, _navOptions, [], _token));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnBlankContentTypes()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.NavigateAndCaptureResourcesByContentTypeAsync(_sessionMock.Object, _navOptions, ["", "  "], _token));
    }
}
