using DotnetCRUD.Models;

namespace DotnetCRUD.DTOs.Auth;

public class DeleteUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = UserRole.USER.ToString();
}