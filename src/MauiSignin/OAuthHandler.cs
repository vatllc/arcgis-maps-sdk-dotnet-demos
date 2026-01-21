using Esri.ArcGISRuntime.Security;

namespace MauiSignin;

public class OAuthHandler : IOAuthHandler
{
    private OAuthHandler() { }

    public static OAuthHandler Instance { get; } = new OAuthHandler();

    public async Task<IDictionary<string, string>> LoginAsync(OAuthLoginParameters parameters)
    {
#if WINDOWS
        // using WinUIEx for handling OAuth on Windows until https://github.com/microsoft/WindowsAppSDK/issues/441 has been resolved and integrated with .NET MAUI's WebAuthenticator
        var result = await WinUIEx.WebAuthenticator.AuthenticateAsync(parameters.AuthorizeUri, parameters.RedirectUri);
#else
        var result = await MainThread.InvokeOnMainThreadAsync(() => // iOS requires authentication on main thread
        {
            return WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions()
            {
                CallbackUrl = parameters.RedirectUri,
                Url = parameters.AuthorizeUri
            });
        }).ConfigureAwait(false);
#endif
        return result.Properties;
    }
}