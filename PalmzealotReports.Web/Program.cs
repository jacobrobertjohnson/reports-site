using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using PalmzealotReports.Web.Config;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = new();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogleOpenIdConnect(o =>
    {
        o.ClientId = appSettings.Auth.ClientId;
        o.ClientSecret = appSettings.Auth.ClientSecret;

        // Use the standard callback path; let ASP.NET Core middleware handle redirection
        o.CallbackPath = new PathString("/signin-oidc");
        o.SaveTokens = true;
        
        // Ensure correlation and nonce cookies preserve SameSite=None for cross-site OAuth flow
        o.NonceCookie.SameSite = SameSiteMode.None;
        o.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        o.CorrelationCookie.SameSite = SameSiteMode.None;
        o.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        // App is behind a reverse proxy that terminates HTTPS; manually set redirect URI to use https
        o.Events = new OpenIdConnectEvents()
        {
            OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.RedirectUri = "https://"
                    + context.HttpContext.Request.Host
                    + "/signin-oidc";
                    
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // On callback, also set the expected redirect_uri to HTTPS so validation matches
                // what we told Google it should be
                context.ProtocolMessage.RedirectUri = "https://"
                    + context.HttpContext.Request.Host
                    + "/signin-oidc";
                    
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                System.Console.WriteLine($"OAuth Auth Failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            }
        };

        // https://developers.google.com/identity/protocols/oauth2/scopes#forms
        foreach (string scope in appSettings.Auth.Scopes)
            o.Scope.Add(scope);
    });

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
