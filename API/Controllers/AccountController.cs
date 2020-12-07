using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _contex;
        public AccountController(DataContext contex)
        {
            _contex = contex;
        }

        // Login method
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            var user = await _contex.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
                return Unauthorized("Invalid username or password");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Password is incorrect!");
            }
            return user;
        }

        // Register method
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username already exists!");

            // for creating password hash and password salt
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _contex.Add(user);
            await _contex.SaveChangesAsync();

            return user;
        }

        // This method validates the new username input with database and prevent from duplications
        private async Task<bool> UserExists(string username)
        {
            return await _contex.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}