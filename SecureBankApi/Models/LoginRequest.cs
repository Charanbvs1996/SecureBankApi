using System.ComponentModel.DataAnnotations;

namespace SecureBankApi.Models
{
    public class LoginRequest
    {

        [Required(ErrorMessage = "Username is required.")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
