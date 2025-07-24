using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Repository.Models;
using UserService.Repository.Repositories;
using UserService.Service.DTO;

namespace UserService.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _configuration;


        public UserService(IUserRepository userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
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


        //public async Task<User?> LoginAsync(string email, string password)
        //{
        //    var user = await _userRepo.GetByEmailAsync(email);

        //    if (user == null)
        //        throw new Exception("Email không tồn tại.");

        //    if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        //        throw new Exception("Mật khẩu không đúng.");

        //    return user;
        //}
        public async Task<LoginResponseDTO?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepo.GetByEmailAsync(loginDto.Email);
            if (user == null || user.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Sai email hoặc mật khẩu");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var assertedToken = jwtTokenHandler.WriteToken(securityToken);

            return new LoginResponseDTO
            {
                AccessToken = assertedToken,
                Email = loginDto.Email
            };
        }
    }
}
