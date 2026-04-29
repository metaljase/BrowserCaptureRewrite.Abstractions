namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Specifies the browser engine to use for browser automation.
/// </summary>
public enum BrowserEngine
{
    /// <summary>
    /// The Google Chrome stable-channel browser.
    /// </summary>
    Chrome,

    /// <summary>
    /// The Google Chrome Dev-channel browser.
    /// </summary>
    ChromeDev,

    /// <summary>
    /// The Chromium open-source browser.
    /// </summary>
    Chromium,

    /// <summary>
    /// The Microsoft Edge browser.
    /// </summary>
    Edge,

    /// <summary>
    /// The Mozilla Firefox browser.
    /// </summary>
    Firefox,

    /// <summary>
    /// The Apple WebKit browser engine, used by Safari.
    /// </summary>
    WebKit
}
