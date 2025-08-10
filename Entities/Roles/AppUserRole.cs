﻿using Microsoft.AspNetCore.Identity;

namespace minutes90.Entities.Roles
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public required AppRole Roles { get; set; }
        public required AppUsers Users { get; set; }
    }
}
