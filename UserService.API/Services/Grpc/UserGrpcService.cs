using Grpc.Core;
using UserService.Service.Services;

namespace UserService.API.Services.Grpc
{
    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly IUserService _userService;
        public UserGrpcService(IUserService userService)
        {
            _userService = userService;
        }
        public async override Task<UserCheckResponse> CheckUserExists(UserCheckRequest request, ServerCallContext context)
        {
            var user = await _userService.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return new UserCheckResponse
                {
                    Exists = false,
                    Error = "User not found"
                };
            }
            return new UserCheckResponse
            {
                Exists = true,
                Error = ""
            };
        }
    }
}
