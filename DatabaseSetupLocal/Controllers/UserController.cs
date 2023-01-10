using System.Diagnostics;
using DatabaseSetupLocal.Data;
using Microsoft.AspNetCore.Mvc;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Rep;
using DatabaseSetupLocal.Repository;

namespace DatabaseSetupLocal.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public UserRepository userRepository { get; set; }


    public UserController(ILogger<HomeController> logger)
    {
        _logger = logger;
        this.userRepository = new UserRepository(new ShotsContext());

        using (var db = new ShotsContext())
        {
            if (!db.UserModel.Any())
            {
                DbSetup.Seed();
            }
        }
    }

    public IActionResult Index()
    {
        var users = userRepository.GetUsers();

        return View(users.ToList());
    }
    public ActionResult Races(string userId)
    {
        ViewBag.User = userRepository.GetUserByID(userId);
        var races = userRepository.GetUserRacesByID(userId);
        if (races == null)
        {
            return HttpNotFound();
        }
        return View(races);
    }
    public ActionResult Shots(string userId, int raceId, string raceLocation)
    {
        ViewBag.User = userRepository.GetUserByID(userId);
        ViewBag.Location = raceLocation;
        var shots = userRepository.GetUserShotsByID(userId, raceId);
        if (shots == null)
        {
            return HttpNotFound();
        }
        return View(shots);
    }

    private ActionResult HttpNotFound()
    {
        throw new NotImplementedException();
    }
}