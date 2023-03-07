namespace teacher_rating.Properties.Dtos;

public class UpdateUser
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string SchoolId { get; set; } = null!;
}