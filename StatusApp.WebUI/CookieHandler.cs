using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace StatusApp.WebUI;

public class CookieHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // Send request and get response
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(request, cancellationToken);
    }
}
