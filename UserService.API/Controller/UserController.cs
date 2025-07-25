using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Service.DTO;
using UserService.Service.Enums;
using UserService.Service.Services;

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

    // ✅ Đăng ký cho User (Mặc định vai trò là User)
    [AllowAnonymous]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
        // Vai trò mặc định là "User" nếu không truyền vào
        if (string.IsNullOrEmpty(user.Role))
        {
            user.Role = "User"; // Mặc định là "User"
        }

        try
        {
            var created = await _userService.RegisterAsync(user);
            return Ok(created); // Trả về tài khoản đã tạo
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   // [Authorize(Roles = "Admin,Manager")]
    [HttpPost("register-admin-manager")]
    public async Task<IActionResult> RegisterAdminOrManager([FromBody] User user)
    {
        // Nếu không truyền vai trò, mặc định là "User"
        if (string.IsNullOrEmpty(user.Role))
        {
            user.Role = UserRoleEnum.User.ToString();  // Mặc định là "User"
        }

        try
        {
            // Kiểm tra và chuyển đổi vai trò số (1, 2, 3) thành tên vai trò
            if (int.TryParse(user.Role, out int role))
            {
                var roleName = Enum.GetName(typeof(UserRoleEnum), role);
                if (roleName != null)
                {
                    user.Role = roleName;  // Gán tên vai trò tương ứng
                }
                else
                {
                    return BadRequest("Role không hợp lệ. Dùng 1:Admin, 2: Manager, hoặc 3: User.");
                }
            }
            else if (!Enum.IsDefined(typeof(UserRoleEnum), user.Role))
            {
                return BadRequest("Role không hợp lệ. Dùng 1:Admin, 2: Manager, hoặc 3: User.");
            }

            var created = await _userService.RegisterAsync(user);
            return Ok(created); // Trả về tài khoản đã tạo
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    // ✅ Đăng nhập – trả JWT Token
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
