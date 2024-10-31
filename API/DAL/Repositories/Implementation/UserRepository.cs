using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class UserRepository : IUserRepository,IRepository
{
    
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);  
    }

    public async Task<User> GetUserByUsernameOrEmailAsync(string username, string email)=>
        await _db.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

    public async Task AddUserAsync(User user) => await _db.Users.AddAsync(user);

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}