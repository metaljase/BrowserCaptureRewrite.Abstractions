using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class CapturedResourceTests
{
    [Fact]
    public void Constructor_AssignsPropertiesCorrectly()
    {
        var url = new Uri("https://example.com");
        var text = "hello";
        var binary = new byte[] { 1, 2, 3 };
        var contentType = "text/plain";
        int? statusCode = 200;
        var headers = new Dictionary<string, string> { ["content-type"] = contentType };

        var resource = new CapturedResource(url, text, binary, contentType, statusCode, headers);

        Assert.Equal(url, resource.Url);
        Assert.Equal(text, resource.TextContent);
        Assert.Equal(binary, resource.BinaryContent);
        Assert.Equal(contentType, resource.ContentType);
        Assert.Equal(statusCode, resource.StatusCode);
        Assert.Equal(headers, resource.ResponseHeaders);
    }

    [Fact]
    public void HasText_True_WhenTextContentIsNotNull()
    {
        var resource = new CapturedResource(new Uri("https://a"), "abc", null, null, null, null);
        Assert.True(resource.HasText);
    }

    [Fact]
    public void HasText_False_WhenTextContentIsNull()
    {
        var resource = new CapturedResource(new Uri("https://a"), null, [1], null, null, null);
        Assert.False(resource.HasText);
    }

    [Fact]
    public void HasBinary_True_WhenBinaryContentIsNotNull()
    {
        var resource = new CapturedResource(new Uri("https://a"), null, [1], null, null, null);
        Assert.True(resource.HasBinary);
    }

    [Fact]
    public void HasBinary_False_WhenBinaryContentIsNull()
    {
        var resource = new CapturedResource(new Uri("https://a"), "abc", null, null, null, null);
        Assert.False(resource.HasBinary);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var url = new Uri("https://eq");
        var bytes = new byte[] { 1 };
        var a = new CapturedResource(url, "t", bytes, "ct", 200, null);
        var b = new CapturedResource(url, "t", bytes, "ct", 200, null);
        Assert.Equal(a, b);
    }
}
