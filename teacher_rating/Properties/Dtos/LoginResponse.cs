namespace teacher_rating.Properties.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    
}