namespace teacher_rating.Models.ViewModels;

public class CreateTeacher
{
    
    public string Name { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Email { get; set; }
    
    public Guid UserId { get; set; }
    
    public string SchoolId { get; set; }
    public string GroupId { get; set; }
}