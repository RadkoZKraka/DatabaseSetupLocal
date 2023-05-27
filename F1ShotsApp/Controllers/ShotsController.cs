using DatabaseSetupLocal.Data;
using Microsoft.AspNetCore.Mvc;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Library;
using DatabaseSetupLocal.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;


namespace DatabaseSetupLocal.Controllers;

[AllowAnonymous]
public class ShotsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ShotsRepository ShotsRepository { get; set; }
    private ShotsContext ShotsContext { get; set; }
    private UserRepository UserRepository { get; set; }

    public ShotsController(ILogger<HomeController> logger, ShotsRepository shotsRepository,
        UserRepository userRepository)
    {

        _logger = logger;

        this.ShotsRepository = shotsRepository;
        this.UserRepository = userRepository;
        this.ShotsContext = ShotsRepository.GetShotsContext();
    }

    public IActionResult Index()
    {
        var users = ShotsRepository.GetUsers();
        var userId = User.Identity.GetUserId();
        ViewBag.AppUserId = userId;
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userId);

        return View(users.ToList());
    }

    public IActionResult Results()
    {
        var users = ShotsRepository.GetUsers().ToArray();
        var userId = User.Identity.GetUserId();
        ViewBag.AppUserId = userId;
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userId);
        // ViewBag.IsMobile = isMobileDevice;

        return View(users.ToList());
    }

    public IActionResult Schedule()
    {
        ViewBag.UsersList = ShotsRepository.GetUsers();
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);

        var schedule = AppSetup.DeserializeDates();
        return View(schedule);
    }

    public ActionResult Races(string userId, int year)
    {
        ViewBag.UsersList = ShotsRepository.GetUsers();
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.Year = year;
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.HasAccessToEdit = ShotsRepository.GetUserById(userId).OwnerId == userIdentityId;
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        var races = ShotsRepository.GetUserRacesById(userId).Where(x => x.RaceYear == year).ToList();

        return View(races);
    }

    public ActionResult Shots(string userId, int raceId, string raceLocation)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.UserId = userId;
        ViewBag.Location = raceLocation;
        ViewBag.RaceId = raceId;
        ViewBag.Year = ShotsRepository.GetRaceYearById(raceId);

        var ownerId = User.Identity.GetUserId();
        ViewBag.HasAccessToEdit = ShotsRepository.GetUserById(userId).OwnerId == ownerId && ownerId != null ;
        ViewBag.IsAdmin = ownerId != null && UserRepository.GetIfUserIsAdminById(ownerId);

        var race = ShotsRepository.GetRaceById(raceId);
        var raceName = race!.RaceLocation;
        if (raceName != null) ViewBag.RaceSchedule = AppSetup.GetRaceScheduleBy(raceName);


        return View(race);
    }

    public ActionResult Years(string userId)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.UsersList = UserRepository.GetUsers();
        var years = ShotsRepository.GetUsersYears(userId);
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);

        return View(years);
    }

    public ActionResult CompareAll(int raceYear, int raceNo)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        var userShots = ShotsRepository.GetUsers();

        return View((userShots.ToList(), raceYear, raceNo));
    }

    public ActionResult HideUser(string userId, string user)
    {
        ShotsRepository.HideUser(userId, user);

        return RedirectToAction("Index");
    }

    public ActionResult ShowUser(string userId, string user)
    {
        ShotsRepository.ShowUser(userId, user);

        return RedirectToAction("Index");
    }

    public ActionResult DeleteUser(string userId)
    {
        ShotsRepository.DeleteUser(userId);

        return RedirectToAction("Index");
    }

    public ActionResult GetRaceResults(int raceId, string user)
    {
        ViewBag.UsersList = UserRepository.GetUsers();
        var race = ShotsRepository.GetRaceById(raceId);
        if (race != null)
        {
            ShotsRepository.CountPointsByRace(race);
            ShotsRepository.UpdateRace(race, user);
        }

        return Redirect(HttpContext.Request.Headers["Referer"]!);
    }


    public ActionResult EditOneShot(int? shotId)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
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

    public IActionResult ImportFromClipBoard(string data, int raceId, string user)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        var race = ShotsRepository.GetRaceById(raceId);
        var listOfShots = data.Split("\n");
        for (var i = 0; i < 20; i++)
        {
            if (race != null) race.Shot[i].UsersShotDriver = listOfShots[i];
        }

        if (race == null) return RedirectToAction("Index");
        race.PolePosition = listOfShots[20];
        race.FastestLap = listOfShots[21];
        ShotsRepository.UpdateRace(race, user);

        return RedirectToAction("Index");
    }


    [HttpPost, ActionName("EditOneShot")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOneShotPost(int? shotId)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        if (shotId == null)
        {
            return NotFound();
        }

        var shotToUpdate = await ShotsContext.ShotModel.FirstOrDefaultAsync(s => s.Id == shotId);
        if (shotToUpdate == null || !await TryUpdateModelAsync(
                shotToUpdate,
                "",
                s => s.UsersShotDriver)) return View(shotToUpdate);
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

        return View(shotToUpdate);
    }

    public ActionResult EditMultipleShots(int raceId, string userId)
    {
        ViewBag.User = ShotsRepository.GetUserById(userId);
        ViewBag.UserId = userId;
        ViewBag.Location = ShotsRepository.GetRaceById(raceId).RaceLocation;
        ViewBag.RaceId = raceId;
        ViewBag.Year = ShotsRepository.GetRaceById(raceId).RaceYear;
        ViewBag.PreviousUrl = HttpContext.Request.GetEncodedUrl();
        ViewBag.CurrentRace = AppSetup.GetCurrentRaceSchedule();
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);


        var selectListItems = new List<string>();
        selectListItems.AddRange(AppSetup.DeserializeDrivers(ShotsRepository.GetRaceYearById(raceId))!.Drivers
            .OrderBy(x => x.LastName).Select(x => x.FullName).ToList());
        ViewBag.F1Grid = selectListItems;

        var race = ShotsContext.RaceModel.FirstOrDefault(s => s.Id == raceId);
        if (race != null)
        {
            ViewBag.RaceSchedule = AppSetup.GetRaceScheduleBy(race.RaceLocation);

        }
        return View(race);

    }

    [HttpPost, ActionName("EditMultipleShots")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult?> EditMultipleShotsPost(int? raceId)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        if (raceId == null)
        {
            return NotFound();
        }

        var raceToUpdate = await ShotsContext.RaceModel.FirstOrDefaultAsync(s => s.Id == raceId);
        if (raceToUpdate == null) return null;
        {
            await ShotsRepository.LogAction("Race with ID: " + raceToUpdate.Id + " has been updated by " +
                                            userIdentityId +
                                            ".\n");

            if (!await TryUpdateModelAsync<Race>(
                    raceToUpdate,
                    "",
                    s => s.Shot, s => s.PolePosition, s => s.FastestLap)) return View(raceToUpdate);
            try
            {
                await ShotsContext.SaveChangesAsync();
                return Redirect(Request.GetEncodedUrl());
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }

            return View(raceToUpdate);
        }

    }

    public ActionResult AddUser(string userId)
    {
        if (String.IsNullOrEmpty(userId))
        {
            return HttpNotFound();
        }

        var user = UserRepository.GetUserById(userId);
        var shots = AppSetup.SetupShotsForNewUser(userId, user.FirstName + " " + user.LastName);
        ShotsRepository.InsertUser(shots);
        return RedirectToAction("Index");
    }

    public ActionResult EditUser(string userId)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        if (String.IsNullOrEmpty(userId))
        {
            return HttpNotFound();
        }

        var user = ShotsRepository.GetUserById(userId);
        ViewBag.UsersList = UserRepository.GetUsers().Select(x => new {x.Id, x.FirstName, x.LastName});
        return View(user);
    }

    [HttpPost, ActionName("EditUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserPost(string userId)
    {
        var userIdentityId = User.Identity.GetUserId();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        var userModelToUpdate = await ShotsContext.UserShotsModel.FirstOrDefaultAsync(s => s.Id == userId);
        if (userModelToUpdate != null)
        {
            await ShotsRepository.LogAction("User with ID: " + userModelToUpdate.Id + " has been updated by " +
                                            userIdentityId +
                                            ".\n");

            if (!await TryUpdateModelAsync(
                    userModelToUpdate,
                    "",
                    s => s.OwnerId, s => s.UserName)) return View("../Home/Index");
        }

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

        return View("../Home/Index");
    }


    public ActionResult CurrentRace()
    {
        var ownerId = User.Identity.GetUserId();
        var userShot = ShotsRepository.GetUserByOwnerId(ownerId);

        ViewBag.User = ShotsRepository.GetUserByOwnerId(ownerId);
        ViewBag.UserId = userShot.Id;
        ViewBag.RaceId = ShotsRepository.GetRaceIdByRaceLoc(userShot.Id, AppSetup.GetCurrentRaceLocation()) ?? 0;
        ViewBag.Location = AppSetup.GetCurrentRaceLocation();

        var userId = ShotsRepository.GetUserIdByOwnerId(ownerId);
        var race = ShotsRepository.GetCurrentRace(ownerId);
        ViewBag.HasAccessToEdit = ShotsRepository.GetUserById(userId).OwnerId == ownerId;
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(ownerId);


        return RedirectToAction("Shots", new {userId, raceId = race.Id, raceLocation = race.RaceLocation});
    }

    public ActionResult CurrentRaceById(string userId)
    {
        var ownerId = User.Identity.GetUserId();
        var userShot = ShotsRepository.GetUserByOwnerId(ownerId);

        ViewBag.User = ShotsRepository.GetUserByOwnerId(ownerId);
        ViewBag.UserId = userShot.Id;
        ViewBag.RaceId = ShotsRepository.GetRaceIdByRaceLoc(userShot.Id, AppSetup.GetCurrentRaceLocation()) ?? 0;
        ViewBag.Location = AppSetup.GetCurrentRaceLocation();

        var race = ShotsRepository.GetCurrentRace(ownerId);
        ViewBag.HasAccessToEdit = ShotsRepository.GetUserById(userId).OwnerId == ownerId;
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(ownerId);


        return RedirectToAction("Shots", new {userId, raceId = race.Id, raceLocation = race.RaceLocation});

    }

    public ActionResult LiveTiming()
    {
        var userIdentityId = User.Identity.GetUserId();
        var userShot = ShotsRepository.GetUserByOwnerId(userIdentityId);
        var raceYear = DateTime.Now.Year;
        var raceNo = ShotsRepository.GetCurrentRaceNo();

        ViewBag.User = ShotsRepository.GetUserByOwnerId(userIdentityId);
        ViewBag.UserId = userShot.Id;
        ViewBag.RaceId = ShotsRepository.GetRaceIdByRaceLoc(userShot.Id, AppSetup.GetCurrentRaceLocation()) ?? throw new InvalidOperationException();
        ViewBag.Location = AppSetup.GetCurrentRaceLocation();
        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);

        ViewBag.IsAdmin = UserRepository.GetIfUserIsAdminById(userIdentityId);
        var userShots = ShotsRepository.GetUsers();

        return View((userShots.ToList(), raceYear, raceNo));
    }

    [HttpGet]
    public async Task<JsonResult> GetLiveTiming()
    {
        var res = await F1WebScraper.GetLiveDataAsync();
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        var model = new JsonResponseViewModel
        {
            ResponseCode = 0,
            ResponseMessage = JsonConvert.SerializeObject(res)
        };

        return Json(model);
    }

    private static ActionResult HttpNotFound()
    {
        throw new NotImplementedException();
    }

    public IActionResult ChangeAbrToFullName(int raceId)
    {
        var ownerId = User.Identity.GetUserId();

        var userId = ShotsRepository.GetUserIdByOwnerId(ownerId);
        var race = ShotsRepository.GetRaceById(raceId);
        foreach (var shot in race.Shot)
        {
            shot.UsersShotDriver = AppSetup.AbrOneDriverToFullName(shot.UsersShotDriver, race.RaceYear);
            
        }
        ShotsRepository.UpdateRace(race, ownerId);
        return RedirectToAction("Shots", new {userId, raceId = race.Id, raceLocation = race.RaceLocation});

    }
}