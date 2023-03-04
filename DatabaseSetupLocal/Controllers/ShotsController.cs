using System.Diagnostics;
using System.Net;
using DatabaseSetupLocal.Data;
using Microsoft.AspNetCore.Mvc;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Rep;
using DatabaseSetupLocal.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Controllers;
[AllowAnonymous]
public class ShotsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public ShotsRepository ShotsRepository { get; set; }
    public ShotsContext ShotsContext { get; set; }
    public UserRepository UserRepository { get; set; }


    public ShotsController(ILogger<HomeController> logger)
    {
        _logger = logger;
        
        using (var db = new ShotsContext())
        {
            var test = db.RaceModel.ToList();
            db.Database.Migrate();
            if (!db.UserModel.Any())
            {
                DbSetup.Seed();
            }
        }

        this.ShotsRepository = new ShotsRepository(new ShotsContext());
        this.UserRepository = new UserRepository(new UsersContext());
        this.ShotsContext = ShotsRepository.GetShotsContext();
    }

    public IActionResult Index()
    {
        var users = ShotsRepository.GetUsers();

        return View(users.ToList());
    }

    public ActionResult Races(string userId, int year)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.Year = year;
        var races = ShotsRepository.GetUserRacesById(userId).Where(x => x.RaceYear == year).ToList();
        if (races == null)
        {
            return HttpNotFound();
        }

        return View(races);
    }

    public ActionResult Shots(string userId, int raceId, string raceLocation)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.Location = raceLocation;
        ViewBag.RaceId = raceId;
        var shots = ShotsRepository.GetUserShotsById(userId, raceId);
        if (shots == null)
        {
            return HttpNotFound();
        }

        return View(shots);
    }

    public ActionResult Years(string userId)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        var years = ShotsRepository.GetUserRacesById(userId).Select(x => x.RaceYear).Distinct().ToList();
        if (years == null)
        {
            return HttpNotFound();
        }

        return View(years);
    }
    public ActionResult EditOneShot(int? shotId)
    {
        if (shotId == null)
        {
            return NotFound();
        }
        var shot = ShotsContext.ShotModel.FirstOrDefault(s => s.Id == shotId);

        if (shot == null)
        {
            return HttpNotFound();
        }
        return View(shot);
    }
    [HttpPost, ActionName("EditOneShot")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOneShotPost(int? shotId)
    {
        if (shotId == null)
        {
            return NotFound();
        }
        var shotToUpdate = await ShotsContext.ShotModel.FirstOrDefaultAsync(s => s.Id == shotId);
        if (await TryUpdateModelAsync<Shot>(
                shotToUpdate,
                "",
                s => s.UsersShotDriver))
        {
            try
            {
                await ShotsContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }
        }
        return View(shotToUpdate);
    }
    public ActionResult EditMultipleShots(int? raceId)
    {
        if (raceId == null)
        {
            return NotFound();
        }
        var race = ShotsContext.RaceModel.FirstOrDefault(s => s.Id == raceId);

        if (race == null)
        {
            return HttpNotFound();
        }
        return View(race);
    }
    [HttpPost, ActionName("EditMultipleShots")]
    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMultipleShotsPost(int? raceId)
    {
        if (raceId == null)
        {
            return NotFound();
        }
        var shotsToUpdate = await ShotsContext.RaceModel.FirstOrDefaultAsync(s => s.Id == raceId);
        if (await TryUpdateModelAsync<Race>(
                shotsToUpdate,
                "",
                s => s.Shot))
        {
            try
            {
                await ShotsContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }
        }
        return View(shotsToUpdate);
    }
    

    private ActionResult HttpNotFound()
    {
        throw new NotImplementedException();
    }
}