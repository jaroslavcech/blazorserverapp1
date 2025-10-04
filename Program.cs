using System;
using BlazorServerApp1.Components;
using BlazorServerApp1.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var httpsPort = builder.Configuration.GetValue<int?>("ASPNETCORE_HTTPS_PORT");
var urlsSetting = builder.Configuration["ASPNETCORE_URLS"];
var hasHttpsUrl = false;

if (!string.IsNullOrWhiteSpace(urlsSetting))
{
    var urls = urlsSetting.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var url in urls)
    {
        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            hasHttpsUrl = true;
            break;
        }
    }
}

if (httpsPort.HasValue)
{
    builder.Services.AddHttpsRedirection(options => options.HttpsPort = httpsPort.Value);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new InvalidOperationException("OpenAI API key is not configured. Set 'OpenAI:ApiKey' or the 'OPENAI_API_KEY' environment variable.");
    }

    return new OpenAIClient(apiKey);
});

builder.Services.AddScoped<Agent1>();
builder.Services.AddScoped<Agent2>();
builder.Services.AddScoped<ChatDispatcher>();

var app = builder.Build();

var shouldUseHttpsRedirection = !app.Environment.IsDevelopment() || httpsPort.HasValue || hasHttpsUrl;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (shouldUseHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
