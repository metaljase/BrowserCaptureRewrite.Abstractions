using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class CaptureSpecTests
{
    [Fact]
    public void Constructor_ThrowsOnNullArguments()
    {
        static bool shouldCapture(IRequestInfo _) => true;
        static Task<CapturedResource?> tryCreate(IRequestInfo req, IResponseInfo resp) => Task.FromResult<CapturedResource?>(null);
        Assert.Throws<ArgumentNullException>(() => new CaptureSpec((Func<IRequestInfo, bool>)null!, tryCreate, shouldCompleteCapture: null));
        Assert.Throws<ArgumentNullException>(() => new CaptureSpec(shouldCapture, null!, shouldCompleteCapture: null));
    }

    [Fact]
    public void Properties_AreAssignedAndBehaveAsExpected()
    {
        static bool shouldCapture(IRequestInfo req) => req.Url.Contains("capture");
        static Task<CapturedResource?> tryCreate(IRequestInfo req, IResponseInfo resp) => Task.FromResult<CapturedResource?>(null);
        static bool completion(NavigationOptions nav, IReadOnlyList<CapturedResource> list, DateTime dt) => true;
        var spec = new CaptureSpec(shouldCapture, tryCreate, completion);

        var reqCapture = new FakeRequestInfo("https://capture");
        var reqSkip = new FakeRequestInfo("https://skip");
        Assert.True(spec.ShouldCapture(reqCapture));
        Assert.False(spec.ShouldCapture(reqSkip));
        Assert.Equal(tryCreate, spec.TryCreateCapturedResourceAsync);
        Assert.Equal(completion, spec.ShouldCompleteCapture);
    }

    [Fact]
    public void Properties_DefaultCompletionCondition_IsNull()
    {
        static Task<CapturedResource?> tryCreate(IRequestInfo req, IResponseInfo resp) => Task.FromResult<CapturedResource?>(null);

        var spec = new CaptureSpec(_ => true, tryCreate, shouldCompleteCapture: null);

        Assert.Null(spec.ShouldCompleteCapture);
    }

    [Fact]
    public async Task TryCreateCapturedResourceAsync_InvokesDelegate()
    {
        var called = false;
        Task<CapturedResource?> tryCreate(IRequestInfo req, IResponseInfo resp)
        {
            called = true;
            return Task.FromResult<CapturedResource?>(null);
        }
        var spec = new CaptureSpec(_ => true, tryCreate, shouldCompleteCapture: null);
        await spec.TryCreateCapturedResourceAsync(new FakeRequestInfo("https://a"), new FakeResponseInfo());
        Assert.True(called);
    }

    private class FakeRequestInfo(string url) : IRequestInfo
    {
        public string Url { get; } = url;
        public string Method { get; } = "GET";
        public IReadOnlyDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    }

    private class FakeResponseInfo : IResponseInfo
    {
        public int? StatusCode => 200;
        public IReadOnlyDictionary<string, string> Headers => new Dictionary<string, string>();
        public Task<string> GetBodyAsStringAsync() => Task.FromResult("");
        public Task<byte[]> GetBodyAsByteArrayAsync() => Task.FromResult(Array.Empty<byte>());
    }
}
