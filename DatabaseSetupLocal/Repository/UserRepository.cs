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
}