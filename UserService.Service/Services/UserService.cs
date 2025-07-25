using BCrypt.Net;
using UserService.Repository.Models;
using UserService.Repository.Repositories;
using UserService.Service.DTO;
using UserService.Service.Enums;

namespace UserService.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // Lấy danh sách người dùng mà không trả mật khẩu
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new User
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = GetRoleName(u.Role),
                CreatedAt = u.CreatedAt
            });
        }

        // Lấy chi tiết người dùng, bao gồm mật khẩu
        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user != null ? new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,  // Trả về mật khẩu đầy đủ ở đây
                FullName = user.FullName,
                Role = GetRoleName(user.Role),
                CreatedAt = user.CreatedAt
            } : null;
        }

        // Tạo tài khoản mới, mã hóa mật khẩu
        public async Task<User> CreateAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        // Cập nhật thông tin người dùng
        public async Task<bool> UpdateAsync(User user)
        {
            _userRepo.Update(user);
            return await _userRepo.SaveChangesAsync() > 0;
        }

        // Xóa người dùng
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

            // Mặc định vai trò là "User" nếu không truyền vai trò
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = UserRoleEnum.User.ToString();  // Mặc định vai trò là "User"
            }
            else
            {
                // Kiểm tra và chuyển đổi số vai trò (1, 2, 3) thành tên vai trò tương ứng
                if (int.TryParse(user.Role, out int role))
                {
                    // Sử dụng Enum.GetName để lấy tên từ giá trị số
                    var roleName = Enum.GetName(typeof(UserRoleEnum), role);
                    if (roleName != null)
                    {
                        user.Role = roleName;  // Gán tên vai trò tương ứng
                    }
                    else
                    {
                        throw new Exception("Role không hợp lệ. Dùng 1:Admin, 2: Manager, hoặc 3: User.");
                    }
                }
                else if (!Enum.IsDefined(typeof(UserRoleEnum), user.Role))
                {
                    throw new Exception("Role không hợp lệ. Dùng Admin, Manager, hoặc User.");
                }
            }

            // Băm mật khẩu & thêm thời gian tạo
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.Now;

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }




        // Đăng nhập và trả về token JWT
        public async Task<LoginResponseDTO?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepo.GetByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))  // Sử dụng mã hóa bcrypt để kiểm tra mật khẩu
                throw new UnauthorizedAccessException("Sai email hoặc mật khẩu");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];

            var roleName = GetRoleName(user.Role);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roleName)  // Trả về vai trò dưới dạng chuỗi
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Mật khẩu không đúng.");

            return new LoginResponseDTO
            {
                AccessToken = assertedToken,
                Email = loginDto.Email,
                Role = roleName  // Trả về vai trò trong response
            };
        }

        // Chuyển vai trò từ số thành chuỗi
        private string GetRoleName(string role)
        {
            // Sử dụng Enum để chuyển từ số thành chuỗi
            if (Enum.TryParse<UserRoleEnum>(role, out var roleEnum))
            {
                return roleEnum.ToString(); // Trả về tên vai trò như Admin, Manager, User
            }

            return UserRoleEnum.User.ToString(); // Mặc định là "User"
        }

    }
}