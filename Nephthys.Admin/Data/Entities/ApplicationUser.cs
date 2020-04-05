using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nephthys.Admin.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

}
