using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Helpers;

public class ContentTypeHelperTests
{
    [Theory]
    // Standard text types
    [InlineData("text/html", true)]
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
