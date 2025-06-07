using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureBankApi.Data;
using SecureBankApi.Models;

namespace SecureBankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}