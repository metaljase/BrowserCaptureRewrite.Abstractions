using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Models;

public class CaptureCompletionResultTests
{
    [Fact]
    public void Constructor_AssignsStatusCorrectly()
    {
        var result = new CaptureCompletionResult(CaptureStatus.CriteriaSatisfied);
        Assert.Equal(CaptureStatus.CriteriaSatisfied, result.Status);
    }

    [Theory]
    [InlineData(CaptureStatus.CriteriaNotSatisfied, false)]
    [InlineData(CaptureStatus.CriteriaSatisfied, true)]
    [InlineData(CaptureStatus.CaptureTimeoutExceeded, true)]
    [InlineData(CaptureStatus.UrlChangedBeforeCompletion, true)]
    public void IsComplete_ReturnsExpected(CaptureStatus status, bool expected)
    {
        var result = new CaptureCompletionResult(status);
        Assert.Equal(expected, result.IsComplete);
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        var a = new CaptureCompletionResult(CaptureStatus.CriteriaSatisfied);
        var b = new CaptureCompletionResult(CaptureStatus.CriteriaSatisfied);
        Assert.Equal(a, b);
    }
}
