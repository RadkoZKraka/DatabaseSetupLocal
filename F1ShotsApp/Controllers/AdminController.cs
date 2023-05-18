using System.Runtime.InteropServices.JavaScript;
using DatabaseSetupLocal.Areas.Identity.Data;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Library;
using DatabaseSetupLocal.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserRepository _userRepository;
    private readonly ShotsRepository _shotRepository;

    public AdminController(ILogger<AdminController> logger, ShotsRepository  ShotsRepository, UserRepository UserRepository)
    {
        _userRepository = UserRepository;
        _shotRepository = ShotsRepository;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Users()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _userRepository.GetUsers();
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);

        return View(usersList);
    }

    public IActionResult LockPreviousRaces()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _userRepository.GetUsers();
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        AppSetup.LockPreviousRaces();
        return RedirectToAction("Index");
    }

    public IActionResult LockPreviousYear()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _userRepository.GetUsers();
        
        
        _shotRepository.LockYear(2022);
        ViewBag.IsAdmin = _userRepository.GetIfUserIsAdminById(appUserId);
        return RedirectToAction("Index");
    }


    public IActionResult LockCurrentRace()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _userRepository.GetUsers();
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);

        return RedirectToAction("Index");
    }

    public IActionResult SumPointsForRaces()
    {
        var races = _shotRepository.GetRaces();
        foreach (var race in races)
        {
            _shotRepository.SumPointsInRace(race);
        }

        return RedirectToAction("Index");
    }

    public IActionResult ImportFromClipBoard(string data)
    {
        var test = "ss";
        return RedirectToAction("Index");

    }
    public IActionResult ChangeAllAbrToFullName()
    {
        var all = _shotRepository.GetUsers();
        foreach (var userShots in all)
        {
            foreach (var race in userShots.Race)
            {
                _shotRepository.ChangeAllAbrToFullNameInARace(race);
            }
        }
        
        return RedirectToAction("Index");

    }

    public IActionResult CountPointsInLockedRaces()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _shotRepository.GetUsers();
        var lockedRaces = usersList.Select(x => x.Race.Where(x => x.Locked)).SelectMany(x => x).ToList();
        foreach (var lockedRace in lockedRaces)
        {
            _shotRepository.CountPointsByRace(lockedRace);
        }

        ;
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);

        return RedirectToAction("Index");
    }

    public ActionResult EditAppUser(string? userId)
    {
        var appUserId = User.Identity.GetUserId();

        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        var user = _userRepository.GetUserById(userId);


        return View(user);
    }

    [HttpPost, ActionName("EditAppUser")]
    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAppUserPost(string userId)
    {
        var userContext = new UsersContext();
        var userToUpdate = await userContext.UserModel.FindAsync(userId);
        if (await TryUpdateModelAsync<AppUser>(
                userToUpdate,
                "",
                s => s.Admin))
        {
            try
            {
                await userContext.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }
        }

        return View(userToUpdate);
    }

    public ActionResult EditUsers()
    {
        var appUserId = User.Identity.GetUserId();

        var usersList = _userRepository.GetUsers();
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        var users = _userRepository.GetUsers();


        return View(users);
    }

    [HttpPost, ActionName("EditUsers")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUsersPost(string? userId)
    {
        var usersContext = new UsersContext();
        var appUsers = new AppUsers();
        var usersToUpdate = await usersContext.UserModel.ToListAsync();
        appUsers.Users = new List<AppUser>(usersToUpdate);
        if (await TryUpdateModelAsync<AppUsers>(
                appUsers,
                "",
                s => s.Users))
        {
            try
            {
                await usersContext.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }
        }

        return View(usersToUpdate);
    }

    public void DownloadCountryListOfRaces()
    {
        var year = DateTime.Now.Year;
        F1WebScraper.GetCountryListOfRaces(year);
    }

    public IActionResult DownloadDatesListOfRaces()
    {
        var year = DateTime.Now.Year;
        F1WebScraper.GetDatesListOfRaces(year);
        return RedirectToAction("Index");
    }

    public IActionResult DownloadResultOfRaces(int year, int raceNumber)
    {
        var raceResults = F1WebScraper.GetRaceResults(year, raceNumber);
        return RedirectToAction("Index");
    }

    public IActionResult GetDriversData()
    {
        int[] years = {2022, 2023};
        foreach (var year in years)
        {
            F1WebScraper.GetDriversData(year);
        }

        return RedirectToAction("Index");
    }

    public IActionResult ResetLegacyData()
    {
        AppSetup.DeleteDb();
        AppSetup.SeedDb();
        return RedirectToAction("Index");
    }
    


    public IActionResult GetScheduleData()
    {
        F1WebScraper.GetScheduleData();
        return RedirectToAction("Index");
    }

    public void DeleteUser()
    {
        var userId = User.Identity.GetUserId();
        _userRepository.DeleteUser(userId);
    }
}

class AppUsers
{
    public List<AppUser> Users { get; set; }
}