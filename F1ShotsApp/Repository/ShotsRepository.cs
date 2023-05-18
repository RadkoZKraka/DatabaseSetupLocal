using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Exceptions;
using DatabaseSetupLocal.Library;
using DatabaseSetupLocal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DatabaseSetupLocal.Repository;

public interface IShotRepository : IDisposable
{
    IEnumerable<UserShots> GetUsers();
    UserShots GetUserById(String userId);
    void InsertUser(UserShots userShots);
    void DeleteUser(String userId);
    void UpdateUser(UserShots userShots, string user);
    void Save();
}

public class ShotsRepository : IShotRepository
{
    private ShotsContext _shotsContext;
    private LoggingService _loggingService;

    public ShotsRepository(ShotsContext shotsContext, LoggingService loggingService)
    {
        _loggingService = loggingService;
        this._shotsContext = shotsContext;
        //TODO: rozkminić dlaczego to jest potrzebne, inaczej nie wchodzi w Racey w Userze
        var races = this._shotsContext.RaceModel.ToList();
        var shots = this._shotsContext.ShotModel.ToList();
    }

    public ShotsContext GetShotsContext()
    {
        return _shotsContext;
    }

    public IEnumerable<UserShots> GetUsers()
    {
        return _shotsContext.UserShotsModel.ToList();
    }
    public IEnumerable<Race> GetRaces()
    {
        return _shotsContext.RaceModel.ToList();
    }
    public Race GetCurrentRace(string ownerId)
    {
        var currentLocation = AppSetup.GetCurrentRaceLocation();
        var race = _shotsContext.UserShotsModel.Where(x => x.OwnerId == ownerId).FirstOrDefault().Race
            .Where(x => x.RaceLocation == currentLocation).FirstOrDefault();
        return race;
    }
    public int GetCurrentRaceNo()
    {
        var currentLocation = AppSetup.GetCurrentRaceLocation();
        var raceNo = _shotsContext.UserShotsModel.FirstOrDefault().Race
            .Where(x => x.RaceLocation == currentLocation).FirstOrDefault().RaceNo;
        return raceNo;
    }

    public UserShots GetUserById(string userId)
    {
        return _shotsContext.UserShotsModel.Find(userId);
    }
    public string GetUserIdByRaceId(int raceId)
    {
        return _shotsContext.UserShotsModel.Where(x => x.Race.Where(r => r.Id == raceId).First() != null).First().Id;
    }


    public string GetUserIdByOwnerId(string ownerId)
    {
        return _shotsContext.UserShotsModel.Where(x => x.OwnerId == ownerId).First().Id;
    }

    public UserShots GetUserByOwnerId(string userId)
    {
        return _shotsContext.UserShotsModel.Where(x => x.OwnerId == userId).FirstOrDefault();
    }
    public void SumPointsInRace(Race race)
    {
        var points = 0;
        foreach (var shot in race.Shot)
        {
            points += shot.Points;
        }
        race.Points = points;
        UpdateRace(race, "System");
    }

