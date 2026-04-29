using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

using Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Validators;

/// <summary>
/// Provides validation logic for <see cref="ResiliencePolicyOptions"/> configuration settings.
/// </summary>
/// <remarks>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="ResiliencePolicyOptions"/>.
/// <para>
/// Validates that all transport and timeout retry delays are greater than zero.  Returns <see cref="ValidateOptionsResult.Fail"/>
/// with detailed error messages for invalid settings, or <see cref="ValidateOptionsResult.Success"/> if all validations pass.
/// </para>
/// <para>
/// This validator is intended for use with configuration binding and options validation in dependency injection scenarios.
/// </para>
/// </remarks>
public sealed class ResiliencePolicyOptionsValidation() : IValidateOptions<ResiliencePolicyOptions>
{
    /// <summary>
    /// Validates the specified <see cref="ResiliencePolicyOptions"/> instance.
    /// </summary>
    /// <param name="name">
    /// The name of the options instance being validated.  May be <see langword="null"/> for unnamed options.
    /// </param>
    /// <param name="options">
    /// The <see cref="ResiliencePolicyOptions"/> instance to validate.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValidateOptionsResult"/> indicating success or failure, with error messages for each validation failure.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Validation failures include:
    /// <list type="bullet">
    ///   <item><description>All transport retry delays must be greater than zero.</description></item>
    ///   <item><description>All timeout retry delays must be greater than zero.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public ValidateOptionsResult Validate(string? name, ResiliencePolicyOptions options)
    {
        var validationResults = new List<ValidationResult>();

        if (options.TransportRetryDelaysSeconds is not null && options.TransportRetryDelaysSeconds.Any(d => d <= 0))
            validationResults.Add(new ValidationResult($"All transport retry delays must be greater than 0 for settings property {nameof(ResiliencePolicyOptions.TransportRetryDelaysSeconds)}."));

        if (options.TimeoutRetryDelaysSeconds is not null && options.TimeoutRetryDelaysSeconds.Any(d => d <= 0))
            validationResults.Add(new ValidationResult($"All timeout retry delays must be greater than 0 for settings property {nameof(ResiliencePolicyOptions.TimeoutRetryDelaysSeconds)}."));

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
