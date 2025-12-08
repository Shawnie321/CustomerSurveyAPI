using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSurveyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) return false;

            existing.Username = user.Username;
            existing.PasswordHash = user.PasswordHash;
            existing.Role = user.Role;

            // propagate newly added properties
            existing.FirstName = user.FirstName;
            existing.MiddleName = user.MiddleName;
            existing.LastName = user.LastName;
            existing.Email = user.Email;
            existing.PhoneNumber = user.PhoneNumber;
            existing.DateOfBirth = user.DateOfBirth;

            // keep CreatedAt as the original creation timestamp (do not overwrite)
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return false;

            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
