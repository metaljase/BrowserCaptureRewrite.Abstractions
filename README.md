# BrowserCaptureRewrite.Abstractions
`BrowserCaptureRewrite.Abstractions` is a .NET library that defines the abstractions for capturing and rewriting in-flight HTTP responses from web pages.  Implementations can intercept HTTP requests from a web page and capture the corresponding HTTP responses in-flight by targeting specific requests and optionally rewriting those responses - including the response for the web page itself.

Because intercepting and modifying HTTP responses happens in-flight before they reach the browser's rendering engine, you can fundamentally alter the page's behaviour.  For example, the initial web page HTML can be rewritten to manipulate the subsequent HTTP requests it makes.  Another example: a web page may fetch a JSON file and render its UI based on that data; by modifying the JSON response before the client-side code processes it, you can change the behaviour of the page.

Optionally, resiliency features such as retry logic and timeout handling can be configured, and the ability to manually sign-in is supported, for when the target web page requires authentication.

A key part for capturing in-flight HTTP responses is creating a `CaptureSpec` instance, which specifies what HTTP responses should be captured.  Similarly, rewriting in-flight HTTP responses relies on a `RewriteSpec` instance, which specifies which responses should be rewritten and how.

With a browser instance, an overload in one of the [convenience classes or extension methods](#capturerewrite-methods) can be called to perform the navigation, capture, and optional rewrite, by providing a `CaptureSpec` and optionally a `RewriteSpec` instance.

# Implementations
[`BrowserCaptureRewrite.Playwright`](https://github.com/metaljase/BrowserCaptureRewrite.Playwright) is a .NET library that implements the abstractions defined in `BrowserCaptureRewrite.Abstractions` using [Playwright](https://playwright.dev/dotnet/) to drive a browser instance.  It can capture and rewrite in-flight HTTP responses from web pages loaded in a Playwright-driven browser.

# Setup instructions
## Installing `BrowserCaptureRewrite.Abstractions`
Add the `BrowserCaptureRewrite.Abstractions` NuGet package to your project via your IDE, or by running the following command:
```bash
dotnet add package Metalhead.BrowserCaptureRewrite.Abstractions
```

## Configuration
The following settings can be added to `appsettings.json` or supplied through any other .NET configuration provider (e.g. environment variables, user secrets, command‑line arguments).

XML documentation for [`NavigationTimingOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/NavigationTimingOptions.cs), [`SignInOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/SignInOptions.cs), [`CaptureTimingOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/CaptureTimingOptions.cs), [`BrowserOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/BrowserOptions.cs), [`ResiliencePolicyOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Resilience/ResiliencePolicyOptions.cs), and [`ConnectivityProbeOptions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Connectivity/ConnectivityProbeOptions.cs) is available in the source code.

```json
{
  "SignInOptions": {
    "AssumeSignedInAfterSeconds": null,
    "PageLoadTimeoutSeconds": 30
  },
  "NavigationTimingOptions": {
    "PageLoadTimeoutSeconds": 30
  },
  "CaptureTimingOptions": {
    "NetworkIdleTimeoutSeconds": 10,
    "CaptureTimeoutSeconds": 20,
    "PollIntervalMilliseconds": 250
  },
  "BrowserOptions": {
    "Browser": "Chromium",
    "ExecutablePath": null,
    "Headless": false,
    "ViewportWidth": 1280,
    "ViewportHeight": 720,
    "UserAgent": null
  },
  "ResiliencePolicyOptions": {
    "TransportRetryDelaysSeconds": [ 3, 8, 15, 30, 60, 300 ],
    "TimeoutRetryDelaysSeconds": [ 1, 3, 5 ]
  },
  "ConnectivityProbeOptions": {
    "ProbeUrl": "http://www.msftncsi.com/ncsi.txt",
    "ExpectedStatusCode": 200,
    "TimeoutMilliseconds": 3000
  }
}
```

Register the options and their validators in your project's dependency injection container:
```csharp
builder.Services.AddOptions<SignInOptions>().Bind(builder.Configuration
    .GetSection(SignInOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<SignInOptions>, SignInOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SignInOptions>>().Value);

builder.Services.AddOptions<NavigationTimingOptions>().Bind(builder.Configuration
    .GetSection(NavigationTimingOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<NavigationTimingOptions>, NavigationTimingOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<NavigationTimingOptions>>().Value);

builder.Services.AddOptions<CaptureTimingOptions>().Bind(builder.Configuration
    .GetSection(CaptureTimingOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<CaptureTimingOptions>, CaptureTimingOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CaptureTimingOptions>>().Value);

builder.Services.AddOptions<BrowserOptions>().Bind(builder.Configuration
    .GetSection(BrowserOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<BrowserOptions>, BrowserOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<BrowserOptions>>().Value);

builder.Services.AddOptions<ResiliencePolicyOptions>().Bind(builder.Configuration
    .GetSection(ResiliencePolicyOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<ResiliencePolicyOptions>, ResiliencePolicyOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ResiliencePolicyOptions>>().Value);

builder.Services.AddOptions<ConnectivityProbeOptions>().Bind(builder.Configuration
    .GetSection(ConnectivityProbeOptions.SectionName));
builder.Services.AddSingleton<IValidateOptions<ConnectivityProbeOptions>, ConnectivityProbeOptionsValidation>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ConnectivityProbeOptions>>().Value);
```

# Examples
For brevity, the following examples omit exception handling and assume the [setup steps](#setup-instructions) have already been completed.  For examples with exception handling and dependency injection, see the `BrowserCaptureRewrite.Samples` & `BrowserCaptureRewrite.Samples.Core` projects in the [`BrowserCaptureRewrite.Playwright`](https://github.com/metaljase/BrowserCaptureRewrite.Playwright) repository.

## Using extension methods
The extension methods are the simplest solution for capturing in-flight HTTP responses from known URLs or URLs with specific file extensions, because custom capture‑completion logic is not required.  The following example performs two separate capture operations, 1) capturing responses containing JSON data by their URLs, and 2) capturing responses by their file extension.
```csharp
public class ExtensionMinimalSample(
    NavigationTimingOptions navigationTimingOptions,
    CaptureTimingOptions captureTimingOptions,
    IBrowserSessionService browserSessionService,
    IBrowserSessionResilienceWrapper resilienceWrapper,
    IBrowserCaptureService captureService)
    : IExtensionMinimalSample
{
    public async Task CaptureResponsesAsync(CancellationToken cancellationToken = default)
    {
        // Create a browser session, wrapped in resiliency to handle transient errors and retries.
        await using var resilientSession = resilienceWrapper.Wrap(
            await browserSessionService.CreateBrowserSessionOrThrowAsync(cancellationToken)
            .ConfigureAwait(false));

        Uri pageUrl = new("https://metaljase.github.io/browsercapturerewrite/index.html?albumsDelay=3");
        Uri[] urlsToCapture = [
            new("https://metaljase.github.io/browsercapturerewrite/bands_a-m.json"),
            new("https://metaljase.github.io/browsercapturerewrite/bands_n-z.json"),
            new("https://metaljase.github.io/browsercapturerewrite/albums.json")];

        // Capture contents of in-flight HTTP responses for all three JSON files, including albums.json
        // that's fetched after a 3-second delay.  The extension method creates a CaptureSpec
        // with a capture-completion predicate that only completes once all 3 URLs have been captured.
        IReadOnlyList<CapturedResource> resultByUrls =
            await captureService.NavigateAndCaptureResourcesAsync(
                resilientSession,
                pageUrl,
                urlsToCapture,
                cancellationToken,
                refererUrl: null,
                navigationTimingOptions.PageLoadTimeout(),
                captureTimingOptions.NetworkIdleTimeout(),
                captureTimingOptions.CaptureTimeout(),
                pollInterval: captureTimingOptions.PollInterval(),
                rewriteSpec: null)
            .ConfigureAwait(false);

        Console.WriteLine("Example 1...");
        foreach (var resource in resultByUrls)
            Console.Write(resource.TextContent);

        // Capture contents of in-flight HTTP responses with a .json extension, however, albums.json
        // will not be captured due to the fetch delay.  The extension method creates a
        // CaptureSpec that captures HTTP responses with a .json extension, but it doesn't
        // include a capture-completion predicate because it doesn't know what JSON files will be
        // fetched, thus nor what JSON files should be captured.  Therefore, capture completes when zero
        // network traffic has been observed for a duration of 500ms.
        // NOTE: A capture-completion predicate can be provided as a parameter, where custom logic can
        // control when capture should complete, e.g. after specific URLs have been captured, or a
        // duration of time has elapsed, or when the file contains certain data.
        IReadOnlyList<CapturedResource> resultByFileExt =
            await captureService.NavigateAndCaptureResourcesAsync(
                resilientSession,
                pageUrl,
                [".json"],
                cancellationToken,
                refererUrl: null,
                navigationTimingOptions.PageLoadTimeout(),
                captureTimingOptions.NetworkIdleTimeout(),
                captureTimingOptions.CaptureTimeout(),
                captureTimingOptions.PollInterval(),
                rewriteSpec: null,
                shouldCompleteCapture: null)
            .ConfigureAwait(false);

        Console.WriteLine("Example 2...");
        foreach (var resource in resultByFileExt)
            Console.Write(resource.TextContent);
    }
}
```

## Using an IBrowserDomCaptureService convenience method
The convenience methods provide the most flexible solution for capturing in-flight HTTP responses when you need custom capture-completion logic or response-rewrite logic.  The following example captures responses containing JSON data by their URLs (via a [`CaptureSpec`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/CaptureSpec.cs)) and rewrites the response body of `bands_a-m.json` by adding more bands (via a [`RewriteSpec`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/RewriteSpec.cs)) before the rewritten response is processed by the client-side code.  The custom capture-completion logic ensures capture only completes once all targeted URLs have been captured, rather than relying on the default period of network inactivity (500ms).
```csharp
public class ConvenienceMinimalSample(
    NavigationTimingOptions navigationTimingOptions,
    CaptureTimingOptions captureTimingOptions,
    IBrowserSessionService browserSessionService,
    IBrowserSessionResilienceWrapper resilienceWrapper,
    IBrowserDomCaptureService domCaptureService)
    : IConvenienceMinimalSample
{
    public async Task CaptureResponsesAndRenderedHtmlAsync(CancellationToken cancellationToken = default)
    {
        // Create a browser session, wrapped in resiliency to handle transient errors and retries.
        await using var resilientSession = resilienceWrapper.Wrap(
            await browserSessionService.CreateBrowserSessionOrThrowAsync(cancellationToken)
            .ConfigureAwait(false));

        Uri pageUrl = new("https://metaljase.github.io/browsercapturerewrite/index.html?albumsDelay=3");
        Uri bandsUrl = new("https://metaljase.github.io/browsercapturerewrite/bands_a-m.json");
        List<Uri> urlsToCapture = [
            bandsUrl,
            new("https://metaljase.github.io/browsercapturerewrite/bands_n-z.json"),
            new("https://metaljase.github.io/browsercapturerewrite/albums.json")];
        
        IReadOnlyList<string> addBands = ["A", "AA", "AAA", "AAAA"];

        // Create a CaptureSpec that captures the contents of in-flight HTTP responses for all
        // three JSON files, and only completes capture when all three JSON files have been captured.
        CaptureSpec captureSpec = new(
            shouldCapture: req => urlsToCapture.Contains(new Uri(req.Url)),
            tryCreateCapturedResourceAsync: TryCreateCapturedResourceAsync,
            shouldCompleteCapture: (navOptions, capturedResources, lastCapturedTime) =>
                urlsToCapture.All(url => capturedResources.Any(r => r.Url.Equals(url))));

        // Create a RewriteSpec that rewrites the in-flight HTTP response body of bands_a-m.json
        // by adding more bands.
        RewriteSpec rewriteSpec = new(
            shouldRewrite: req => Uri.TryCreate(req.Url, UriKind.Absolute, out var requestUri) && requestUri.Equals(bandsUrl),
            tryRewriteResponseAsync: async (req, resp) =>
                await RewriteAsync(req, resp, addBands).ConfigureAwait(false));

        NavigationOptions navigationOptions = new(
            pageUrl, RefererUrl: null, PageLoadTimeout: navigationTimingOptions.PageLoadTimeout());

        // Capture contents of in-flight HTTP responses for all three JSON files, including albums.json
        // that's fetched after a 3-second delay.  The CaptureSpec has a capture-completion
        // predicate that only completes once all three URLs have been captured.
        PageCaptureResult result = await domCaptureService.NavigateAndCaptureHtmlAndResourcesResultAsync(
            resilientSession,
            navigationOptions,
            captureSpec,
            rewriteSpec,
            cancellationToken,
            captureTimingOptions)
            .ConfigureAwait(false);

        Console.WriteLine(result.RenderedHtml);
        foreach (var resource in result.Resources)
            Console.Write(resource.TextContent);
    }

    private record Bands(List<string> BandNames);

    // This method is supplied as the tryCreateCapturedResourceAsync delegate, and is invoked for each
    // response that matches the shouldCapture predicate.  The response should be examined to determine
    // if it contains data you want to keep; if it does, return it as a CapturedResource.
    // It's OK to return a CapturedResource containing a response you're NOT interested in, as this can
    // be filtered out later, so your logic can be relatively loose, but this will use more memory than
    // necessary, and may affect performance for large responses.  For this example, the shouldCapture
    // predicate ensures only bands_a-m.json is intercepted, so examining the content isn't necessary.
    private static async Task<CapturedResource?> TryCreateCapturedResourceAsync(
        IRequestInfo req, IResponseInfo resp)
    {
        resp.Headers.TryGetValue("content-type", out var contentType);
        if (!Uri.TryCreate(req.Url, UriKind.Absolute, out var requestUri))
            return null;

        var body = await resp.GetBodyAsStringAsync().ConfigureAwait(false);
        return new CapturedResource(requestUri, body, null, contentType, resp.StatusCode, resp.Headers);
    }

    // This method is supplied as the rewriteResponse delegate, and is invoked for each response that
    // matches the shouldRewrite predicate, i.e. the URL for bands_a-m.json.  The response body is
    // deserialized, modified by adding more bands, and then serialized back to JSON and returned.  The
    // browser will receive the modified response body.
    private static async Task<ResponseRewriteResult> RewriteAsync(
        IRequestInfo req, IResponseInfo resp, IReadOnlyList<string> addBands)
    {
        if (!Uri.TryCreate(req.Url, UriKind.Absolute, out _))
            return ResponseRewriteResult.NotRewritten;

        var body = await resp.GetBodyAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(body) 
            || !TryDeserialize<Bands>(body, out var bands)
            || bands?.BandNames is not { Count: > 0 })
            return ResponseRewriteResult.NotRewritten;

        bands.BandNames.InsertRange(0, addBands);
        return new ResponseRewriteResult(true, JsonSerializer.Serialize(bands), null);
    }

    private static bool TryDeserialize<T>(string json, out T? model)
    {
        try
        {
            model = JsonSerializer.Deserialize<T>(json);
            return model != null;
        }
        catch
        {
            model = default;
            return false;
        }
    }
}
```

# Capture/Rewrite methods
## Return types
The methods for capturing and rewriting in-flight HTTP responses return either a `Task<IReadOnlyList<CapturedResource>>` or a `Task<PageCaptureResult>`, depending on the method called.

XML documentation for [`CapturedResource`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/CapturedResource.cs), [`PageCaptureResult`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Models/PageCaptureResult.cs), [`PageLoadStatus`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Enums/PageLoadStatus.cs), and [`CaptureStatus`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Enums/CaptureStatus.cs) is available in the source code.
```csharp
public sealed record CapturedResource(
    Uri Url,
    string? TextContent,
    byte[]? BinaryContent,
    string? ContentType,
    int? StatusCode,
    IReadOnlyDictionary<string, string>? ResponseHeaders)
{
    public bool HasText => TextContent is not null;
    public bool HasBinary => BinaryContent is not null;
}
```

```csharp
public sealed record PageCaptureResult(
    string? ResponseHtml,
    string? RenderedHtml,
    IReadOnlyList<CapturedResource> Resources,
    PageLoadStatus? PageLoadStatus = null,
    ResourceCaptureStatus? ResourceCaptureStatus = null);

public enum PageLoadStatus
{
    Completed,
    NetworkIdleTimeoutExceeded
}

public enum CaptureStatus
{
    CriteriaNotSatisfied,
    CriteriaSatisfied,
    CaptureTimeoutExceeded,
    UrlChangedBeforeCompletion
}
```


## Extension methods
XML documentation for [`BrowserCaptureServiceExtensions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/BrowserCaptureServiceExtensions.cs) and [`BrowserDomCaptureServiceExtensions`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/BrowserDomCaptureServiceExtensions.cs) is available in the source code.

Methods in `BrowserCaptureServiceExtensions` only return in-flight HTTP responses, and does not return the page's response HTML or rendered HTML.

```csharp
public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    this IBrowserCaptureService service,
    IBrowserSession session,
    Uri url,
    string[] fileExtensions,
    CancellationToken cancellationToken,
    Uri? refererUrl = null,
    TimeSpan? navigationTimeout = null,
    TimeSpan? networkIdleTimeout = null,
    TimeSpan? networkCallsTimeout = null,
    TimeSpan? pollInterval = null,
    RewriteSpec? rewriteSpec = null,
    Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null)

public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    this IBrowserCaptureService service,
    IBrowserSession session,
    Uri url,
    Uri[] urlsToCapture,
    CancellationToken cancellationToken,
    Uri? refererUrl = null,
    TimeSpan? navigationTimeout = null,
    TimeSpan? networkIdleTimeout = null,
    TimeSpan? networkCallsTimeout = null,
    TimeSpan? pollInterval = null,
    RewriteSpec? rewriteSpec = null)
