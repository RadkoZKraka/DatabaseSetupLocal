using DatabaseSetupLocal.Areas.Identity.Data;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Exceptions;

namespace DatabaseSetupLocal.Repository;

public class UserRepository
{
    public readonly UsersContext _usersContext;

    public UserRepository(UsersContext usersContext)
    {
        this._usersContext = usersContext;
    }

    public AppUser GetUserById(string userId)
    {
        var user = _usersContext.UserModel.Find(userId);
        if (user  == null)
        {
            throw new AppUserDoesntExistException();
        }
        return user;
    }

    public List<AppUser> GetUsers()
    {
        return _usersContext.UserModel.ToList();
    }

    public void DeleteUser(string userId)
    {
        var user = _usersContext.UserModel.Find(userId);
        if (user != null) _usersContext.UserModel.Remove(user);
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