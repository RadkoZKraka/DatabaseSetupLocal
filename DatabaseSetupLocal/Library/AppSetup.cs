using System.ComponentModel;
using System.Text.RegularExpressions;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace DatabaseSetupLocal.Rep;

public static class AppSetup
{
    public static void SeedDb()
    {
        using (var db = new ShotsContext())
        {
            var test = db.RaceModel.ToList();
            db.Database.Migrate();
            if (db.UserModel.Any())
            {
                return;
            }
        }
        
        var legacyData = GetLegacyData.GetData();
        var userList = new List<UserShots>();
        foreach (var user in legacyData)
        {
            var currentUser = new UserShots();
            currentUser.UserName = user.Key.ToUpper();
            currentUser.Race = new List<Race>();
            var raceNo = 1;
            foreach (var race in user.Value)
            {
                var currentRace = new Race();
                currentRace.Shot = new List<Shot>();
                currentRace.RaceYear = 2022;
                currentRace.RaceNo = raceNo;

                foreach (var shot in race)
                {
                    currentRace.RaceLocation = shot.Key.ToUpper();
                    currentRace.PolePosition =
                        shot.Value.Skip(20).Count() == 0 || string.IsNullOrEmpty(shot.Value.Skip(20).First())
                            ? "EMPTY"
                            : shot.Value.Skip(20).First().ToUpper();
                    currentRace.Rand =
                        shot.Value.Skip(21).Count() == 0 || string.IsNullOrEmpty(shot.Value.Skip(21).First())
                            ? "EMPTY"
                            : shot.Value.Skip(21).First().ToUpper();
                    foreach (var driver in shot.Value.Take(20))
                    {
                        var currentShot = new Shot();
                        string result = Regex.Replace(string.IsNullOrEmpty(driver) ? "Empty" : driver.ToUpper(),
                            @"[^a-zA-Z]", "");
                        currentShot.UsersShotDriver = result;
                        currentRace.Shot.Add(currentShot);
                    }
                }

                currentUser.Race.Add(currentRace);
                raceNo++;
            }

            userList.Add(currentUser);
        }

        using (var context = new ShotsContext())
        {
            foreach (var user in userList)
            {
                context.UserModel.Add(user);
            }

            context.SaveChanges();
        }
    }

    public static void GetDrivers()
    {
        var file = "drivers.json";
        if (!File.Exists(file))
        {
            var grid = F1WebScraper.GetDriversData();
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, grid);
            }
        }
    }
    
    public static void GetDates()
    {
        var file = "drivers.json";
        if (!File.Exists(file))
        {
            var grid = F1WebScraper.GetDriversData();
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, grid);
            }
        }
    }
}