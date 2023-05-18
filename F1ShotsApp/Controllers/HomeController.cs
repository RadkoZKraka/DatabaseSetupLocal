using System.Diagnostics;
using DatabaseSetupLocal.Data;
using Microsoft.AspNetCore.Mvc;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Library;
using DatabaseSetupLocal.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DatabaseSetupLocal.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ShotsRepository _shotsRepository;

    public HomeController(ILogger<HomeController> logger, ShotsRepository shotsRepository)
    {
        this._shotsRepository = shotsRepository;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var appUserId = User.Identity.GetUserId();
        ViewBag.AppUserId = appUserId;

        var race = AppSetup.GetCurrentRaceSchedule();
        ViewBag.UserHasShots = _shotsRepository.GetIfAppUserHasShots(appUserId);
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        return View(race);
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