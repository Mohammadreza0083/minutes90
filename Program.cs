
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using minutes90.Data;
using minutes90.Entities;
using minutes90.Entities.Roles;
using minutes90.Extensions;

namespace minutes90
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            SwaggerServicesExtensions.AddOpenApi(builder.Services);
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                });
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            using IServiceScope scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                AppDbContext context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
                var userManager = services.GetRequiredService<UserManager<AppUsers>>();
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                await IdentityDataSeederExtension.SeedUsersAndRolesAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                ILogger logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex.Message, "An error occurred while migrating the database.");
            }
            app.Run();
        }
    }
}
