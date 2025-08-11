﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using minutes90.Data;
using minutes90.Entities;
using minutes90.Entities.Roles;
using System.Text;

namespace minutes90.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices
       (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<AppUsers, AppRole>(op =>
            {
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireDigit = true;
                op.Password.RequireLowercase = true;
                op.Password.RequireUppercase = true;
                op.Password.RequiredLength = 8;
                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                op.Lockout.MaxFailedAccessAttempts = 5;
                op.Lockout.AllowedForNewUsers = true;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                string tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("TokenKey is missing");
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                op.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];

                        if (string.IsNullOrEmpty(context.Token))
                        {
                            var accessTokenFromQuery = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/hubs") && !string.IsNullOrEmpty(accessTokenFromQuery))
                            {
                                context.Token = accessTokenFromQuery;
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorizationBuilder()
                .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

            return services;
        }
    }
}
