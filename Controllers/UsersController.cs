using Microsoft.AspNetCore.Mvc;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeregrineBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { users, total = users.Count, success = true });
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { success = false, message = "المستخدم غير موجود" });
            }

            return Ok(new { user, success = true });
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var existingUser = await _userService.GetUserByUsernameAsync(userDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, message = "اسم المستخدم موجود بالفعل" });
            }

            var user = new User
            {
                Username = userDto.Username,
                DisplayName = userDto.DisplayName,
                Email = userDto.Email,
                Role = userDto.Role,
                LastLogin = DateTime.Now,
                IsActive = true
            };

            var createdUser = await _userService.CreateUserAsync(user, userDto.Password);

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, new { 
                user = new { 
                    id = createdUser.Id,
                    username = createdUser.Username,
                    displayName = createdUser.DisplayName,
                    email = createdUser.Email,
                    role = createdUser.Role
                }, 
                success = true 
            });
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "المستخدم غير موجود" });
            }

            user.DisplayName = userDto.DisplayName;
            user.Email = userDto.Email;
            user.Role = userDto.Role;
            user.IsActive = userDto.IsActive;

            var result = await _userService.UpdateUserAsync(user);
            if (!result)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث المستخدم" });
            }

            return Ok(new { 
                user = new { 
                    id = user.Id,
                    username = user.Username,
                    displayName = user.DisplayName,
                    email = user.Email,
                    role = user.Role,
                    isActive = user.IsActive
                }, 
                success = true 
            });
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = "المستخدم غير موجود" });
            }

            return Ok(new { success = true, message = "تم حذف المستخدم بنجاح" });
        }
    }

    // DTOs
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class UserUpdateDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
