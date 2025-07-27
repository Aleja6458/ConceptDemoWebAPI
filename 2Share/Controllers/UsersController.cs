using _2Share.Data;
using _2Share.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _2Share.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            // Verifica si el nombre de usuario ya existe
            var userExists = await _context.Users.AnyAsync(u => u.UserName == user.UserName);
            if (userExists)
            {
                return Conflict(new { message = "Username already exists." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully.",
                user = new
                {
                    user.UserId,
                    user.UserName
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == login.UserName && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);

            var authToken = new AuthToken
            {
                UserId = user.UserId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            _context.AuthTokens.Add(authToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                userId = authToken.UserId,

                success = true,
                message = "Login successful",
                token
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token is required." });

            var authToken = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (authToken == null)
                return NotFound(new { message = "Token not found or already logged out." });

            _context.AuthTokens.Remove(authToken);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logout successful." });
        }

        [HttpPost("validate-session")]
        public async Task<IActionResult> ValidateSession([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token is required." });

            var authToken = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == token);

            if (authToken == null)
                return NotFound(new { message = "Session not found or already logged out." });

            if (authToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(new { message = "Session has expired." });

            return Ok(new { sessionActive = true, message = "Session is active.", userId = authToken.UserId });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this_is_a_very_secure_256bit_key_!@#2025_super_secret_key");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
