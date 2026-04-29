using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Transport;

public class ResponseInfoExtensionsTests
{
    private class FakeResponseInfo : IResponseInfo
    {
        public int? StatusCode { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Func<Task<string>>? GetBodyAsStringAsyncImpl { get; set; }
        public Func<Task<byte[]>>? GetBodyAsByteArrayAsyncImpl { get; set; }

        public Task<string> GetBodyAsStringAsync() =>
            GetBodyAsStringAsyncImpl != null ? GetBodyAsStringAsyncImpl() : Task.FromResult("");
        public Task<byte[]> GetBodyAsByteArrayAsync() =>
            GetBodyAsByteArrayAsyncImpl != null ? GetBodyAsByteArrayAsyncImpl() : Task.FromResult(Array.Empty<byte>());
    }

    [Fact]
    public async Task TryGetBodyAsByteArrayAsync_ReturnsBytesOnSuccess()
    {
        var expected = new byte[] { 1, 2, 3 };
        var response = new FakeResponseInfo
        {
            GetBodyAsByteArrayAsyncImpl = () => Task.FromResult(expected)
        };
        var result = await response.TryGetBodyAsByteArrayAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TryGetBodyAsByteArrayAsync_ReturnsNullOnException()
    {
        var response = new FakeResponseInfo
        {
            GetBodyAsByteArrayAsyncImpl = () => throw new Exception("fail")
        };
        var result = await response.TryGetBodyAsByteArrayAsync();
        Assert.Null(result);
    }

    [Fact]
    public async Task TryGetBodyAsStringAsync_ReturnsStringOnSuccess()
    {
        var expected = "hello world";
        var response = new FakeResponseInfo
        {
            GetBodyAsStringAsyncImpl = () => Task.FromResult(expected)
        };
        var result = await response.TryGetBodyAsStringAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TryGetBodyAsStringAsync_ReturnsNullOnException()
    {
        var response = new FakeResponseInfo
        {
            GetBodyAsStringAsyncImpl = () => throw new Exception("fail")
        };
        var result = await response.TryGetBodyAsStringAsync();
        Assert.Null(result);
    }
}
