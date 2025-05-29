using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeregrineBackend.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var user = await _userService.GetUserByUsernameAsync(loginRequest.Username);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "اسم المستخدم أو كلمة المرور غير صحيحة" });
            }

            var isValidPassword = await _userService.ValidateUserCredentialsAsync(loginRequest.Username, loginRequest.Password);
            if (!isValidPassword)
            {
                return Unauthorized(new { success = false, message = "اسم المستخدم أو كلمة المرور غير صحيحة" });
            }

            if (user.Role != loginRequest.Role)
            {
                return Unauthorized(new { success = false, message = "الدور غير مطابق" });
            }

            // تحديث آخر تسجيل دخول
            user.LastLogin = DateTime.Now;
            await _userService.UpdateUserAsync(user);

            // إنشاء رمز JWT
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiryInHours"]));

            // تأكد من إرجاع كائن JSON وليس مصفوفة
            var result = new
            {
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    displayName = user.DisplayName,
                    email = user.Email,
                    role = user.Role,
                    profileImageUrl = user.ProfileImageUrl,
                    // تعديل هنا: تأكد من أن permissions هو كائن وليس مصفوفة
                    permissions = new Dictionary<string, bool>
                    {
                        { "viewProfile", true },
                        { "editProfile", true },
                        { "viewReports", user.Role == "client" || user.Role == "support" || user.Role == "admin" },
                        { "createReports", user.Role == "support" || user.Role == "admin" },
                        { "manageUsers", user.Role == "admin" },
                        { "manageClients", user.Role == "support" || user.Role == "admin" },
                        { "manageGuards", user.Role == "support" || user.Role == "admin" },
                        { "viewRequests", user.Role == "support" || user.Role == "admin" },
                        { "processRequests", user.Role == "support" || user.Role == "admin" }
                    },
                    lastLogin = user.LastLogin
                },
                token = new
                {
                    token = token,
                    refreshToken = Guid.NewGuid().ToString(), // في التطبيق الحقيقي، يجب تخزين رمز التحديث
                    expiresAt = expiresAt,
                    tokenType = "Bearer"
                },
                success = true,
                message = "تم تسجيل الدخول بنجاح"
            };

            // استخدام Ok() لإرجاع كائن JSON وليس مصفوفة
            return Ok(result);
        }

        // POST: api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            // في التطبيق الحقيقي، يجب التحقق من رمز التحديث
            return BadRequest(new { success = false, message = "لم يتم تنفيذ هذه الوظيفة بعد" });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // في التطبيق الحقيقي، يجب إبطال رمز التحديث
            return Ok(new { success = true, message = "تم تسجيل الخروج بنجاح" });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiryInHours"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
