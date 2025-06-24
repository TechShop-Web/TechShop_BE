using UserService.Repository.Models;

namespace UserService.Repository.Repositories
{
    public interface IUserRepository
    {

        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
        Task<int> SaveChangesAsync();
    }
}
