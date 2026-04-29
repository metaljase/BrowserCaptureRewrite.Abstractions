using System.Net;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for evaluating HTTP request and response status codes for retry behaviour.
/// </summary>
/// <remarks>
/// <para>
/// All methods are <see langword="static"/> and intended for use in resilience and retry policy logic across browser
/// automation and HTTP capture scenarios.
/// </para>
/// <para>
/// Implements logic to determine whether an HTTP request exception or status code is considered transient and should be retried.
/// </para>
/// </remarks>
public static class HttpHelper
{
    /// <summary>
    /// Determines whether the specified <see cref="HttpRequestException"/> is considered retryable based on its status code.
    /// </summary>
    /// <param name="ex">
    /// The <see cref="HttpRequestException"/> to evaluate.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the exception has a <see cref="HttpRequestException.StatusCode"/> of
    /// <see langword="null"/>, or if the status code is considered retryable; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <paramref name="ex"/> has a <see langword="null"/> <see cref="HttpRequestException.StatusCode"/>, the
    /// exception is treated as a transient transport error and is considered retryable.
    /// </para>
    /// <para>
    /// Otherwise, the retryability is determined by <see cref="IsRetryableHttpRequestException(HttpStatusCode)"/>.
    /// </para>
    /// </remarks>
    public static bool IsRetryableHttpRequestException(HttpRequestException ex)
    {
        // If a transport error occurs, StatusCode may be null; treat it as transient.
        return ex.StatusCode is null || IsRetryableHttpRequestException(ex.StatusCode.Value);
    }

    /// <summary>
    /// Determines whether the specified <see cref="HttpStatusCode"/> is considered retryable.
    /// </summary>
    /// <param name="statusCode">
    /// The HTTP status code to evaluate.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="statusCode"/> is 408 (Request Timeout), 429 (Too Many Requests),
    /// or any 5xx (server error) code; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Retryable status codes are:
    /// <list type="bullet">
    ///   <item><description>408 (Request Timeout)</description></item>
    ///   <item><description>429 (Too Many Requests)</description></item>
    ///   <item><description>Any 5xx (server error) status code</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public static bool IsRetryableHttpRequestException(HttpStatusCode statusCode)
    {
        var code = (int)statusCode;
        return code is 408 or 429 or >= 500;
    }
}
