using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Exceptions;

public class PageCaptureIncompleteExceptionTests
{
    [Fact]
    public void Constructor_SetsPropertiesAndDefaultMessage()
    {
        var status = CaptureStatus.CaptureTimeoutExceeded;
        var url = new Uri("https://example.com/test");
        var ex = new PageCaptureIncompleteException(status, url);

        Assert.Equal(status, ex.CaptureStatus);
        Assert.Null(ex.PageLoadStatus);
        Assert.Equal(url, ex.Url);
        Assert.Equal($"Page capture incomplete: {status} for {url}", ex.Message);
    }

    [Fact]
    public void Constructor_NullUrl_DefaultMessageOmitsUrl()
    {
        var status = CaptureStatus.CriteriaNotSatisfied;
        var ex = new PageCaptureIncompleteException(status, null);

        Assert.Equal(status, ex.CaptureStatus);
        Assert.Null(ex.PageLoadStatus);
        Assert.Null(ex.Url);
        Assert.Equal($"Page capture incomplete: {status}", ex.Message);
    }

    [Fact]
    public void Constructor_CustomMessage_OverridesDefault()
    {
        var status = CaptureStatus.UrlChangedBeforeCompletion;
        var url = new Uri("https://example.com/other");
        var customMsg = "Custom error message.";
        var ex = new PageCaptureIncompleteException(status, url, customMsg);

        Assert.Equal(status, ex.CaptureStatus);
        Assert.Null(ex.PageLoadStatus);
        Assert.Equal(url, ex.Url);
        Assert.Equal(customMsg, ex.Message);
    }

    [Fact]
    public void Constructor_PageLoadStatus_SetsPropertiesAndDefaultMessage()
    {
        var status = PageLoadStatus.NetworkIdleTimeoutExceeded;
        var url = new Uri("https://example.com/test");
        var ex = new PageCaptureIncompleteException(status, url);

        Assert.Equal(status, ex.PageLoadStatus);
        Assert.Null(ex.CaptureStatus);
        Assert.Equal(url, ex.Url);
        Assert.Equal($"Page capture incomplete: {status} for {url}", ex.Message);
    }

    [Fact]
    public void Exception_CanBeCaughtAsException()
    {
        var status = CaptureStatus.CriteriaNotSatisfied;
        var url = new Uri("https://example.com/err");
        try
        {
            throw new PageCaptureIncompleteException(status, url);
        }
        catch (Exception ex)
        {
            Assert.IsType<PageCaptureIncompleteException>(ex);
        }
    }
}
