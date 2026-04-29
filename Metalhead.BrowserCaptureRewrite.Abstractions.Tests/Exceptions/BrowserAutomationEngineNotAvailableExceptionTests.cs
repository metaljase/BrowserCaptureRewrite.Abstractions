using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Exceptions;

public class BrowserAutomationEngineNotAvailableExceptionTests
{
    [Fact]
    public void Constructor_SetsMessageAndInnerException_WithoutInstallHint()
    {
        var message = "Engine not available";
        var inner = new InvalidOperationException("Inner");
        var ex = new BrowserAutomationEngineNotAvailableException(message: message, innerException: inner);

        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Null(ex.ResolutionHint);
    }

    [Fact]
    public void Constructor_SetsInstallHint_WhenProvided()
    {
        var message = "Engine not available";
        var inner = new Exception("Inner");
        var hint = "Install Playwright via 'dotnet tool install playwright'";
        var ex = new BrowserAutomationEngineNotAvailableException(hint, message, inner);

        Assert.Equal(message, ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Equal(hint, ex.ResolutionHint);
    }
}