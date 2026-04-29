using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;

using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Provides helper methods for classifying, annotating, and handling connectivity-related exceptions.
/// </summary>
/// <remarks>
/// <para>
/// Classification delegates to an <see cref="IConnectivityClassifier"/> to determine whether an exception is
/// connectivity-related, then uses an <see cref="IConnectivityProbe"/> where needed to resolve an ambiguous or
/// <see cref="ConnectivityScope.HostnameResolution"/> scope to <see cref="ConnectivityScope.LocalEnvironment"/>
/// or a more specific scope.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations
/// throw <see cref="OperationCanceledException"/>.
/// </para>
/// </remarks>
public static class ConnectivityExceptionHelper
{
    /// <summary>
    /// Determines the connectivity scope of the specified exception using the provided classifier and probe.
    /// </summary>
    /// <param name="ex">The exception to classify.  Must not be <see langword="null"/>.</param>
    /// <param name="classifier">The connectivity classifier to use.  Must not be <see langword="null"/>.</param>
    /// <param name="probe">The connectivity probe to use for additional checks.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// The determined <see cref="ConnectivityScope"/> for the exception.  Returns <see cref="ConnectivityScope.Unknown"/>
    /// when the exception is not classified as connectivity-related.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ex"/>, <paramref name="classifier"/>, or
    /// <paramref name="probe"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<ConnectivityScope> GetConnectivityScopeAsync(
        Exception ex, IConnectivityClassifier classifier, IConnectivityProbe probe, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ex);
        ArgumentNullException.ThrowIfNull(classifier);
        ArgumentNullException.ThrowIfNull(probe);

        var classification = classifier.ClassifyException(ex, cancellationToken);

        if (!classification.IsConnectivityRelated)
            return ConnectivityScope.Unknown;

        var scope = classification.Scope;
        if (scope is ConnectivityScope.Unknown or ConnectivityScope.HostnameResolution)
        {
            var up = await probe.HasGeneralConnectivityAsync(cancellationToken).ConfigureAwait(false);
            // If the scope has been classified as HostnameResolution (e.g. DNS issue), BUT there is a general connectivity issue too,
            // then reclassify the scope as LocalEnvironment (leave it as HostnameResolution if we do have general connectivity).
            scope = up
                ? (scope is ConnectivityScope.HostnameResolution ? ConnectivityScope.HostnameResolution : ConnectivityScope.RemoteSite)
                : ConnectivityScope.LocalEnvironment;
        }

        return scope;
    }

    /// <summary>
    /// Annotates the specified exception with its connectivity scope using the provided classifier and probe.
    /// </summary>
    /// <param name="ex">The exception to annotate.  Must not be <see langword="null"/>.</param>
    /// <param name="classifier">The connectivity classifier to use.  Must not be <see langword="null"/>.</param>
    /// <param name="probe">The connectivity probe to use for additional checks.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// The determined <see cref="ConnectivityScope"/> for the exception, after annotating <paramref name="ex"/> with
    /// the scope.  Returns <see cref="ConnectivityScope.Unknown"/> when the exception is not connectivity-related.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ex"/>, <paramref name="classifier"/>, or
    /// <paramref name="probe"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException"/>
    /// <remarks>
    /// <para>
    /// Sets <c>ex.Data["ConnectivityScope"]</c> to the determined <see cref="ConnectivityScope"/>, but only if the
    /// key is not already present in <see cref="Exception.Data"/>.
    /// </para>
    /// </remarks>
    public static async Task<ConnectivityScope> AnnotateExceptionWithConnectivityScopeAsync(
        Exception ex,
        IConnectivityClassifier classifier,
        IConnectivityProbe probe,
        CancellationToken cancellationToken)
    {
        var scope = await GetConnectivityScopeAsync(ex, classifier, probe, cancellationToken).ConfigureAwait(false);

        if (ex.Data["ConnectivityScope"] is not ConnectivityScope)
            ex.Data["ConnectivityScope"] = scope;

        return scope;
    }

    /// <summary>
    /// Classifies a timeout or non-cancellation <see cref="TaskCanceledException"/> as connectivity-related or not.
    /// </summary>
    /// <param name="ex">The exception to classify.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">
    /// Used to distinguish a <see cref="TaskCanceledException"/> caused by a timeout from one caused by explicit
    /// cancellation.  Not used for async cancellation.
    /// </param>
    /// <returns>
    /// A <see cref="ConnectivityClassificationResult"/> indicating whether the exception is connectivity-related.  When
    /// connectivity-related, the scope is always <see cref="ConnectivityScope.Unknown"/> as timeouts are treated as ambiguous.
    /// </returns>
    public static ConnectivityClassificationResult ClassifyTimeout(Exception ex, CancellationToken cancellationToken)
    {
        // Treat general timeouts as ambiguous (could be local or remote).
        return ex is TimeoutException || (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested)
            ? ConnectivityClassificationResult.ConnectivityRelated(ConnectivityScope.Unknown)
            : ConnectivityClassificationResult.NotConnectivityRelated;
    }

    /// <summary>
    /// Classifies an <see cref="HttpRequestException"/> as connectivity-related or not.
    /// </summary>
    /// <param name="ex">The HTTP request exception to classify.  Must not be <see langword="null"/>.</param>
    /// <returns>
    /// A <see cref="ConnectivityClassificationResult"/> indicating whether the exception is connectivity-related and its
    /// scope.  Returns <see cref="ConnectivityClassificationResult.NotConnectivityRelated"/> when no identifying inner
    /// exception can be found.
    /// </returns>
    public static ConnectivityClassificationResult ClassifyHttpException(HttpRequestException ex)
    {
        if (ex.StatusCode.HasValue)
            return ConnectivityClassificationResult.ConnectivityRelated(ConnectivityScope.RemoteSite);

        var socket = UnwrapInnerException<SocketException>(ex);
        if (socket is not null)
        {
            var scope = socket.SocketErrorCode switch
            {
                SocketError.NetworkUnreachable or SocketError.NetworkDown => ConnectivityScope.LocalEnvironment,
                SocketError.HostNotFound => ConnectivityScope.HostnameResolution,
                SocketError.HostUnreachable or SocketError.ConnectionRefused or SocketError.TimedOut or SocketError.ConnectionReset => ConnectivityScope.RemoteSite,
                _ => ConnectivityScope.Unknown
            };

            return ConnectivityClassificationResult.ConnectivityRelated(scope);
        }

        if (UnwrapInnerException<AuthenticationException>(ex) is not null)
            return ConnectivityClassificationResult.ConnectivityRelated(ConnectivityScope.RemoteSite);

        // An HttpRequestException with no identifying inner exception could still be connectivity-related,
        // but we don't have enough information to say so reliably here.
        return ConnectivityClassificationResult.NotConnectivityRelated;
    }

    /// <summary>
    /// Throws a connectivity-specific exception if the specified exception is determined to be connectivity-related.
    /// </summary>
    /// <param name="ex">The exception to analyse and possibly throw.  Must not be <see langword="null"/>.</param>
    /// <param name="classifier">The connectivity classifier to use.  Must not be <see langword="null"/>.</param>
    /// <param name="probe">The connectivity probe to use for additional checks.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// A <see cref="Task"/> that completes normally when the exception is not connectivity-related or scope is
    /// <see cref="ConnectivityScope.Unknown"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ex"/>, <paramref name="classifier"/>, or
    /// <paramref name="probe"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ConnectivityException">Thrown when the exception is classified as a
    /// <see cref="ConnectivityScope.LocalEnvironment"/> or <see cref="ConnectivityScope.HostnameResolution"/>
    /// connectivity failure.</exception>
    /// <exception cref="HttpRequestException">Thrown when the exception is classified as a
    /// <see cref="ConnectivityScope.RemoteSite"/> connectivity failure.</exception>
    public static async Task ThrowIfConnectivityFailureAsync(
        Exception ex, IConnectivityClassifier classifier, IConnectivityProbe probe, CancellationToken cancellationToken)
    {
        var scope = await GetConnectivityScopeAsync(ex, classifier, probe, cancellationToken).ConfigureAwait(false);

        if (scope is ConnectivityScope.RemoteSite)
        {
            if (ex is TimeoutException || (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested))
            {
                ex.Data["ConnectivityScope"] = scope;
                ExceptionDispatchInfo.Capture(ex).Throw();
            }

            var httpEx = ToHttpRequestException(ex, scope);
            ExceptionDispatchInfo.Capture(httpEx).Throw();
        }
        else if (scope is ConnectivityScope.LocalEnvironment or ConnectivityScope.HostnameResolution)
        {
            var connEx = ToConnectivityException(ex, scope);
            ExceptionDispatchInfo.Capture(connEx).Throw();
        }
    }

    private static HttpRequestException ToHttpRequestException(Exception ex, ConnectivityScope scope)
    {
        if (ex is HttpRequestException httpEx)
        {
            httpEx.Data["ConnectivityScope"] = scope;
            return httpEx;
        }

        var newHttpEx = new HttpRequestException(ex.Message, ex);
        newHttpEx.Data["ConnectivityScope"] = scope;
        return newHttpEx;
    }

    private static ConnectivityException ToConnectivityException(Exception ex, ConnectivityScope scope)
    {
        var connEx = new ConnectivityException(scope, ex);
        connEx.Data["ConnectivityScope"] = scope;
        return connEx;
    }

    private static T? UnwrapInnerException<T>(Exception ex) where T : Exception
    {
        for (var e = ex; e is not null; e = e.InnerException!)
            if (e is T t)
                return t;

        return null;
    }
}
