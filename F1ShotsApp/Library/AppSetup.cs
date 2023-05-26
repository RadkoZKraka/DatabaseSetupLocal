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
        using var db = new ShotsContext();
        db.Database.EnsureDeleted();
    }

    public static bool GetDarkModeEnabledStatus()
    {
        return true;
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
            var currentUser = new UserShots
            {
                UserName = user.Key.ToUpper(),
                Race = new List<Race>()
            };
            var raceNo = 1;
            var races = user.Value.Skip(1);
            foreach (var race in races)
            {
                var currentRace = new Race
                {
                    Shot = new List<Shot>(),
                    RaceYear = 2022,
                    RaceNo = raceNo
                };

                foreach (var shot in race)
                {
                    currentRace.RaceLocation = shot.Key.ToUpper();
                    currentRace.PolePosition =
                        !shot.Value.Skip(20).Any() || string.IsNullOrEmpty(shot.Value.Skip(20).First())
                            ? "EMPTY"
                            : shot.Value.Skip(20).First().ToUpper();
                    currentRace.Rand =
                        !shot.Value.Skip(21).Any() || string.IsNullOrEmpty(shot.Value.Skip(21).First())
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

    private static IEnumerable<Race> SeedForNewSeason()
    {
        var f1Schedule = F1WebScraper.GetScheduleData();
        var raceListToSeed = new List<Race>();
        var i = 1;
        foreach (var raceSchedule in f1Schedule.Races)
        {
            var raceToSeed = new Race
            {
                RaceYear = 2023,
                Shot = new List<Shot>()
            };
            for (var j = 0; j < 20; j++)
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
        if (File.Exists(file)) return;
        var grid = F1WebScraper.GetDriversData(year);
        var serializer = new JsonSerializer();
        using var sw = new StreamWriter(file);
        using JsonWriter writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, grid);
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

            if (f1Grid == null)
            {
                return res;
            }
            res.Add(f1Grid.Drivers.FirstOrDefault(x => x.Abbreviation == s)!.FullName);
        }

        return res;
    }
    public static string? AbrOneDriverToFullName(string abr, int year)
    {
        var f1Grid = DeserializeDrivers(year);

        var res = f1Grid?.Drivers.First(x => x.Abbreviation == abr).FullName;

        return res;
    }

    public static void SerializeDates()
    {
        const string file = "dates.json";
        if (File.Exists(file)) return;
        var scheduleData = F1WebScraper.GetScheduleData();
        var serializer = new JsonSerializer();
        using var sw = new StreamWriter(file);
        using var writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, scheduleData);
    }

    public static F1Schedule? DeserializeDates()
    {
        const string file = "dates.json";
        var f1Schedule = JsonConvert.DeserializeObject<F1Schedule>(File.ReadAllText(file));

        return f1Schedule;
    }

    public static string GetCurrentRaceLocation()
    {
        var f1Schedule = DeserializeDates();
        if (f1Schedule != null)
        {
            var listOfDates = f1Schedule.Races
                .Select(x => x.F1Events.First(f1Event => f1Event.EventName == "Qualifying").EventDateAndTime).ToList();

            var closest = GetNextDateTime(listOfDates);
            var closestRaceName = f1Schedule.Races
                .First(x => x.F1Events.Any(f1Event => f1Event.EventDateAndTime == closest)).RaceName;
            return closestRaceName;
        }

        return "";
    }

    public static List<int> CalculateCumulativeSum(IEnumerable<int> numbers)
    {
        var cumulativeSumList = new List<int>();
        var sum = 0;

        foreach (var num in numbers)
        {
            sum += num;
            cumulativeSumList.Add(sum);
        }

        return cumulativeSumList;
    }

    public static void LockPreviousRaces()
    {
        var listOfLocations = DeserializeDates()?.Races.Select(x => x.RaceName).ToList();
        var times = AppSetup.DeserializeDates()
            ?.Races
            .Select(raceSchedule => raceSchedule.F1Events.Where(f1Event => f1Event.EventName == "Qualifying")
                .Select(f1Event => f1Event.EventDateAndTime).ToList())
            .ToList().SelectMany(x => x);
        if (times == null) return;
        var racesPassed = GetNumberOfPassedDates(times.ToList());
        if (listOfLocations == null) return;
        foreach (var location in listOfLocations.Take(racesPassed))
        {
            var shotsRepo = new ShotsRepository(new ShotsContext(), new LoggingService());
            shotsRepo.LockRace(DateTime.Now.Year, location);
        }
    }

    private static int GetNumberOfPassedDates(IEnumerable<DateTime> dateTimes)
    {
        var now = DateTime.Now;
        return dateTimes.Count(dt => dt < now);
    }

    public static RaceSchedule? GetCurrentRaceSchedule()
    {
        var f1Schedule = DeserializeDates();
        var listOfDates = f1Schedule?.Races
            .Select(x => x.F1Events.First(f1Event => f1Event.EventName == "Qualifying").EventDateAndTime).ToList();

        if (listOfDates == null) return null;
        {
            var closestDate = GetNextDateTime(listOfDates);
            var closestRace = f1Schedule?.Races
                .First(raceSchedule => raceSchedule.F1Events.Any(f1Event => f1Event.EventDateAndTime == closestDate));
            return closestRace;
        }
    }

    public static RaceSchedule? GetRaceScheduleBy(string raceName)
    {
        var f1Schedule = DeserializeDates();
        var listOfDates = f1Schedule?.Races
            .Where(raceSchedule => raceSchedule.RaceName == raceName).Select(raceSchedule =>
                raceSchedule.F1Events.First(f1Event => f1Event.EventName == "Qualifying").EventDateAndTime).ToList();

        if (listOfDates == null || listOfDates.Count == 0) return null;
        {
            var closestDate = GetNextDateTime(listOfDates);

            var closestRace = f1Schedule?.Races
                .First(raceSchedule => raceSchedule.F1Events.Any(f1Event => f1Event.EventDateAndTime == closestDate));
            return closestRace;
        }
    }

    public static async Task ScheduleTasks()
    {
        var taskTimes = new List<DateTime>();
        taskTimes.AddRange(AppSetup.DeserializeDates()
            ?.Races
            .Select(raceSchedule => raceSchedule.F1Events.Where(f1Event => f1Event.EventName == "Qualifying")
                .Select(f1Event => f1Event.EventDateAndTime).ToList())
            .ToList().SelectMany(dateTimes => dateTimes) ?? Array.Empty<DateTime>());

        var currentRaceLocation = AppSetup.GetCurrentRaceLocation();
        var currentYear = DateTime.Now.Year;

        await ScheduleTasksAtSpecifiedTimes(taskTimes, async () =>
        {
            Console.WriteLine("It went off!");
            var shotsRepo = new ShotsRepository(new ShotsContext(), new LoggingService());
            await shotsRepo.LockRaceAsync(currentYear, currentRaceLocation);
            // Call your method that saves the form here.
        });
    }

    private static Task ScheduleTasksAtSpecifiedTimes(List<DateTime> times, Func<Task> taskFunc)
    {
        var nextTaskTime = times.OrderBy(t => t).FirstOrDefault(t => t > DateTime.Now);
        if (nextTaskTime == default)
        {
            return Task.CompletedTask;
        }

        // Calculate the delay until the next task time
        var delayTime = nextTaskTime - DateTime.Now;
        var timer = new System.Timers.Timer(delayTime.TotalMilliseconds);
        timer.AutoReset = false;

        // Schedule the next task
        timer.Elapsed += async (_, _) =>
        {
            await taskFunc();

            // Schedule the next task recursively
            await ScheduleTasksAtSpecifiedTimes(times, taskFunc);
        };

        timer.Start();
        return Task.CompletedTask;
    }

    private static DateTime GetNextDateTime(IReadOnlyCollection<DateTime> dateTimes)
    {
        var now = DateTime.Now;
        var closest = dateTimes.OrderBy(dt => dt).FirstOrDefault(dt => dt > now);

        if (closest == DateTime.MinValue)
        {
            if (dateTimes.Count == 0)
            {
                return DateTime.Now;
            }
            closest = dateTimes.First();
        }
        return closest;
    }

    public static UserShots SetupShotsForNewUser(string ownerId, string fullName)
    {
        //TODO: also adds for year 2022 to fix
        var userShots = new UserShots
        {
            OwnerId = ownerId,
            Race = new List<Race>()
        };
        userShots.Race.AddRange(SeedForNewSeason());
        userShots.UserName = fullName;
        return userShots;
    }
}