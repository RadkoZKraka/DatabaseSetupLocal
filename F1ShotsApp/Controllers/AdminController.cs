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

    public AdminController(ILogger<AdminController> logger, ShotsRepository shotsRepository,
        UserRepository userRepository)
    {
        _userRepository = userRepository;
        _shotRepository = shotsRepository;
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
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        _logger.LogInformation("Previous races have been locked by {AppUserId}", appUserId);
        AppSetup.LockPreviousRaces();
        return RedirectToAction("Index");
    }

    public IActionResult LockPreviousYear()
    {
        var appUserId = User.Identity.GetUserId();
        _logger.LogInformation("Previous year races have been locked by {AppUserId}", appUserId);
        _shotRepository.LockYear(2022);
        ViewBag.IsAdmin = _userRepository.GetIfUserIsAdminById(appUserId);

        return RedirectToAction("Index");
    }


    public IActionResult LockCurrentRace()
    {
        var appUserId = User.Identity.GetUserId();
        _logger.LogInformation("Current race have been locked by {AppUserId}", appUserId);
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);

        return RedirectToAction("Index");
    }

    public IActionResult UnlockCurrentRace()
    {
        var appUserId = User.Identity.GetUserId();
        _logger.LogInformation("Current race have been locked by {AppUserId}", appUserId);
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        int raceYearToUnlock = DateTime.Now.Year;
        var raceLoc = _shotRepository.GetCurrentRaceLoc();
        _shotRepository.UnlockRace(raceYearToUnlock, raceLoc);

        return RedirectToAction("Index");
    }

    public IActionResult SumPointsForRaces()
    {
        var appUserId = User.Identity.GetUserId();
        var races = _shotRepository.GetRaces();
        foreach (var race in races)
        {
            _shotRepository.SumPointsInRace(race);
        }
        _logger.LogInformation("Sum of points have be calculated by {AppUserId}", appUserId);

        
        return RedirectToAction("Index");
    }

    public IActionResult ChangeAllAbrToFullName()
    {
        var appUserId = User.Identity.GetUserId();
        var all = _shotRepository.GetUsers();
        foreach (var userShots in all)
        {
            foreach (var race in userShots.Race)
            {
                _shotRepository.ChangeAllAbrToFullNameInARace(race);
            }
        }
        _logger.LogInformation("Changed all abr to full names by {AppUserId}", appUserId);

        
        return RedirectToAction("Index");
    }

    public IActionResult CountPointsInLockedRaces()
    {
        var appUserId = User.Identity.GetUserId();
        var usersList = _shotRepository.GetUsers();
        var lockedRaces = usersList.Select(userShots => userShots.Race.Where(race => race.Locked)).SelectMany(listOfRaces => listOfRaces).ToList();
        foreach (var lockedRace in lockedRaces)
        {
            _shotRepository.CountPointsByRace(lockedRace);
        }
        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        _logger.LogInformation("Points have been counted in locked races by {AppUserId}", appUserId);


        return RedirectToAction("Index");
    }

    public ActionResult EditAppUser(string? userId)
    {
        var appUserId = User.Identity.GetUserId();

        var usersRepository = new UserRepository(new UsersContext());
        ViewBag.IsAdmin = usersRepository.GetIfUserIsAdminById(appUserId);
        if (userId != null)
        {
            var user = _userRepository.GetUserById(userId);


            return View(user);
        }
        return View("Index");
    }

    [HttpPost, ActionName("EditAppUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAppUserPost(string userId)
    {
        var appUserId = User.Identity.GetUserId();
        var userContext = new UsersContext();
        var userToUpdate = await userContext.UserModel.FindAsync(userId);
        if (userToUpdate != null && await TryUpdateModelAsync(
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
        _logger.LogInformation("{UserEdited} have been edited by {AppUserId}", userToUpdate?.UserName, appUserId);


        return View(userToUpdate);
    }

    public ActionResult EditUsers()
    {
        var appUserId = User.Identity.GetUserId();
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
        if (await TryUpdateModelAsync(
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

    public IActionResult ResetLegacyData()
    {
        AppSetup.DeleteDb();
        AppSetup.SeedDb();
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
    public List<AppUser>? Users { get; set; }
}