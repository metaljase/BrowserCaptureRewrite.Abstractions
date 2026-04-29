namespace Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

/// <summary>
/// Provides extension methods for safely retrieving HTTP response bodies from <see cref="IResponseInfo"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// These methods attempt to retrieve the response body as a byte array or string, returning <see langword="null"/> if an
/// exception occurs during retrieval.
/// </para>
/// <para>
/// Intended for scenarios where response body access may fail due to network errors, disposal, or other transient issues, and
/// where a <see langword="null"/> result is preferable to an exception.
/// </para>
/// </remarks>
public static class ResponseInfoExtensions
{
    /// <summary>
    /// Attempts to asynchronously retrieve the response body as a byte array, returning <see langword="null"/> if retrieval fails.
    /// </summary>
    /// <param name="response">
    /// The <see cref="IResponseInfo"/> instance to retrieve the body from.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  The task result is the response body as a byte array, or
    /// <see langword="null"/> if retrieval fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <paramref name="response"/> is <see langword="null"/>, a <see cref="NullReferenceException"/> may be thrown.
    /// Any exceptions thrown by <see cref="IResponseInfo.GetBodyAsByteArrayAsync"/> are caught and suppressed, returning
    /// <see langword="null"/>.
    /// </para>
    /// </remarks>
    public static async Task<byte[]?> TryGetBodyAsByteArrayAsync(this IResponseInfo response)
    {
        try { return await response.GetBodyAsByteArrayAsync().ConfigureAwait(false); }
        catch { return null; }
    }

    /// <summary>
    /// Attempts to asynchronously retrieve the response body as a string, returning <see langword="null"/> if retrieval fails.
    /// </summary>
    /// <param name="response">
    /// The <see cref="IResponseInfo"/> instance to retrieve the body from.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  The task result is the response body as a string, or
    /// <see langword="null"/> if retrieval fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <paramref name="response"/> is <see langword="null"/>, a <see cref="NullReferenceException"/> may be thrown.
    /// Any exceptions thrown by <see cref="IResponseInfo.GetBodyAsStringAsync"/> are caught and suppressed, returning
    /// <see langword="null"/>.
    /// </para>
    /// </remarks>
    public static async Task<string?> TryGetBodyAsStringAsync(this IResponseInfo response)
    {
        try { return await response.GetBodyAsStringAsync().ConfigureAwait(false); }
        catch { return null; }
    }
}