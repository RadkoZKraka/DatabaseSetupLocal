using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Exceptions;
using DatabaseSetupLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Repository;

public interface IShotRepository : IDisposable
{
    IEnumerable<UserShots> GetUsers();
    UserShots GetUserById(String userId);
    void InsertUser(UserShots userShots);
    void DeleteUser(String userId);
    void UpdateUser(UserShots userShots);
    void Save();
}

public class ShotsRepository : IShotRepository
{
    private ShotsContext _shotsContext;

    public ShotsRepository(ShotsContext shotsContext)
    {
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
        return _shotsContext.UserModel.ToList();
    }

    public UserShots GetUserById(string userId)
    {
        return _shotsContext.UserModel.Find(userId);
    }
    public string GetUserIdByOwnerId(string ownerId)
    {
        return _shotsContext.UserModel.Where(x => x.OwnerId == ownerId).First().Id;
    }
    public UserShots GetUserByOwnerId(string userId)
    {
        return _shotsContext.UserModel.Where(x => x.OwnerId == userId).FirstOrDefault();
    }
    public bool GetIfAppUserHasShots(string userId)
    {
        if (_shotsContext.UserModel.Where(x => x.OwnerId == userId).FirstOrDefault() == null)
        {
            return false;
        }
        return true;
    }

    public int? GetRaceIdByRaceLoc(string userId,string raceLoc)
    {
        return _shotsContext.UserModel.Find(userId)?.Race.Find(x => x.RaceLocation == raceLoc).Id;

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
        var result = _shotsContext.UserModel.Find(userId)?.Race.ToList();
        return result;
    }
    public List<int> GetUsersYears(string userId)
    {
        var yearsList = _shotsContext.UserModel.Find(userId)?.Race.Select(x => x.RaceYear).Distinct().ToList() 
                        ?? throw new YearListDoesntExistException();
        return yearsList;
    }
    public List<int> GetUserPointsByYear(string userId, int year)
    {
        var userPointsByYear = _shotsContext.UserModel.Find(userId)?.Race.Select(x => x.Points);
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
        var listOfRaces = _shotsContext.RaceModel.Where(x => x.RaceYear == year).Select(x => x.RaceLocation).ToList();
        return listOfRaces;
    }

    public List<Shot>? GetUserShotsByUserIdAndRaceId(string userId, int raceId)
    {
        var result = _shotsContext.UserModel.Find(userId)?.Race.Find(x => x.Id == raceId)?.Shot;
        return result;
    }
    public List<Shot>? GetUserShotsByUserIdAndRaceLoc(string userId, string raceLoc)
    {
        var result = _shotsContext.UserModel.Find(userId)?.Race.Find(x => x.RaceLocation == raceLoc)?.Shot;
        return result;
    }

    public void InsertUser(UserShots userShots)
    {
        _shotsContext.UserModel.Add(userShots);
        _shotsContext.SaveChanges();
    }

    public void DeleteUser(string userId)
    {
        var user = _shotsContext.UserModel.Find(userId);
        _shotsContext.UserModel.Remove(user);
        _shotsContext.SaveChanges();

    }
    public void HideUser(string userId)
    {
        var user = _shotsContext.UserModel.Find(userId);
        _shotsContext.UserModel.Find(userId);
        _shotsContext.SaveChanges();

    }

    public void UpdateUser(UserShots userShots)
    {
        _shotsContext.Entry(userShots).State = EntityState.Modified;
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