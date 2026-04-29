using Moq;

using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Services;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Services;

public class DefaultBrowserDomServiceTests
{
    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAsync_ThrowsIfSessionNull()
    {
        var service = new DefaultBrowserDomService();
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.NavigateAndCaptureResponseHtmlAsync(
            null!, new NavigationOptions(new Uri("https://example.com")), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAsync_ThrowsIfNavOptionsNull()
    {
        var service = new DefaultBrowserDomService();
        var sessionMock = new Mock<IBrowserSession>();
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.NavigateAndCaptureResponseHtmlAsync(
            sessionMock.Object, null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAsync_ReturnsResponseHtml()
    {
        var sessionMock = new Mock<IBrowserSession>();
        var navOptions = new NavigationOptions(new Uri("https://example.com"));
        sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            PageCaptureParts.ResponseHtml,
            navOptions,
            null,
            null,
            It.IsAny<CaptureTimingOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult("<html>initial</html>", null, [], null, null));

        var service = new DefaultBrowserDomService();
        var result = await service.NavigateAndCaptureResponseHtmlAsync(sessionMock.Object, navOptions, TestContext.Current.CancellationToken);
        Assert.Equal("<html>initial</html>", result);
    }

    [Fact]
    public async Task NavigateAndCaptureRenderedHtmlAsync_ThrowsIfSessionNull()
    {
        var service = new DefaultBrowserDomService();
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
        service.NavigateAndCaptureRenderedHtmlAsync(
            null!,
            new NavigationOptions(new Uri("https://example.com")),
            cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureRenderedHtmlAsync_ThrowsIfNavOptionsNull()
    {
        var service = new DefaultBrowserDomService();
        var sessionMock = new Mock<IBrowserSession>();
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.NavigateAndCaptureRenderedHtmlAsync(
            sessionMock.Object, null!, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureRenderedHtmlAsync_ReturnsRenderedHtml()
    {
        var sessionMock = new Mock<IBrowserSession>();
        var navOptions = new NavigationOptions(new Uri("https://example.com"));
        var captureTimingOptions = new CaptureTimingOptions(TimeSpan.FromSeconds(1), pollInterval: CaptureTimingOptions.DefaultPollInterval);
        sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            PageCaptureParts.RenderedHtml,
            navOptions,
            null,
            null,
            It.IsAny<CaptureTimingOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult(null, "<html>final</html>", [], null, null));

        var service = new DefaultBrowserDomService();
        var result = await service.NavigateAndCaptureRenderedHtmlAsync(
            sessionMock.Object, navOptions, TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("<html>final</html>", result);
    }

    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAndRenderedHtmlAsync_ThrowsIfSessionNull()
    {
        var service = new DefaultBrowserDomService();
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
        service.NavigateAndCaptureResponseAndRenderedHtmlAsync
        (null!,
        new NavigationOptions(new Uri("https://example.com")),
        cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAndRenderedHtmlAsync_ThrowsIfNavOptionsNull()
    {
        var service = new DefaultBrowserDomService();
        var sessionMock = new Mock<IBrowserSession>();
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.NavigateAndCaptureResponseAndRenderedHtmlAsync(
            sessionMock.Object, null!, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NavigateAndCaptureResponseHtmlAndRenderedHtmlAsync_ReturnsBothHtml()
    {
        var sessionMock = new Mock<IBrowserSession>();
        var navOptions = new NavigationOptions(new Uri("https://example.com"));
        sessionMock.Setup(s => s.NavigateAndCaptureResultAsync(
            PageCaptureParts.ResponseHtml | PageCaptureParts.RenderedHtml,
            navOptions,
            null,
            null,
            It.IsAny<CaptureTimingOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageCaptureResult("<html>initial</html>", "<html>final</html>", [], null, null));

        var service = new DefaultBrowserDomService();
        var (responseHtml, renderedHtml) = await service.NavigateAndCaptureResponseAndRenderedHtmlAsync(
            sessionMock.Object, navOptions, TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("<html>initial</html>", responseHtml);
        Assert.Equal("<html>final</html>", renderedHtml);
    }
}
