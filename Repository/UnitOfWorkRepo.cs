using minutes90.Data;
using minutes90.Interfaces;

namespace minutes90.Repository
{
    public class UnitOfWorkRepo(AppDbContext context, IUserRepo userRepo) : IUnitOfWorkRepo
    {
        public IUserRepo UserRepository => userRepo;

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
