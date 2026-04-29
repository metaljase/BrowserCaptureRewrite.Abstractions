using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Validators;

/// <summary>
/// Provides validation logic for <see cref="BrowserOptions"/> configuration settings.
/// </summary>
/// <remarks>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="BrowserOptions"/>.
/// <para>
/// Validates viewport dimensions, executable path, and ensures that width and height are set together and are
/// positive if specified.
/// </para>
/// <para>
/// Returns <see cref="ValidateOptionsResult.Fail"/> with detailed error messages for invalid settings, or
/// <see cref="ValidateOptionsResult.Success"/> if all validations pass.
/// </para>
/// </remarks>
public sealed class BrowserOptionsValidation() : IValidateOptions<BrowserOptions>
{
    /// <summary>
    /// Validates the specified <see cref="BrowserOptions"/> instance.
    /// </summary>
    /// <param name="name">
    /// The name of the options instance being validated.  May be <see langword="null"/> for unnamed options.
    /// </param>
    /// <param name="options">
    /// The <see cref="BrowserOptions"/> instance to validate.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValidateOptionsResult"/> indicating success or failure, with error messages for each validation failure.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Validation failures include:
    /// <list type="bullet">
    ///   <item><description>Viewport width and height must be set together.</description></item>
    ///   <item><description>Viewport width and height, if set, must be greater than zero.</description></item>
    ///   <item><description>Executable path, if set, must exist on disk.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public ValidateOptionsResult Validate(string? name, BrowserOptions options)
    {
        var validationResults = new List<ValidationResult>();

        if ((options.ViewportWidth is null && options.ViewportHeight is not null)
            || (options.ViewportWidth is not null && options.ViewportHeight is null))
            validationResults.Add(new ValidationResult($"Both browser window width and height must be set together for settings properties {nameof(BrowserOptions.ViewportWidth)} and {nameof(BrowserOptions.ViewportHeight)}."));

        if (options.ViewportWidth is not null && options.ViewportWidth <= 0)
            validationResults.Add(new ValidationResult($"Browser viewport width must be null or greater than 0 for settings property {nameof(BrowserOptions.ViewportWidth)}."));

        if (options.ViewportHeight is not null && options.ViewportHeight <= 0)
            validationResults.Add(new ValidationResult($"Browser viewport height must be null or greater than 0 for settings property {nameof(BrowserOptions.ViewportHeight)}."));

        if (!string.IsNullOrWhiteSpace(options.ExecutablePath) && !File.Exists(Environment.ExpandEnvironmentVariables(options.ExecutablePath)))
            validationResults.Add(new ValidationResult($"Invalid path for settings property {nameof(BrowserOptions.ExecutablePath)}."));

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
