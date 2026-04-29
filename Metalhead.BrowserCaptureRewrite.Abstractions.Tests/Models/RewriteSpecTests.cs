using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class RewriteSpecTests
{
    [Fact]
    public void Constructor_ThrowsOnNullArguments()
    {
        static bool shouldRewrite(IRequestInfo _) => true;
        static Task<ResponseRewriteResult> tryRewrite(IRequestInfo req, IResponseInfo resp) => Task.FromResult(ResponseRewriteResult.NotRewritten);

        Assert.Throws<ArgumentNullException>(() => new RewriteSpec((Func<IRequestInfo, bool>)null!, tryRewrite));
        Assert.Throws<ArgumentNullException>(() => new RewriteSpec(shouldRewrite, null!));
    }

    [Fact]
    public void Properties_AreAssignedAndBehaveAsExpected()
    {
        static bool shouldRewrite(IRequestInfo req) => req.Url.Contains("rewrite");
        static Task<ResponseRewriteResult> tryRewrite(IRequestInfo req, IResponseInfo resp) =>
            Task.FromResult(new ResponseRewriteResult(true, "body", "ct"));

        var spec = new RewriteSpec(shouldRewrite, tryRewrite);
        var reqRewrite = new FakeRequestInfo("https://rewrite");
        var reqSkip = new FakeRequestInfo("https://skip");

        Assert.True(spec.ShouldRewrite(reqRewrite));
        Assert.False(spec.ShouldRewrite(reqSkip));
        Assert.Equal(tryRewrite, spec.TryRewriteResponseAsync);
    }

    private sealed class FakeRequestInfo(string url) : IRequestInfo
    {
        public string Url { get; } = url;
        public string Method { get; } = "GET";
        public IReadOnlyDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    }
}
