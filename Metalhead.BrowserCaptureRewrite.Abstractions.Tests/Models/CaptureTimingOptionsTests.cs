using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class CaptureTimingOptionsTests
{
    [Fact]
    public void Constructor_AssignsPropertiesCorrectly()
    {
        var idle = TimeSpan.FromSeconds(1);
        var capture = TimeSpan.FromSeconds(2);
        var poll = TimeSpan.FromMilliseconds(500);

        var opts = new CaptureTimingOptions(idle, capture, poll);

        Assert.Equal(idle, opts.NetworkIdleTimeout());
        Assert.Equal(capture, opts.CaptureTimeout());
        Assert.Equal(poll, opts.PollInterval());
    }

    [Fact]
    public void DefaultPollInterval_HasExpectedValue()
    {
        Assert.Equal(TimeSpan.FromMilliseconds(250), CaptureTimingOptions.DefaultPollInterval);
    }

    [Fact]
    public void PollInterval_WhenPollIntervalMillisecondsIsNull_ReturnsDefaultPollInterval()
    {
        var opts = new CaptureTimingOptions();

        Assert.Equal(CaptureTimingOptions.DefaultPollInterval, opts.PollInterval());
    }

    [Fact]
    public void NetworkIdleTimeout_WhenNetworkIdleTimeoutSecondsIsNull_ReturnsNull()
    {
        var opts = new CaptureTimingOptions();

        Assert.Null(opts.NetworkIdleTimeout());
    }

    [Fact]
    public void CaptureTimeout_WhenCaptureTimeoutSecondsIsNull_ReturnsNull()
    {
        var opts = new CaptureTimingOptions();

        Assert.Null(opts.CaptureTimeout());
    }
}
