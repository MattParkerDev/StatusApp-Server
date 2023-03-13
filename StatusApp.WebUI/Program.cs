using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using StatusApp.WebUI;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(
    sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
);

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddMudServices();

const string ApiClient = nameof(ApiClient);

var apiBaseUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:7104"
    : "https://statusapp1.azurewebsites.net";

builder.Services
    .AddHttpClient(
        ApiClient,
        client => client.BaseAddress = new Uri(apiBaseUrl ?? throw new InvalidOperationException())
    )
    .AddHttpMessageHandler(sp => sp.GetRequiredService<CookieHandler>());

builder.Services.AddHttpClient<Client>(ApiClient);

await builder.Build().RunAsync();
