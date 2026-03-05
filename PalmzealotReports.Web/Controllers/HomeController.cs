using Google.Apis.Auth.AspNetCore3;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PalmzealotReports.Web.Config;
using PalmzealotReports.Web.Models;
using System.Diagnostics;

namespace PalmzealotReports.Web.Controllers;

[Authorize]
public class HomeController(
    AppSettings _appSettings
) : Controller
{
    // [AllowAnonymous]
    // public IActionResult Login()
    // {
    //     return Challenge(new AuthenticationProperties { RedirectUri = "/" }, GoogleOpenIdConnectDefaults.AuthenticationScheme);
    // }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        return View(new FormListViewModel
        {
            Forms = _appSettings.Forms.Adapt<FormListItemViewModel[]>(),
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
