using System.ComponentModel;
using System.Text.RegularExpressions;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
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

        var legacyData = LegacyData.GetData();
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
            foreach (var u in userList)
            {
                u.Race.AddRange(SeedForNewSeason());
            }

            foreach (var user in userList)
            {
                context.UserModel.Add(user);
            }

            context.SaveChanges();
        }
    }

    public static List<Race> SeedForNewSeason()
    {
        var f1Schedule = F1WebScraper.GetScheduleData();
        var raceListToSeed = new List<Race>();
        var i = 1;
        foreach (var raceSchedule in f1Schedule.Races)
        {
            var raceToSeed = new Race();
            raceToSeed.RaceYear = 2023;
            raceToSeed.Shot = new List<Shot>();
            for (int j = 0; j < 20; j++)
            {
                raceToSeed.Shot.Add(new Shot());
            }

            raceToSeed.RaceNo = i;
            raceToSeed.RaceLocation = raceSchedule.RaceName.Contains("NEXT")
                ? raceSchedule.RaceName.Replace("NEXT", "")
                : raceSchedule.RaceName;
            raceListToSeed.Add(raceToSeed);
            i++;
        }

        return raceListToSeed;
    }

    public static void Test()
    {
        var repo = new ShotsRepository(new ShotsContext());
        var f1Schedule = F1WebScraper.GetScheduleData();
        var raceListToSeed = new List<Race>();
        var users = repo.GetUsers();
        foreach (var raceSchedule in f1Schedule.Races)
        {
            var raceToSeed = new Race();
            raceToSeed.RaceYear = 2023;
            raceToSeed.RaceLocation = raceSchedule.RaceName;
            raceListToSeed.Add(raceToSeed);
        }

        foreach (var user in users)
        {
            user.Race.AddRange(raceListToSeed);
            repo.UpdateUser(user);
        }
    }

    public static void SerializeDrivers()
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

    public static F1Grid? DeserializeDrivers()
    {
        var file = "drivers.json";
        var f1Grid = JsonConvert.DeserializeObject<F1Grid>(File.ReadAllText(file));

        return f1Grid;
    }

    public static void SerializeDates()
    {
        var file = "dates.json";
        if (!File.Exists(file))

        {
            var scheduleData = F1WebScraper.GetScheduleData();
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, scheduleData);
            }
        }
    }
}