using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Tests.Helpers;

public class FileExtensionHelperTests
{
    [Fact]
    public void NormalizeFileExtensions_Null_ReturnsEmpty()
    {
        var result = FileExtensionHelper.NormalizeFileExtensions(null!);
        Assert.Empty(result);
    }

    [Fact]
    public void NormalizeFileExtensions_MixedWhitespaceCasingAndMissingDots_ReturnsNormalizedDistinctSet()
    {
        var result = FileExtensionHelper.NormalizeFileExtensions(" M3U8 ", ".Js", "css", "m3u8");
        Assert.Equal([".m3u8", ".js", ".css"], result);
    }

    [Fact]
    public void NormalizeFileExtensions_BlanksAndDuplicates_IgnoredAndDistinctReturned()
    {
        var result = FileExtensionHelper.NormalizeFileExtensions(".mp4", "", " .MP4 ", " .Ts ");
        Assert.Equal([".mp4", ".ts"], result);
    }

    [Fact]
    public void HasMatchingFileExtension_EmptyExtensionList_ReturnsFalse()
    {
        Assert.False(FileExtensionHelper.HasMatchingFileExtension("/video/segment.ts", []));
    }

    [Fact]
    public void HasMatchingFileExtension_EmptyPath_ReturnsFalse()
    {
        var exts = FileExtensionHelper.NormalizeFileExtensions(".ts");
        Assert.False(FileExtensionHelper.HasMatchingFileExtension(string.Empty, exts));
    }

    [Theory]
    [InlineData("/video/master.m3u8", new[] { ".m3u8" }, true)]
    [InlineData("/video/master.M3U8", new[] { ".m3u8" }, true)]
    [InlineData("/video/file.ts", new[] { ".m3u8" }, false)]
    [InlineData("/video/file.min.js", new[] { ".min.js" }, true)]
    [InlineData("/video/file.min.js", new[] { ".js" }, true)]
    [InlineData("/video/file.min.js", new[] { ".css" }, false)]
    [InlineData("/video/archive.tar.gz", new[] { ".tar.gz" }, true)]
    [InlineData("/video/archive.tar.gz", new[] { ".gz" }, true)]
    public void HasMatchingFileExtension_CandidateSet_EvaluatesMatchResult(string path, string[] candidates, bool expected)
    {
        var normalized = FileExtensionHelper.NormalizeFileExtensions(candidates);
        Assert.Equal(expected, FileExtensionHelper.HasMatchingFileExtension(path, normalized));
    }

    [Fact]
    public void HasMatchingFileExtension_MultipleCandidateExtensions_EvaluatesMixedMatches()
    {
        var normalized = FileExtensionHelper.NormalizeFileExtensions(".m3u8", " .ts ", "MP4");
        Assert.True(FileExtensionHelper.HasMatchingFileExtension("/content/seg1.ts", normalized));
        Assert.True(FileExtensionHelper.HasMatchingFileExtension("/content/video.MP4", normalized));
        Assert.True(FileExtensionHelper.HasMatchingFileExtension("/content/playlist.m3u8", normalized));
        Assert.False(FileExtensionHelper.HasMatchingFileExtension("/content/image.png", normalized));
    }
}
