using DotnetCRUD.DTOs.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetCRUD.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    Task<UserResponseDto> GetByIdAsync(int id);
    Task<List<UserResponseDto>> GetAllUsersAsync();
}
