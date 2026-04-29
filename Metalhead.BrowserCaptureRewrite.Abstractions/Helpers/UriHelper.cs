namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for validating and parsing HTTP and HTTPS URIs.
/// </summary>
/// <remarks>
/// <para>
/// All methods are <see langword="static"/> and intended for use in scenarios where only absolute HTTP
/// or HTTPS URIs are valid.
/// </para>
/// <para>
/// Methods throw exceptions for invalid input and perform strict validation of scheme and absolute URI requirements.
/// </para>
/// </remarks>
public class UriHelper
{
    /// <summary>
    /// Determines whether the specified <see cref="Uri"/> is an absolute HTTP or HTTPS URI.
    /// </summary>
    /// <param name="uri">
    /// The URI to validate.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="uri"/> is absolute and uses the HTTP or HTTPS scheme;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method throws if <paramref name="uri"/> is <see langword="null"/>.  It does not throw for non-absolute
    /// or non-HTTP/HTTPS URIs, but returns <see langword="false"/>.
    /// </para>
    /// </remarks>
    public static bool IsValidHttpClientUri(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        return uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Parses a string as an absolute HTTP or HTTPS URI.
    /// </summary>
    /// <param name="url">
    /// The URL string to parse.  Must not be <see langword="null"/> or empty.
    /// </param>
    /// <returns>
    /// A <see cref="Uri"/> instance representing the absolute HTTP or HTTPS URL.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="url"/> is not a valid absolute URL, or does not use the HTTP or HTTPS scheme.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method throws if <paramref name="url"/> is <see langword="null"/>, empty, not an absolute URL,
    /// or not an HTTP/HTTPS URL.
    /// </para>
    /// </remarks>
    public static Uri ParseAbsoluteUrl(string url)
    {
        return !Uri.TryCreate(url, UriKind.Absolute, out var uri)
            ? throw new ArgumentException($"'{url}' is not a valid absolute URL.", nameof(url))
            : uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps
                ? throw new ArgumentException($"'{url}' is not an HTTP/HTTPS URL.", nameof(url))
                : uri;
    }
}