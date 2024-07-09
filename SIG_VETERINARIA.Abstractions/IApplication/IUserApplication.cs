using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;

namespace SIG_VETERINARIA.Abstractions.IApplication
{
    public interface IUserApplication
    {
        public Task<ResultDto<UserListResponseDTO>> GetAll(UserListRequestDto request);
        public Task<ResultDto<int>> Create(UserCreateRequestDto request);

        public Task<ResultDto<int>> Delete(DeleteDto request);
        public Task<AuthResponseDto> Login(LoginRequestDto request);
    }
}
