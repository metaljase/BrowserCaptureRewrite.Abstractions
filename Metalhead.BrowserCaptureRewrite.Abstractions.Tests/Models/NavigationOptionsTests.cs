using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class NavigationOptionsTests
{
    [Fact]
    public void Constructor_AssignsPropertiesCorrectly_AllArguments()
    {
        var url = new Uri("https://example.com");
        var referer = new Uri("https://referer.com");
        var timeout = TimeSpan.FromSeconds(5);
        var opts = new NavigationOptions(url, referer, timeout);
        Assert.Equal(url, opts.Url);
        Assert.Equal(referer, opts.RefererUrl);
        Assert.Equal(timeout, opts.PageLoadTimeout);
    }

    [Fact]
    public void Constructor_AssignsPropertiesCorrectly_OnlyUrl()
    {
        var url = new Uri("https://example.com");
        var opts = new NavigationOptions(url);
        Assert.Equal(url, opts.Url);
        Assert.Null(opts.RefererUrl);
        Assert.Null(opts.PageLoadTimeout);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var url = new Uri("https://eq");
        var referer = new Uri("https://ref");
        var timeout = TimeSpan.FromSeconds(1);
        var a = new NavigationOptions(url, referer, timeout);
        var b = new NavigationOptions(url, referer, timeout);
        Assert.Equal(a, b);
    }
}
