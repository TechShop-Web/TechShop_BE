using UserService.Repository.Models;

namespace UserService.Service.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User> RegisterAsync(User user);
        Task<User?> LoginAsync(string email, string password);
    }
}
