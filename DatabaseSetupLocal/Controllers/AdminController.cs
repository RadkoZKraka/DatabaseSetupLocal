using System.Runtime.InteropServices.JavaScript;
using DatabaseSetupLocal.Areas.Identity.Data;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Rep;
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

    public AdminController(ILogger<AdminController> logger)
    {
        _userRepository = new UserRepository(new UsersContext());
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
    public void DownloadDatesListOfRaces()
    {
        var year = DateTime.Now.Year;
        F1WebScraper.GetDatesListOfRaces(year);
    }

    public void DownloadResultOfRaces(int year, int raceNumber)
    {
        var raceResults = F1WebScraper.GetRaceResults(year, raceNumber);
    }
    public void GetDriversData()
    {
        
        F1WebScraper.GetDriversData();
    }
    public void GetScheduleData()
    {
        
        F1WebScraper.GetScheduleData();
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