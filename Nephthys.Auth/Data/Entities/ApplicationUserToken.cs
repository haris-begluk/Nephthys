using Microsoft.AspNetCore.Identity;
using System;

namespace Nephthys.Admin.Data.Entities
{
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
