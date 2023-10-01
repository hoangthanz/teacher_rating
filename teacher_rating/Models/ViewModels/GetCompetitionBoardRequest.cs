namespace teacher_rating.Models.ViewModels;

public class GetCompetitionBoardRequest
{
    public string SchoolId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string GroupId { get; set; }
    public string UserId { get; set; }
}