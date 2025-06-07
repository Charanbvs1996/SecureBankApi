using System.ComponentModel.DataAnnotations;

namespace SecureBankWeb.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Identifier is required (username, email, mobile, or Aadhar).")]
        public string Identifier { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