    public bool GetIfAppUserHasShots(string userId)
    {
        if (_shotsContext.UserShotsModel.Where(x => x.OwnerId == userId).FirstOrDefault() == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> LockRaceAsync(int raceYear, string raceLoc)
    {
        var races = _shotsContext.RaceModel.Where(x => x.RaceLocation == raceLoc && x.RaceYear == raceYear).ToList();
        try
        {
            foreach (var race in races)
            {
                race.Locked = true;
                UpdateRace(race, "System");
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
            return false;
        }
    }

    public void LockRace(int raceYear, string raceLoc)
    {
        var races = _shotsContext.RaceModel.Where(x => x.RaceLocation == raceLoc && x.RaceYear == raceYear).ToList();
        foreach (var race in races)
        {
            race.Locked = true;
            UpdateRace(race, "System");
        }
    }

    public void LockYear(int raceYear)
    {
        var races = _shotsContext.RaceModel.Where(x => x.RaceYear == raceYear).ToList();
        races.ForEach(x => x.Locked = true);
        foreach (var raceLocked in races)
        {
            UpdateRace(raceLocked, "System");
        }
    }

    public void CountPointsByRace(Race race)
    {
        var results = F1WebScraper.GetRaceResults(race.RaceYear, race.RaceNo);
        var polePosition = F1WebScraper.GetPoleSitter(race.RaceYear, race.RaceNo);
        var fastestLap = F1WebScraper.GetFastestLap(race.RaceYear, race.RaceNo);
        var usersShots = race.Shot.Select(x => x.UsersShotDriver).ToList();
        var fullNameResult = AppSetup.AbrListToFullName(results, race.RaceYear);
        for (int i = 0; i < usersShots.Count; i++)
        {
            if (usersShots[i] == null || usersShots[i].Length != 3)
            {
                continue;
            }
            usersShots[i] = AppSetup.AbrOneDriverToFullName(usersShots[i], race.RaceYear);
        }

        var listOfPoints = CalculateUsersPoints(usersShots, fullNameResult);
        for (int i = 0; i < listOfPoints.Count; i++)
        {
            race.Shot[i].Points = listOfPoints[i];
            race.Shot[i].ResultDriver = results[i];
        }

        if (!string.IsNullOrEmpty(race.FastestLap) && race.FastestLap.Length == 3)
        {
            race.FastestLap = AppSetup.AbrOneDriverToFullName(race.FastestLap, race.RaceYear);
        }
        if (!string.IsNullOrEmpty(race.PolePosition) && race.PolePosition.Length == 3)
        {
            race.PolePosition = AppSetup.AbrOneDriverToFullName(race.PolePosition, race.RaceYear);
        }
        
        if (race.FastestLap == AppSetup.AbrOneDriverToFullName(fastestLap, race.RaceYear))
        {
            race.FastestLapPoints = 2;
        }
        if (race.PolePosition == AppSetup.AbrOneDriverToFullName(polePosition, race.RaceYear))
        {
            race.FastestLapPoints = 2;
        }


        UpdateRace(race , "System");
    }

    public List<int> CalculateUsersPoints(List<string> listA, List<string> listB)
    {
        var result = new List<int>();

        for (int i = 0; i < listA.Count; i++)
        {
            if (i == 0)
            {
                if (listA[i] == listB[i])
                {
                    result.Add(3);
                    continue;
                }

                if (listA[i + 1] == listB[i])
                {
                    result.Add(1);
                    continue;
                }
            }

            if (i == listA.Count - 1)
            {
                if (listA[i] == listB[i])
                {
                    result.Add(3);
                    continue;
                }

                if (listA[i - 1] == listB[i])
                {
                    result.Add(1);
                    continue;
                }

                result.Add(0);
                break;
            }

            if (i > 0)
            {
                if (listA[i] == listB[i])
                {
                    result.Add(3);
                    continue;
                }

                if (listA[i + 1] == listB[i])
                {
                    result.Add(1);
                    continue;
                }

                if (listA[i - 1] == listB[i])
                {
                    result.Add(1);
                    continue;
                }
            }

            result.Add(0);
        }

        return result;
    }

    public bool UpdateRacePoints(Race race, List<string> results)
    {
        for (int i = 0; i < race.Shot.Count; i++)
        {
            race.Shot[i].ResultDriver = results[i];
        }

        return true;
    }

    public int? GetRaceIdByRaceLoc(string userId, string raceLoc)
    {
        return _shotsContext.UserShotsModel.Find(userId)?.Race.Find(x => x.RaceLocation == raceLoc).Id;
    }

    public Race? GetRaceById(int raceId)
    {
        var race = _shotsContext.RaceModel.Find(raceId);
        if (race == null)
        {
            throw new RaceDoesntExistException();
        }

        return race;
    }

    public int GetRaceYearById(int raceId)
    {
        var race = _shotsContext.RaceModel.Find(raceId);
        if (race == null)
        {
            throw new RaceDoesntExistException();
        }

        return race.RaceYear;
    }

    public List<Race> GetUserRacesById(string userId)
    {
        var result = _shotsContext.UserShotsModel.Find(userId)?.Race.ToList();
        return result;
    }

    public List<int> GetUsersYears(string userId)
    {
        var yearsList = _shotsContext.UserShotsModel.Find(userId)?.Race.Select(x => x.RaceYear).Distinct().ToList()
                        ?? throw new YearListDoesntExistException();
        return yearsList;
    }
    public List<int> GetYears()
    {
        var yearsList = _shotsContext.UserShotsModel.First().Race.Select(x => x.RaceYear).Distinct().ToList();
        return yearsList;
    }

    public List<int> GetUserPointsByYear(string userId, int year)
    {
        var userPointsByYear = _shotsContext.UserShotsModel.Find(userId)?.Race.Select(x => x.Points);
        var result = new List<int>();
        var temp = 0;
        foreach (var points in userPointsByYear)
        {
            result.Add(temp + points);
        }

        return result;
    }

    public List<String> GetListOfRaceLocations(int year)
    {
        var listOfRaces = _shotsContext.RaceModel.Where(x => x.RaceYear == year).Select(x => x.RaceLocation)
            .ToList();
        return listOfRaces;
    }

    public List<Shot>? GetUserShotsByUserIdAndRaceId(string userId, int raceId)
    {
        var result = _shotsContext.UserShotsModel.Find(userId)?.Race.Find(x => x.Id == raceId)?.Shot;
        return result;
    }

    public List<Shot>? GetUserShotsByUserIdAndRaceLoc(string userId, string raceLoc)
    {
        var result = _shotsContext.UserShotsModel.Find(userId)?.Race.Find(x => x.RaceLocation == raceLoc)?.Shot;
        return result;
    }
    public void ChangeAllAbrToFullNameInARace(Race race)
    {
        
        foreach (var shot in race.Shot)
        {
            if (shot.UsersShotDriver == null || shot.UsersShotDriver.Length != 3)
            {
                continue;
            }
            shot.UsersShotDriver = AppSetup.AbrOneDriverToFullName(shot.UsersShotDriver, race.RaceYear);
        }
        UpdateRace(race, "System");
    }

    public void InsertUser(UserShots userShots)
    {
        _shotsContext.UserShotsModel.Add(userShots);
        _shotsContext.SaveChanges();
    }

    public void DeleteUser(string userId)
    {
        var user = _shotsContext.UserShotsModel.Find(userId);
        _shotsContext.UserShotsModel.Remove(user);
        _shotsContext.SaveChanges();
    }

    public void HideUser(string userId, string userThatChanged)
    {
        var user = _shotsContext.UserShotsModel.Find(userId);

        user.Hidden = true;
        UpdateUser(user, userThatChanged);
    }

    public void ShowUser(string userId, string userThatChanged)
    {
        var user = _shotsContext.UserShotsModel.Find(userId);

        user.Hidden = false;
        UpdateUser(user, userThatChanged);
    }

    public async void UpdateUser(UserShots userShots, string user)
    {
        _shotsContext.Entry(userShots).State = EntityState.Modified;
        _shotsContext.SaveChangesAsync();
        await LogAction("User " + userShots.UserName + " has been updated by " + user + ".\n");
    }
    public async Task LogAction(string logMessage)
    {
        await _loggingService.AppendToLogFile(logMessage);

        // Your other controller logic here
    }

    public async void UpdateRace(Race race, string user)
    {
        _shotsContext.RaceModel.Entry(race).State = EntityState.Modified;
        _shotsContext.SaveChangesAsync();

        await LogAction("Race with ID: " + race.Id + " has been updated by " + user + ".\n");

    }

    public void Save()
    {
        _shotsContext.SaveChanges();
    }

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                _shotsContext.Dispose();
            }
        }

        this._disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}