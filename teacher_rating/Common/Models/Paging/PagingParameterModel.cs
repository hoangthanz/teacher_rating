using System.ComponentModel.DataAnnotations;

namespace teacher_rating.Common.Models.Paging;

public class PagingParameterModel
{
    const int MaxPageSize = 100;

    public bool IsPaging { get; set; } = true;
    public int PageNumber { get; set; } = 1;

    private int _pageSize { get; set; } = 20;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}