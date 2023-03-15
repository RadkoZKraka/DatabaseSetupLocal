using DatabaseSetupLocal.Data;
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
    
    public List<Race> GetUserRacesById(string userId)
    {
        var result = _shotsContext.UserModel.Find(userId)?.Race.ToList();
        return result;
    }
    public List<int> GetYears()
    {
        var result = _shotsContext.RaceModel.Select(x => x.RaceYear).Distinct().ToList();
        return result;
    }

    public List<Shot>? GetUserShotsById(string userId, int raceId)
    {
        var result = _shotsContext.UserModel.Find(userId)?.Race.Find(x => x.Id == raceId)?.Shot;
        return result;
    }

    public void InsertUser(UserShots userShots)
    {
        _shotsContext.UserModel.Add(userShots);
    }

    public void DeleteUser(string userId)
    {
        var user = _shotsContext.UserModel.Find(userId);
        _shotsContext.UserModel.Remove(user);
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