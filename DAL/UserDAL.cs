using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL;

public class UserDAL
{
    private readonly LeVietTravelDbContext _context;

    public UserDAL(LeVietTravelDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var cleanEmail = email.Trim().ToLower();
        return await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail);
    }

    public async Task<bool> ExistsEmailAsync(string email)
    {
        var cleanEmail = email.Trim().ToLower();
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == cleanEmail);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }
}
