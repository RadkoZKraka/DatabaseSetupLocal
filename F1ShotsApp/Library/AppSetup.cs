using System.Text.RegularExpressions;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Models;
using DatabaseSetupLocal.Repository;
using Newtonsoft.Json;

namespace DatabaseSetupLocal.Library;

public static class AppSetup
{
    public static void DeleteDb()
    {
        using (var db = new ShotsContext())
        {
            db.Database.EnsureDeleted();
        }
    }
    public static void SeedDb()
    {
        using (var db = new ShotsContext())
        {
            // db.Database.EnsureDeleted();
            // db.Database.EnsureCreated();
            // db.Database.Migrate();
            if (db.UserShotsModel.Any())
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
            var races = user.Value.Skip(1);
            foreach (var race in races)
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
                context.UserShotsModel.Add(user);
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

    public static void SerializeDrivers(int year)
    {
        var file = $"drivers{year}.json";
        if (!File.Exists(file))

        {
            var grid = F1WebScraper.GetDriversData(year);
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(file))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, grid);
            }
        }
    }

    public static F1Grid? DeserializeDrivers(int year)
    {
        var file = $"drivers{year}.json";
        var f1Grid = JsonConvert.DeserializeObject<F1Grid>(File.ReadAllText(file));

        return f1Grid;
    }

    public static List<string> AbrListToFullName(List<string> abr, int year)
    {
        var f1Grid = DeserializeDrivers(year);
        var res = new List<string>();

        foreach (var s in abr)
        {
            if (s.Length != 3)
            {
                continue;
            }
            res.Add(f1Grid.Drivers.Where(x => x.Abbreviation == s).First().FullName);
        }

        return res;
    }
    public static string AbrOneDriverToFullName(string abr, int year)
    {
        var f1Grid = DeserializeDrivers(year);

        var res = f1Grid.Drivers.Where(x => x.Abbreviation == abr).First().FullName;

        return res;
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

    public static string GetCurrentRaceLocation()
    {
        var f1Schedule = DeserializeDates();
        var listOfDates = f1Schedule.Races
            .Select(x => x.F1Events.Where(x => x.EventName == "Qualifying").First().EventDateAndTime).ToList();

        var closest = GetNextDateTime(listOfDates);
        var closestRaceName = f1Schedule.Races.Where(x => x.F1Events.Where(x => x.EventDateAndTime == closest).Any())
            .First().RaceName;
        return closestRaceName;
    }

    public static void LockPreviousRaces()
    {
        
        var listOfLocations = DeserializeDates().Races.Select(x => x.RaceName).ToList();
        var times = AppSetup.DeserializeDates().Races
            .Select(x => x.F1Events.Where(x => x.EventName == "Qualifying").Select(x => x.EventDateAndTime).ToList())
            .ToList().SelectMany(x => x);
        var racesPassed = GetNumberOfPassedDates(times.ToList());
        foreach (var location in listOfLocations.Take(racesPassed))
        {
            var shotsRepo = new ShotsRepository(new ShotsContext());
            shotsRepo.LockRace(DateTime.Now.Year, location);
        }
    }
    
    public static int GetNumberOfPassedDates(List<DateTime> dateTimes)
    {
        DateTime now = DateTime.Now;
        return dateTimes.Count(dt => dt < now);
    }

    public static RaceSchedule GetCurrentRaceSchedule()
    {
        var f1Schedule = DeserializeDates();
        var listOfDates = f1Schedule.Races
            .Select(x => x.F1Events.Where(x => x.EventName == "Qualifying").First().EventDateAndTime).ToList();

        var closestDate = GetNextDateTime(listOfDates);
        var closestRace = f1Schedule.Races.Where(x => x.F1Events.Where(x => x.EventDateAndTime == closestDate).Any())
            .First();
        return closestRace;
    }

    public static async Task ScheduleTasks()
    {
        var taskTimes = new List<DateTime>();
        taskTimes.AddRange(AppSetup.DeserializeDates().Races
            .Select(x => x.F1Events.Where(x => x.EventName == "Qualifying").Select(x => x.EventDateAndTime).ToList())
            .ToList().SelectMany(x => x));

        var currentRaceLocation = AppSetup.GetCurrentRaceLocation();
        var currentYear = DateTime.Now.Year;

        await ScheduleTasksAtSpecifiedTimes(taskTimes, async () =>
        {
            Console.WriteLine("It went off!");
            var shotsRepo = new ShotsRepository(new ShotsContext());
            await shotsRepo.LockRaceAsync(currentYear, currentRaceLocation);
            // Call your method that saves the form here.
        });
    }

    public static async Task ScheduleTasksAtSpecifiedTimes(List<DateTime> times, Func<Task> taskFunc)
    {
        var nextTaskTime = times.OrderBy(t => t).FirstOrDefault(t => t > DateTime.Now);
        if (nextTaskTime == default)
        {
            return;
        }

        // Calculate the delay until the next task time
        var delayTime = nextTaskTime - DateTime.Now;
        var timer = new System.Timers.Timer(delayTime.TotalMilliseconds);
        timer.AutoReset = false;

        // Schedule the next task
        timer.Elapsed += async (sender, e) =>
        {
            await taskFunc();

            // Schedule the next task recursively
            await ScheduleTasksAtSpecifiedTimes(times, taskFunc);
        };

        timer.Start();
    }

    public static DateTime GetNextDateTime(List<DateTime> dateTimes)
    {
        DateTime now = DateTime.Now;
        return dateTimes.OrderBy(dt => dt).FirstOrDefault(dt => dt > now);
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