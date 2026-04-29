using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Connectivity;

public class ConnectivityClassificationResultTests
{
    [Fact]
    public void NotConnectivityRelated_HasExpectedValues()
    {
        var result = ConnectivityClassificationResult.NotConnectivityRelated;

        Assert.False(result.IsConnectivityRelated);
        Assert.Equal(ConnectivityScope.Unknown, result.Scope);
    }

    [Fact]
    public void ConnectivityRelated_Factory_SetsExpectedValues()
    {
        var scope = ConnectivityScope.RemoteSite;
        var result = ConnectivityClassificationResult.ConnectivityRelated(scope);

        Assert.True(result.IsConnectivityRelated);
        Assert.Equal(scope, result.Scope);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var a = new ConnectivityClassificationResult(true, ConnectivityScope.LocalEnvironment);
        var b = new ConnectivityClassificationResult(true, ConnectivityScope.LocalEnvironment);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Record_Deconstruction_Works()
    {
        var result = new ConnectivityClassificationResult(true, ConnectivityScope.RemoteSite);
        var (isRelated, scope) = result;

        Assert.True(isRelated);
        Assert.Equal(ConnectivityScope.RemoteSite, scope);
    }
}
