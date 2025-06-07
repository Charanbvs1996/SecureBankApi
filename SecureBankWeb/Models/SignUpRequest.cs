using System.ComponentModel.DataAnnotations;

namespace SecureBankWeb.Models
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [Phone(ErrorMessage = "Invalid Mobile Number.")]
        [StringLength(15, ErrorMessage = "Mobile Number cannot exceed 15 characters.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Aadhar Number is required.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhar Number must be 12 digits.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar Number must be 12 digits.")]
        public string AadharNumber { get; set; }

        [Required(ErrorMessage = "Account Type is required.")]
        [StringLength(20, ErrorMessage = "Account Type cannot exceed 20 characters.")]
        public string AccountType { get; set; } // e.g., "Savings", "Current"

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string Gender { get; set; } // e.g., "Male", "Female", "Other"

        [Required(ErrorMessage = "Branch is required.")]
        [StringLength(50, ErrorMessage = "Branch name cannot exceed 50 characters.")]
        public string Branch { get; set; } // e.g., "Main Branch", "City Center"

    }
}
