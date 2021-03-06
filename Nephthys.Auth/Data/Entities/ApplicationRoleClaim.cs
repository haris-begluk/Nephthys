﻿using Microsoft.AspNetCore.Identity;
using System;

namespace Nephthys.Admin.Data.Entities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
