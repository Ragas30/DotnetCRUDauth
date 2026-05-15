using DotnetCRUD.Models;

namespace DotnetCRUD.Repositories

    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string email);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
