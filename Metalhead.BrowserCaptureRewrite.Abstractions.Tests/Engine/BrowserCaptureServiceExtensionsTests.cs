using Moq;

using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Engine;

public class BrowserCaptureServiceExtensionsTests
{
    [Fact]
    public async Task NavigateAndCaptureResourcesByFileExtensionAsync_OverloadWithFileExtensions_InvokesService()
    {
        var service = new Mock<IBrowserCaptureService>(MockBehavior.Strict);
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var expected = new List<CapturedResource>();
        service.Setup(s => s.NavigateAndCaptureResourcesByFileExtensionAsync(
            session,
            It.IsAny<NavigationOptions>(),
            It.IsAny<string[]>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RewriteSpec?>(),
            null,
            It.IsAny<CaptureTimingOptions>()))
            .ReturnsAsync(expected)
            .Verifiable();

        var result = await service.Object.NavigateAndCaptureResourcesByFileExtensionAsync(
            session,
            url,
            It.IsAny<string[]>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Uri?>(),
            null,
            null,
            null,
            null,
            It.IsAny<RewriteSpec?>(),
            null);

        Assert.Equal(expected, result);
        service.Verify();
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByUrlAsync_OverloadWithUris_InvokesService()
    {
        var service = new Mock<IBrowserCaptureService>(MockBehavior.Strict);
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var expected = new List<CapturedResource>();
        service.Setup(s => s.NavigateAndCaptureResourcesByUrlAsync(
            session,
            It.IsAny<NavigationOptions>(),
            It.IsAny<Uri[]>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RewriteSpec?>(),
            It.IsAny<CaptureTimingOptions>()))
            .ReturnsAsync(expected)
            .Verifiable();

        var result = await service.Object.NavigateAndCaptureResourcesByUrlAsync(
            session,
            url,
            It.IsAny<Uri[]>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Uri?>(),
            null,
            null,
            null,
            null,
            It.IsAny<RewriteSpec?>());

        Assert.Equal(expected, result);
        service.Verify();
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_OverloadWithCaptureSpec_InvokesService()
    {
        var service = new Mock<IBrowserCaptureService>(MockBehavior.Strict);
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var captureSpec = new CaptureSpec(_ => true, async (req, resp) => null, null);
        var expected = new List<CapturedResource>();
        service.Setup(s => s.NavigateAndCaptureResourcesAsync(
            session,
            It.IsAny<NavigationOptions>(),
            captureSpec,
            null,
            It.IsAny<CancellationToken>(),
            It.IsAny<CaptureTimingOptions>()))
            .ReturnsAsync(expected)
            .Verifiable();

        var result = await service.Object.NavigateAndCaptureResourcesAsync(session, url, captureSpec, CancellationToken.None);

        Assert.Equal(expected, result);
        service.Verify();
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_OverloadWithCaptureSpecAndRewriteSpec_InvokesService()
    {
        var service = new Mock<IBrowserCaptureService>(MockBehavior.Strict);
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var captureSpec = new CaptureSpec(_ => true, async (req, resp) => null, null);
        var rewriteSpec = new RewriteSpec(_ => true, async (req, resp) =>
            new ResponseRewriteResult(false, null, null));
        var expected = new List<CapturedResource>();
        service.Setup(s => s.NavigateAndCaptureResourcesAsync(
            session,
            It.IsAny<NavigationOptions>(),
            captureSpec,
            rewriteSpec,
            It.IsAny<CancellationToken>(),
            It.IsAny<CaptureTimingOptions>()))
            .ReturnsAsync(expected)
            .Verifiable();

        var result = await service.Object.NavigateAndCaptureResourcesAsync(session, url, captureSpec, rewriteSpec, CancellationToken.None);

        Assert.Equal(expected, result);
        service.Verify();
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesAsync_ThrowsOnNullArguments()
    {
        var service = new Mock<IBrowserCaptureService>().Object;
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var captureSpec = new CaptureSpec(_ => true, async (req, resp) => null, null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => BrowserCaptureServiceExtensions.NavigateAndCaptureResourcesAsync(service, session, url, captureSpec: null!, CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_OverloadWithContentTypes_InvokesService()
    {
        // Arrange
        var service = new Mock<IBrowserCaptureService>(MockBehavior.Strict);
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");
        var contentTypes = new[] { "application/json", "video/mp4" };
        var expected = new List<CapturedResource>();
        service.Setup(s => s.NavigateAndCaptureResourcesByContentTypeAsync(
            session,
            It.IsAny<NavigationOptions>(),
            contentTypes,
            It.IsAny<CancellationToken>(),
            It.IsAny<RewriteSpec?>(),
            null,
            It.IsAny<CaptureTimingOptions>()))
            .ReturnsAsync(expected)
            .Verifiable();

        // Act
        var result = await service.Object.NavigateAndCaptureResourcesByContentTypeAsync(
            session,
            url,
            contentTypes,
            CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
        service.Verify();
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnNullService()
    {
        var session = new Mock<IBrowserSession>().Object;
        var url = new Uri("https://example.com");

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrowserCaptureServiceExtensions.NavigateAndCaptureResourcesByContentTypeAsync(
                null!, session, url, ["application/json"], CancellationToken.None));
    }

    [Fact]
    public async Task NavigateAndCaptureResourcesByContentTypeAsync_ThrowsOnNullUrl()
    {
        var service = new Mock<IBrowserCaptureService>().Object;
        var session = new Mock<IBrowserSession>().Object;

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrowserCaptureServiceExtensions.NavigateAndCaptureResourcesByContentTypeAsync(
                service, session, null!, ["application/json"], CancellationToken.None));
    }
}
