using System.ComponentModel.DataAnnotations;

namespace SecureBankApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters.")]
        public string PasswordHash { get; set; } = string.Empty; // Will store the hashed password

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
        [Phone(ErrorMessage = "Invalid mobile number format.")] // Provides basic phone format validation
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aadhar number is required.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhar number must be 12 digits.")]
        public string AadharNumber { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string? Gender { get; set; } // Nullable, as it might be an optional field in your form

        [Required(ErrorMessage = "Account type is required.")]
        [StringLength(20, ErrorMessage = "Account type cannot exceed 20 characters.")]
        public string AccountType { get; set; } = string.Empty; // E.g., "Savings", "Current"

        [Required(ErrorMessage = "Branch is required.")]
        [StringLength(50, ErrorMessage = "Branch cannot exceed 50 characters.")]
        public string Branch { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Sets the creation timestamp, default to UTC now

    }
}
