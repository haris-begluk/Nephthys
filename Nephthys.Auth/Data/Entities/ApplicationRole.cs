using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Nephthys.Admin.Data.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}
