namespace Application.DTOs.Auth
{

    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; }
    }
}