using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Validators;

/// <summary>
/// Validates a <see cref="SignInOptions"/> instance to ensure all settings have acceptable values.
/// </summary>
/// <remarks>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="CaptureTimingOptions"/>.
/// </remarks>
public sealed class CaptureTimingOptionsValidation() : IValidateOptions<CaptureTimingOptions>
{
    /// <summary>
    /// Validates the specified <see cref="CaptureTimingOptions"/> instance.
    /// </summary>
    /// <param name="name">The name of the options instance being validated, or <see langword="null"/> for the default instance.</param>
    /// <param name="options">The <see cref="CaptureTimingOptions"/> instance to validate.  Must not be <see langword="null"/>.</param>
    /// <returns>
    /// <see cref="ValidateOptionsResult.Success"/> if all values are valid; otherwise a
    /// <see cref="ValidateOptionsResult"/> containing one or more failure messages.
    /// </returns>
    public ValidateOptionsResult Validate(string? name, CaptureTimingOptions options)
    {
        var validationResults = new List<ValidationResult>();

        if (options.NetworkIdleTimeoutSeconds is not null && options.NetworkIdleTimeoutSeconds <= 0)
            validationResults.Add(new ValidationResult($"Network idle timeout seconds value must be null or greater than 0 for settings property {nameof(CaptureTimingOptions.NetworkIdleTimeoutSeconds)}."));

        if (options.CaptureTimeoutSeconds is not null && options.CaptureTimeoutSeconds <= 0)
            validationResults.Add(new ValidationResult($"Capture timeout seconds value must be null or greater than 0 for settings property {nameof(SignInOptions.PageLoadTimeoutSeconds)}."));

        if (options.PollIntervalMilliseconds is not null && options.PollIntervalMilliseconds <= 0)
            validationResults.Add(new ValidationResult($"Capture poll interval milliseconds value must be null or greater than 0 for settings property {nameof(CaptureTimingOptions.PollIntervalMilliseconds)}."));

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
