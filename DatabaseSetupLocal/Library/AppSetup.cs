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
            // db.Database.EnsureDeleted();
            // db.Database.EnsureCreated();
            // db.Database.Migrate();
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

    public static F1Schedule? DeserializeDates()
    {
        var file = "dates.json";
        var f1Schedule = JsonConvert.DeserializeObject<F1Schedule>(File.ReadAllText(file));

        return f1Schedule;
    }

    public static string GetCurrentRace()
    {
        var f1Schedule = DeserializeDates();
        var listOfDates = f1Schedule.Races
            .Select(x => x.F1Events.Where(x => x.EventName == "Qualifying").First().EventDateAndTime).ToList();

        var closest = ReturnClosest(listOfDates);
        var closestRaceName = f1Schedule.Races.Where(x => x.F1Events.Where(x => x.EventDateAndTime == closest).Any())
            .First().RaceName;
        return closestRaceName;
    }

    public static DateTime ReturnClosest(List<DateTime> dateTimes)
    {
        //Establish Now for simpler code below
        var now = DateTime.Now;

        //Start by assuming the first in the list is the closest
        var closestDateToNow = dateTimes[0];

        //Set up initial interval value, accounting for dates before and after Now
        TimeSpan shortestInterval = now > dateTimes[0] ? now - dateTimes[0] : dateTimes[0] - now;

        //Loop through the rest of the list and correct closest item if appropriate
        //Note this starts at index 1, which is the SECOND value, we've evaluated the first above
        for (var i = 1; i < dateTimes.Count; i++)
        {
            TimeSpan testinterval = now > dateTimes[i] ? now - dateTimes[i] : dateTimes[i] - now;

            if (testinterval < shortestInterval)
            {
                shortestInterval = testinterval;
                closestDateToNow = dateTimes[i];
            }
        }

        //return the closest Datetime object
        return closestDateToNow;
    }

    public static UserShots SetupShotsForNewUser(string ownerId, string fullName)
    {
        //TODO: also adds for year 2022 to fix
        var userShots = new UserShots();
        userShots.OwnerId = ownerId;
        userShots.Race = new List<Race>();
        userShots.Race.AddRange(SeedForNewSeason());
        userShots.UserName = fullName;
        return userShots;
    }
}