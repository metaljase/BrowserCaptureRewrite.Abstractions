using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;
using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Validators;

/// <summary>
/// Provides validation logic for <see cref="ConnectivityProbeOptions"/> configuration settings.
/// </summary>
/// <remarks>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="ConnectivityProbeOptions"/>.
/// <para>
/// Validates that the probe URL is a valid absolute HTTP/HTTPS URI, the timeout is greater than zero, and the expected
/// status code is within the valid HTTP range (100–599).
/// </para>
/// <para>
/// Returns <see cref="ValidateOptionsResult.Fail"/> with detailed error messages for invalid settings, or
/// <see cref="ValidateOptionsResult.Success"/> if all validations pass.
/// </para>
/// </remarks>
public sealed class ConnectivityProbeOptionsValidation() : IValidateOptions<ConnectivityProbeOptions>
{
    /// <summary>
    /// Validates the specified <see cref="ConnectivityProbeOptions"/> instance.
    /// </summary>
    /// <param name="name">
    /// The name of the options instance being validated.  May be <see langword="null"/> for unnamed options.
    /// </param>
    /// <param name="options">
    /// The <see cref="ConnectivityProbeOptions"/> instance to validate.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValidateOptionsResult"/> indicating success or failure, with error messages for each validation failure.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Validation failures include:
    /// <list type="bullet">
    ///   <item><description>Probe URL must be a valid absolute HTTP/HTTPS URI if specified.</description></item>
    ///   <item><description>Timeout must be greater than zero.</description></item>
    ///   <item><description>Expected status code must be between 100 and 599 (inclusive).</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public ValidateOptionsResult Validate(string? name, ConnectivityProbeOptions options)
    {
        var validationResults = new List<ValidationResult>();

        if (options.ProbeUrl is not null && !UriHelper.IsValidHttpClientUri(options.ProbeUrl))
            validationResults.Add(new ValidationResult($"Connectivity probe URL is invalid: {options.ProbeUrl.OriginalString}"));

        if (options.TimeoutMilliseconds <= 0)
            validationResults.Add(new ValidationResult($"Connectivity probe timeout must be greater than 0 for settings property {nameof(ConnectivityProbeOptions.TimeoutMilliseconds)}."));

        if (options.ExpectedStatusCode < 100 || options.ExpectedStatusCode > 599)
            validationResults.Add(new ValidationResult($"Connectivity probe expected status code must be between 100 and 599 for settings property {nameof(ConnectivityProbeOptions.ExpectedStatusCode)}."));

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
