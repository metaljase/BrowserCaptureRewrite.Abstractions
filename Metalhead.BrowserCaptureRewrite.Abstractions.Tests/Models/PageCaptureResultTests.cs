using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class PageCaptureResultTests
{
    [Fact]
    public void Constructor_AssignsPropertiesCorrectly()
    {
        var responseHtml = "<html>responseHtml</html>";
        var renderedHtml = "<html>renderedHtml</html>";
        var resources = new List<CapturedResource> { new(new Uri("https://a"), "t", null, "ct", 200, null) };
        var status = CaptureStatus.CriteriaSatisfied;
        var result = new PageCaptureResult(responseHtml, renderedHtml, resources, null, status);
        Assert.Equal(responseHtml, result.ResponseHtml);
        Assert.Equal(renderedHtml, result.RenderedHtml);
        Assert.Equal(resources, result.Resources);
        Assert.Equal(status, result.CaptureStatus);
    }

    [Fact]
    public void Constructor_AllowsNulls()
    {
        var result = new PageCaptureResult(null, null, [], null, null);
        Assert.Null(result.ResponseHtml);
        Assert.Null(result.RenderedHtml);
        Assert.Empty(result.Resources);
        Assert.Null(result.CaptureStatus);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var resources = new List<CapturedResource> { new(new Uri("https://a"), "t", null, "ct", 200, null) };
        var a = new PageCaptureResult("i", "f", resources, null, CaptureStatus.CriteriaSatisfied);
        var b = new PageCaptureResult("i", "f", resources, null, CaptureStatus.CriteriaSatisfied);
        Assert.Equal(a, b);
    }
}
