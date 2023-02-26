using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Repository;

public interface IUserRepository : IDisposable
{
    IEnumerable<User> GetUsers();
    User GetUserByID(String userId);
    void InsertUser(User user);
    void DeleteUser(String userId);
    void UpdateUser(User user);
    void Save();
}
public class UserRepository : IUserRepository
{
    private ShotsContextFinal context;

    public UserRepository(ShotsContextFinal context)
    {
        this.context = context;
        var races = this.context.RaceModel.ToList();
        var shots = this.context.ShotModel.ToList();
    }

    public IEnumerable<User> GetUsers()
    {
        return context.UserModel.ToList();
    }

    public User GetUserByID(string userId)
    {
        return context.UserModel.Find(userId);
    }
    
    public List<Race> GetUserRacesByID(string userId)
    {
        var result =  context.UserModel.Find(userId).Race.ToList();
        return result;
    }
    
    public List<Shot> GetUserShotsByID(string userId, int raceId)
    {
        var result =  context.UserModel.Find(userId).Race.Find(x => x.Id == raceId).Shot;
        return result;
    }

    public void InsertUser(User user)
    {
        context.UserModel.Add(user);
    }

    public void DeleteUser(string userId)
    {
        var user = context.UserModel.Find(userId);
        context.UserModel.Remove(user);
    }

    public void UpdateUser(User user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    public void Save()
    {
        context.SaveChanges();
    }
    
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}