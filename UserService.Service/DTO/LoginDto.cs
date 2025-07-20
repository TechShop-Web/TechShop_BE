using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Service.DTO
{
    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class LoginResponseDTO
    {
        public string? Email { get; set; } = null!;
        public string? AccessToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }
}
