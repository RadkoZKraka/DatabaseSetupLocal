using System.Diagnostics;
using DatabaseSetupLocal.Data;
using Microsoft.AspNetCore.Mvc;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Rep;
using DatabaseSetupLocal.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DatabaseSetupLocal.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var appUserId = User.Identity.GetUserId();
        ViewBag.AppUserId = appUserId;
        var shotsRepo = new ShotsRepository(new ShotsContext());
        ViewBag.UserHasShots = shotsRepo.GetIfAppUserHasShots(appUserId);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}