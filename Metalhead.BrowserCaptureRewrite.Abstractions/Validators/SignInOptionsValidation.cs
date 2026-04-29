using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Validators;

/// <summary>
/// Validates a <see cref="SignInOptions"/> instance to ensure all settings have acceptable values.
/// </summary>
/// <remarks>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="SignInOptions"/>.
/// </remarks>
public sealed class SignInOptionsValidation() : IValidateOptions<SignInOptions>
{
    /// <summary>
    /// Validates the specified <see cref="SignInOptions"/> instance.
    /// </summary>
    /// <param name="name">The name of the options instance being validated, or <see langword="null"/> for the default instance.</param>
    /// <param name="options">The <see cref="SignInOptions"/> instance to validate.  Must not be <see langword="null"/>.</param>
    /// <returns>
    /// <see cref="ValidateOptionsResult.Success"/> if all values are valid; otherwise a
    /// <see cref="ValidateOptionsResult"/> containing one or more failure messages.
    /// </returns>
    public ValidateOptionsResult Validate(string? name, SignInOptions options)
    {
        var validationResults = new List<ValidationResult>();

        if (options.AssumeSignedInAfterSeconds is not null && options.AssumeSignedInAfterSeconds <= 0)
            validationResults.Add(new ValidationResult($"Assume sign-in after seconds value must be null or greater than 0 for settings property {nameof(SignInOptions.AssumeSignedInAfterSeconds)}."));

        if (options.PageLoadTimeoutSeconds is not null && options.PageLoadTimeoutSeconds <= 0)
            validationResults.Add(new ValidationResult($"Page load timeout seconds value must be null or greater than 0 for settings property {nameof(SignInOptions.PageLoadTimeoutSeconds)}."));

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
