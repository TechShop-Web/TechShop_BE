using BCrypt.Net;
using UserService.Repository.Models;
using UserService.Repository.Repositories;

namespace UserService.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepo.GetAllAsync();

        public async Task<User?> GetByIdAsync(int id) => await _userRepo.GetByIdAsync(id);

        public async Task<User> CreateAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _userRepo.Update(user);
            return await _userRepo.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<User> RegisterAsync(User user)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existing = await _userRepo.GetByEmailAsync(user.Email);
            if (existing != null)
                throw new Exception("Email đã tồn tại.");

            // Băm mật khẩu & thêm thời gian tạo
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.Now;

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }


        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("Email không tồn tại.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Mật khẩu không đúng.");

            return user;
        }

    }
}
