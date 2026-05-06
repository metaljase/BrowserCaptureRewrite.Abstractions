using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Helpers;

public class ContentTypeHelperTests
{
    [Theory]
    // Unparameterised candidate: matches response with no parameters
    [InlineData("application/json", new[] { "application/json" }, true)]
    // Unparameterised candidate: wildcard — matches response that has parameters
    [InlineData("application/json; charset=utf-8", new[] { "application/json" }, true)]
    // Unparameterised candidate: wildcard — matches response with multiple parameters
    [InlineData("application/json; charset=utf-8; boundary=something", new[] { "application/json" }, true)]
    // Parameterised candidate: matches response with exact same parameter (case-insensitive)
    [InlineData("application/json; charset=utf-8", new[] { "application/json; charset=utf-8" }, true)]
    [InlineData("application/json; charset=UTF-8", new[] { "application/json; charset=utf-8" }, true)]
    // Parameterised candidate: subset match — response has extra parameters beyond what candidate specifies
    [InlineData("application/json; boundary=something; charset=utf-8", new[] { "application/json; charset=utf-8" }, true)]
    // Parameterised candidate: order-insensitive — candidate parameters in different order to response
    [InlineData("application/json; boundary=something; charset=utf-8", new[] { "application/json; charset=utf-8; boundary=something" }, true)]
    // Parameterised candidate: no match — response missing the required parameter
    [InlineData("application/json", new[] { "application/json; charset=utf-8" }, false)]
    // Parameterised candidate: no match — parameter value differs
    [InlineData("application/json; charset=utf-16", new[] { "application/json; charset=utf-8" }, false)]
    // Multiple candidates: first doesn't match but second does
    [InlineData("VIDEO/MP4", new[] { "text/html", "video/mp4" }, true)]
    // Wrong media type
    [InlineData("application/json", new[] { "text/html" }, false)]
    // Empty/null response
    [InlineData("", new[] { "text/html" }, false)]
    [InlineData(null, new[] { "text/html" }, false)]
    // Empty candidate list
    [InlineData("text/html", new string[0], false)]
    public void HasMatchingContentType_CandidateSet_EvaluatesMatchResult(
        string? responseContentType, string[] candidates, bool expected)
    {
        // Arrange
        var normalized = ContentTypeHelper.NormalizeContentTypes(candidates);

        // Act
        var result = ContentTypeHelper.HasMatchingContentType(responseContentType, normalized);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NormalizeContentTypes_Null_ReturnsEmpty()
    {
        // Act
        var result = ContentTypeHelper.NormalizeContentTypes(null!);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void NormalizeContentTypes_MixedCasingParametersAndDuplicates_ReturnsNormalizedDistinctSet()
    {
        // Act
        var result = ContentTypeHelper.NormalizeContentTypes(
            "APPLICATION/JSON; charset=utf-8", "video/mp4", "  text/html  ", "application/json; charset=utf-8");

        // Assert
        Assert.Equal(["application/json; charset=utf-8", "video/mp4", "text/html"], result);
    }

    [Fact]
    public void NormalizeContentTypes_ParametersAreSortedAlphabetically()
    {
        // Act
        var result = ContentTypeHelper.NormalizeContentTypes("application/json; boundary=something; charset=utf-8");

        // Assert
        Assert.Equal(["application/json; boundary=something; charset=utf-8"], result);
    }

    [Fact]
    public void NormalizeContentTypes_BlanksOnly_ReturnsEmpty()
    {
        // Act
        var result = ContentTypeHelper.NormalizeContentTypes("", "  ", null!);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("text/plain", true)]
    [InlineData("text/xml", true)]
    [InlineData("text/css", true)]
    // JSON types
    [InlineData("application/json", true)]
    [InlineData("application/vnd.api+json", true)]
    // XML types
    [InlineData("application/xml", true)]
    [InlineData("application/soap+xml", true)]
    [InlineData("application/atom+xml", true)]
    // Script types
    [InlineData("application/javascript", true)]
    [InlineData("text/javascript", true)]
    // Form types
    [InlineData("application/x-www-form-urlencoded", true)]
    // GraphQL
    [InlineData("application/graphql", true)]
    // Content-type with parameters should match on the MIME type, not the parameters
    [InlineData("text/html; charset=utf-8", true)]
    [InlineData("application/json; charset=utf-8", true)]
    // Binary types
    [InlineData("application/octet-stream", false)]
    [InlineData("image/png", false)]
    [InlineData("audio/mpeg", false)]
    [InlineData("video/mp4", false)]
    // Office OpenXML formats should not be treated as text despite containing "xml"
    [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", false)]
    [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", false)]
    [InlineData("application/vnd.openxmlformats-officedocument.presentationml.presentation", false)]
    // Boundary containing "xml" keyword should not be treated as text
    [InlineData("multipart/form-data; boundary=xmlseparator", false)]
    // Edge cases
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsTextBasedContentType_ReturnsExpectedResult(string? contentType, bool expectedResult)
    {
        // Act
        var result = ContentTypeHelper.IsTextBasedContentType(contentType);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