```

Methods in `BrowserDomCaptureServiceExtensions` return the page's response HTML, rendered HTML, and in-flight HTTP responses.
```csharp
public static async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
    this IBrowserDomCaptureService service,
    IBrowserSession session,
    Uri url,
    Uri? refererUrl,
    CaptureSpec captureSpec,
    CancellationToken cancellationToken,
    TimeSpan? navigationTimeout = null,
    TimeSpan? networkIdleTimeout = null,
    TimeSpan? networkCallsTimeout = null,
    TimeSpan? pollInterval = null)

public static async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
    this IBrowserDomCaptureService service,
    IBrowserSession session,
    Uri url,
    Uri? refererUrl,
    CaptureSpec captureSpec,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken,
    TimeSpan? navigationTimeout = null,
    TimeSpan? networkIdleTimeout = null,
    TimeSpan? networkCallsTimeout = null,
    TimeSpan? pollInterval = null)
```

## Convenience classes / interfaces
XML documentation for [`IBrowserDomService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserDomService.cs), [`IBrowserCaptureService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserCaptureService.cs), and [`IBrowserDomCaptureService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserDomCaptureService.cs) is available in the source code.

[`IBrowserDomService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserDomService.cs):  Implementations (`DefaultBrowserDomService`) only return the page's response HTML and/or rendered HTML, and do not return in-flight HTTP responses.

```csharp
Task<string?> NavigateAndCaptureResponseHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CancellationToken cancellationToken = default);

Task<string?> NavigateAndCaptureResponseHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);

Task<string?> NavigateAndCaptureRenderedHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout = null,
    CancellationToken cancellationToken = default);

Task<string?> NavigateAndCaptureRenderedHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout = null,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);

Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout = null,
    CancellationToken cancellationToken = default);

Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout = null,
    CancellationToken cancellationToken = default);

Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    TimeSpan? networkIdleTimeout,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken = default);
```

[`IBrowserCaptureService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserCaptureService.cs): Implementations (`DefaultBrowserCaptureService`) only return in-flight HTTP responses, and do not return the page's response HTML or rendered HTML.

```csharp
Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    string[] fileExtensions,
    CancellationToken cancellationToken,
    RewriteSpec? rewriteSpec = null,
    Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
    CaptureTimingOptions? timingOptions = null);

Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    string[] fileExtensions,
    CancellationToken cancellationToken,
    RewriteSpec? rewriteSpec = null,
    Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
    CaptureTimingOptions? timingOptions = null);

Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    Uri[] urlsToCapture,
    CancellationToken cancellationToken,
    RewriteSpec? rewriteSpec = null,
    CaptureTimingOptions? timingOptions = null);

Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    Uri[] urlsToCapture,
    CancellationToken cancellationToken,
    RewriteSpec? rewriteSpec = null,
    CaptureTimingOptions? timingOptions = null);

Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);

Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);

Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);

Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);
```

[`IBrowserDomCaptureService`](https://github.com/metaljase/BrowserCaptureRewrite.Abstractions/blob/master/Metalhead.BrowserCaptureRewrite.Abstractions/Engine/IBrowserDomCaptureService.cs): Implementations (`DefaultBrowserDomCaptureService`) return the page's response HTML, rendered HTML, and in-flight HTTP responses.

```csharp
Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);

Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
    IBrowserSession session,
    NavigationOptions navOptions,
    CaptureSpec captureSpec,
    RewriteSpec? rewriteSpec,
    CancellationToken cancellationToken,
    CaptureTimingOptions? timingOptions = null);
```
