using DatabaseSetupLocal.Areas.Identity.Data;
using DatabaseSetupLocal.Data;

namespace DatabaseSetupLocal.Repository;

public class UserRepository
{
    public UsersContext _usersContext;

    public UserRepository(UsersContext usersContext)
    {
        this._usersContext = usersContext;
    }

    public AppUser GetUserById(string userId)
    {
        return _usersContext.UserModel.Find(userId);
    }

    public List<AppUser> GetUsers()
    {
        return _usersContext.UserModel.ToList();
    }

    public void DeleteUser(string userId)
    {
        var user = _usersContext.UserModel.Find(userId);
        _usersContext.UserModel.Remove(user);
        _usersContext.SaveChanges();
    }

    public bool GetIfUserIsAdminById(string userId)
    {
        var user = _usersContext.UserModel.Find(userId);
        if (user == null)
        {
            return false;
        }

        return user.Admin;
    }
}