using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Repository.Models;
using UserService.Service.DTO;
using UserService.Service.Enums;
using UserService.Service.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        // ✅ Lấy toàn bộ user – Admin/Manager mới được phép
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // ✅ Lấy chi tiết user theo ID – người dùng nào cũng được xem
        [Authorize]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        // ✅ Tạo mới user – chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            try
            {
                var created = await _userService.CreateAsync(user);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Cập nhật user – Admin/Manager
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();

            var success = await _userService.UpdateAsync(user);
            return success ? NoContent() : NotFound();
        }

        // ✅ Xóa user – chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        // ✅ Đăng ký (cho phép dùng số hoặc tên vai trò)
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (int.TryParse(user.Role, out int roleVal) && Enum.IsDefined(typeof(UserRoleEnum), roleVal))
            {
                user.Role = ((UserRoleEnum)roleVal).ToString();
            }
            else if (!Enum.TryParse<UserRoleEnum>(user.Role, out _))
            {
                return BadRequest("Role không hợp lệ. Dùng 0:Admin, 1:Manager, 2:User hoặc chuỗi tương ứng.");
            }

            try
            {
                var created = await _userService.RegisterAsync(user);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //    // ✅ Đăng nhập – trả JWT Token
        //    [AllowAnonymous]
        //    [HttpPost("login")]
        //    public async Task<IActionResult> Login([FromBody] LoginDto login)
        //    {
        //        try
        //        {
        //            var user = await _userService.LoginAsync(login.Email, login.Password);

        //            var claims = new[]
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //                new Claim(ClaimTypes.Email, user.Email),
        //                new Claim(ClaimTypes.Role, user.Role)
        //            };

        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //            var token = new JwtSecurityToken(
        //                issuer: _config["Jwt:Issuer"],
        //                audience: _config["Jwt:Audience"],
        //                claims: claims,
        //                expires: DateTime.Now.AddHours(3),
        //                signingCredentials: creds
        //            );

        //            return Ok(new
        //            {
        //                token = new JwtSecurityTokenHandler().WriteToken(token),
        //                user = new { user.Id, user.Email, user.FullName, user.Role }
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            return Unauthorized(new { message = ex.Message });
        //        }
        //    }
        //}
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Tên người dùng hoặc mật khẩu không đúng");
            }
        }

     
    }
}
