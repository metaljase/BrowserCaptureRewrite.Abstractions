using System.Net;

using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a sign-in operation fails during browser automation.
/// </summary>
/// <remarks>
/// <para>
/// The exception message is automatically constructed from the sign-in URL and status code if no custom message is provided.
/// </para>
/// </remarks>
/// <param name="signInUrl">The URL that was used for the sign-in attempt.  Must not be <see langword="null"/>.</param>
/// <param name="statusCode">An optional HTTP status code returned by the sign-in attempt.</param>
/// <param name="message">An optional exception message.  Defaults to a message constructed from <paramref name="signInUrl"/> and
/// <paramref name="statusCode"/> if <see langword="null"/>.</param>
/// <param name="innerException">An optional inner exception.</param>
public sealed class SignInException(
    Uri signInUrl, int? statusCode = null, string? message = null, Exception? innerException = null)
    : Exception(message ?? BuildMessage(signInUrl, statusCode), innerException)
{
    /// <summary>
    /// Gets the URL that was used for the sign-in attempt.
    /// </summary>
    public Uri SignInUrl { get; } = signInUrl;

    /// <summary>
    /// Gets the HTTP status code returned by the sign-in attempt, if available.
    /// </summary>
    public int? StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the HTTP status code as a <see cref="System.Net.HttpStatusCode"/>, if available.
    /// </summary>
    public HttpStatusCode? HttpStatusCode => StatusCode.HasValue ? (HttpStatusCode)StatusCode.Value : null;

    /// <summary>
    /// Gets a human-readable description of the HTTP status code, if available.
    /// </summary>
    public string? Status => HttpStatusCode.HasValue ? HumanizeHelper.HumanizeEnum(HttpStatusCode.Value) : null;

    private static string BuildMessage(Uri signInUrl, int? statusCode)
    {
        var statusText = statusCode.HasValue ? $" {HumanizeHelper.HumanizeEnum((HttpStatusCode)statusCode.Value)}" : string.Empty;
        var reason = statusCode.HasValue ? $" with HTTP {statusCode}{statusText}" : string.Empty;
        return $"Sign-in failed{reason} for URL: {signInUrl}.";
    }
}