namespace Application.DTOs.Auth
{

    public class RefreshTokenRequestDto
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? AccessToken { get; set; }

        public DateTime? Expiration { get; set; }

        public string? RefreshToken { get; set; }   
    }
}