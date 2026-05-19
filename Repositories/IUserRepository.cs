using DotnetCRUD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetCRUD.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
}
