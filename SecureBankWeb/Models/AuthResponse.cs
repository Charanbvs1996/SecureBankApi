namespace SecureBankWeb.Models
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // This will contain the JWT
        public bool IsSuccess => string.IsNullOrEmpty(Message) || Token != null; // Simple success check

    }
}
