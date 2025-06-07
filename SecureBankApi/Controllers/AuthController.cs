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
        private readonly ApplicationDbContext _context; // Field to hold the database context

        // Constructor: ASP.NET Core's dependency injection will provide an instance of ApplicationDbContext
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/Auth/signup (This is the specific endpoint for user registration)
        [HttpPost("signup")] // Defines this method as an HTTP POST endpoint at the specified route
        public async Task<IActionResult> SignUp([FromBody] User newUser)
        {
            // 1. Model State Validation: Checks if the incoming JSON data conforms to the User model's data annotations (e.g., [Required], [EmailAddress])
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns a 400 Bad Request with validation errors if data is invalid
            }

            // 2. Uniqueness Checks: Prevents duplicate users based on unique fields
            bool userExists = await _context.Users.AnyAsync(u =>
                u.Username == newUser.Username ||
                u.Email == newUser.Email ||
                u.MobileNumber == newUser.MobileNumber ||
                u.AadharNumber == newUser.AadharNumber);

            if (userExists)
            {
                // Returns a 409 Conflict if a user with any of these unique identifiers already exists
                return Conflict("User with provided Username, Email, Mobile, or Aadhar already exists.");
            }

            // 3. Password Hashing: Hashes the plain-text password received from Angular before storing it
            // It's CRITICAL to never store plain-text passwords.
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

            // 4. Set Creation Timestamp: (Though HasDefaultValueSql handles this, setting it here is also fine)
            // newUser.CreatedAt is already configured to default to GETUTCDATE() in the database,
            // so you technically don't need this line, but it doesn't hurt.
            // newUser.CreatedAt = DateTime.UtcNow;

            // 5. Add User to Database: Adds the new user entity to the DbContext's change tracker
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Persists the changes (the new user) to the database

            // 6. Return Success Response: Prepares a success response for Angular
            // For now, no JWT token is generated here for immediate login. That will be added for the Login page.
            return Ok(new AuthResponse
            {
                UserId = newUser.UserId,       // Return the ID assigned by the database
                Username = newUser.Username,   // Return the username
                Message = "User registered successfully!", // Success message
                Token = null                   // Token is null for now, as JWT logic is for login
            });
        }
    }
}