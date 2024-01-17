using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DatabaseSetupLocal.Areas.Identity.Data;
[AllowAnonymous]
// Add profile data for application users by adding properties to the F1Shotsuser class
public class AppUser : IdentityUser
{
    [PersonalData]
    public string ? FirstName { get; set; }
    [PersonalData]
    public string ? LastName { get; set; }
    public bool Admin { get; set; }
    public bool GroupAdmin { get; set; }
}

