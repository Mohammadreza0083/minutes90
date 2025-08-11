using minutes90.Dto;
using minutes90.Entities;

namespace minutes90.Interfaces
{
    public interface IUserRepo
    {
        Task<AppUsers?> AddUserAsync(RegisterDto registerDto);

    }
}
