using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using minutes90.Data;
using minutes90.Dto;
using minutes90.Entities;
using minutes90.Interfaces;

namespace minutes90.Repository
{
    public class UserRepo(UserManager<AppUsers> manager, AppDbContext context, IMapper mapper) : IUserRepo
    {
        public async Task<AppUsers?> AddUserAsync(RegisterDto registerDto)
        {
            if (await context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == registerDto.Username.ToUpper())
                is not null)
            {
                throw new Exception("Username is taken");
            }
            var user = mapper.Map<AppUsers>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            var result = await manager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                return user;
            }
            foreach (var error in result.Errors)
            {
                throw new Exception(error.Description);
            }
            return null;
        }
    }
}
