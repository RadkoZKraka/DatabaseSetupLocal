using System.Runtime.InteropServices.JavaScript;
using DatabaseSetupLocal.Rep;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseSetupLocal.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
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
}