using teacher_rating.Common.Models.Paging;

namespace teacher_rating.Models.ViewModels;

public class SearchSelfCriticism : PagingParameterModel
{
    public string? AssessmentCriteria { get; set; }
    public string? GroupId { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}