using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PeregrineBackend.API.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        // --- ابدأ بإضافة الكود المؤقت هنا (لتوليد هاش كلمة مرور العميل) ---
      
        // --- انتهى الكود المؤقت ---

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // هذه الدالة تُستخدم عند إنشاء مستخدم جديد
        public async Task<User> CreateUserAsync(User user, string password)
        {
            // تشفير كلمة المرور باستخدام BCrypt قبل حفظها
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // هذه الدالة هي التي تتحقق من صحة كلمة المرور عند تسجيل الدخول
        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return false; // المستخدم غير موجود

            // استخدام BCrypt.Verify لمقارنة كلمة المرور المُدخلة مع الهاش المخزن في قاعدة البيانات
            // هذه الدالة تقوم بكل العمل السحري للمقارنة الآمنة
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}

