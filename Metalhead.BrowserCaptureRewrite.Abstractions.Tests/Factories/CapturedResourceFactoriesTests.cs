using System.Text;

using Metalhead.BrowserCaptureRewrite.Abstractions.Factories;
using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Factories;

public class CapturedResourceFactoriesTests
{
    private static readonly Uri s_testUri = new("https://example.com/resource");

    [Fact]
    public async Task TextFactory_ReturnsTextContent()
    {
        var req = new FakeRequestInfo(s_testUri.ToString());
        var resp = new FakeResponseInfo("text/plain", 200, "hello world");
        var factory = CapturedResourceFactories.Text();
        var result = await factory(req, resp);
        Assert.NotNull(result);
        Assert.Equal(s_testUri, result!.Url);
        Assert.Equal("hello world", result.TextContent);
        Assert.Null(result.BinaryContent);
        Assert.Equal("text/plain", result.ContentType);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task BinaryFactory_ReturnsBinaryContent()
    {
        var req = new FakeRequestInfo(s_testUri.ToString());
        var bytes = Encoding.UTF8.GetBytes("bin-data");
        var resp = new FakeResponseInfo("application/octet-stream", 201, bytes);
        var factory = CapturedResourceFactories.Binary();
        var result = await factory(req, resp);
        Assert.NotNull(result);
        Assert.Equal(s_testUri, result!.Url);
        Assert.Null(result.TextContent);
        Assert.Equal(bytes, result.BinaryContent);
        Assert.Equal("application/octet-stream", result.ContentType);
        Assert.Equal(201, result.StatusCode);
    }

    [Theory]
    [InlineData("text/html", "<html></html>", null)]
    [InlineData("application/json", "{\"a\":1}", null)]
    [InlineData("application/xml", "<x></x>", null)]
    [InlineData("application/octet-stream", null, "bin")]
    [InlineData(null, null, "bin")]
    public async Task AutoFactory_ChoosesCorrectContentType(string? contentType, string? text, string? binary)
    {
        var req = new FakeRequestInfo(s_testUri.ToString());
        IResponseInfo resp = text != null
            ? new FakeResponseInfo(contentType, 200, text)
            : new FakeResponseInfo(contentType, 200, Encoding.UTF8.GetBytes(binary!));
        var factory = CapturedResourceFactories.Auto();
        var result = await factory(req, resp);
        Assert.NotNull(result);
        Assert.Equal(s_testUri, result!.Url);
        if (text != null)
        {
            Assert.Equal(text, result.TextContent);
            Assert.Null(result.BinaryContent);
        }
        else
        {
            Assert.Null(result.TextContent);
            Assert.Equal(Encoding.UTF8.GetBytes(binary!), result.BinaryContent);
        }
        Assert.Equal(contentType, result.ContentType);
    }

    private class FakeRequestInfo(string url, string method = "GET", IReadOnlyDictionary<string, string>? headers = null) : IRequestInfo
    {
        public string Url { get; } = url;
        public string Method { get; } = method;
        public IReadOnlyDictionary<string, string> Headers { get; } = headers ?? new Dictionary<string, string>();
    }

    private class FakeResponseInfo : IResponseInfo
    {
        public FakeResponseInfo(string? contentType, int? statusCode, string text)
        {
            Headers = contentType != null ? new Dictionary<string, string> { ["content-type"] = contentType } : [];
            StatusCode = statusCode;
            _text = text;
            _bytes = Encoding.UTF8.GetBytes(text);
        }

        public FakeResponseInfo(string? contentType, int? statusCode, byte[] bytes)
        {
            Headers = contentType != null ? new Dictionary<string, string> { ["content-type"] = contentType } : [];
            StatusCode = statusCode;
            _text = null;
            _bytes = bytes;
        }

        public int? StatusCode { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        private readonly string? _text;
        private readonly byte[] _bytes;
        public Task<string> GetBodyAsStringAsync() => Task.FromResult(_text ?? "");
        public Task<byte[]> GetBodyAsByteArrayAsync() => Task.FromResult(_bytes);
    }
}
