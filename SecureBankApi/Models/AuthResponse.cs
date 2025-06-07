namespace SecureBankApi.Models
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty; 
        public string? Token { get; set; } 

    }
}
