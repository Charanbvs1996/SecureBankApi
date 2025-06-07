namespace SecureBankApi.Models
{
    public class AuthResponse
    {
        public int UserId { get; set; } // The ID of the newly registered user
        public string Username { get; set; } = string.Empty; // The username of the newly registered user
        public string Message { get; set; } = string.Empty; // A friendly message, e.g., "User registered successfully!"
        public string? Token { get; set; } // Placeholder for the JWT token. Will be null for now, but used later.

    }
}
