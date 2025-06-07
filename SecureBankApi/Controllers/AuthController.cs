using Microsoft.Extensions.Configuration; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureBankApi.Data;
using SecureBankApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureBankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("signup")] 
        public async Task<IActionResult> SignUp([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            bool userExists = await _context.Users.AnyAsync(u =>
                u.Username == newUser.Username ||
                u.Email == newUser.Email ||
                u.MobileNumber == newUser.MobileNumber ||
                u.AadharNumber == newUser.AadharNumber);

            if (userExists)
            {
                return Conflict("User with provided Username, Email, Mobile, or Aadhar already exists.");
            }

         
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

            
            newUser.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); 


            return Ok(new AuthResponse
            {
                UserId = newUser.UserId,      
                Username = newUser.Username, 
                Message = "User registered successfully!",
                Token = null          
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users
                                                    .FirstOrDefaultAsync(u => 
                                                                                      u.Username == loginRequest.Identifier || 
                                                                                      u.Email == loginRequest.Identifier ||
                                                                                      u.AadharNumber == loginRequest.Identifier);
            if (user == null)
            {
                return Unauthorized("Invalid username, email, or Aadhar number.");
            }
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid password.");
            }
            var token = GenerateJwtToken(user);
            return Ok(new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Message = "Login successful!",
                Token = token 
            });
        }
        private string GenerateJwtToken(User user)
        {
            // 1. Define Claims: These are pieces of information about the user that will be included in the token.
            var claims = new List<Claim> 
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // User's ID
        new Claim(ClaimTypes.Name, user.Username), // User's Username
        // You can add more claims here if needed, e.g., user roles, account types etc.
        // new Claim(ClaimTypes.Role, user.AccountType.ToString()), // Example if you have roles
        };

            // 2. Get JWT Settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationInDays = Convert.ToDouble(jwtSettings["ExpirationInDays"] ?? "7"); // Default to 7 days if not set

            // Create the security key from your secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create signing credentials using the key and hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set the token's expiration time
            var expires = DateTime.Now.AddDays(expirationInDays);

            // 3. Create the JWT Token
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            // 4. Serialize the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}