﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minutes90.Entities;

namespace minutes90.Data.Config
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUsers>
    {
        public void Configure(EntityTypeBuilder<AppUsers> builder)
        {
            builder.ToTable("Users");
            builder.Ignore(u => u.PhoneNumber);
            builder.Ignore(u => u.PhoneNumberConfirmed);
            builder.Ignore(u => u.TwoFactorEnabled);
            builder.Ignore(u => u.EmailConfirmed);
        }
    }
}
