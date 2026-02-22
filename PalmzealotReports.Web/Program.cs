using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    .AddCookie()
    .AddGoogleOpenIdConnect(o =>
    {
        o.ClientId = appSettings.Auth.ClientId;
        o.ClientSecret = appSettings.Auth.ClientSecret;

        // https://developers.google.com/identity/protocols/oauth2/scopes#forms
        foreach (string scope in appSettings.Auth.Scopes)
            o.Scope.Add(scope);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
