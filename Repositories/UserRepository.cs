using DotnetCRUD.Data;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DotnetCRUD.Repositories;

public class UserRepository: IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsycn(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> UserAsync(UserRepository user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}