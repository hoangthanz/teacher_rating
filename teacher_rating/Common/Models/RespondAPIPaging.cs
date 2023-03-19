using teacher_rating.Common.Models.Paging;

namespace teacher_rating.Common.Models;

public class RespondAPIPaging<T>
{
    public ResultRespond Result { get; set; }
    public string Code { get; set; } = "00";
    public string Message { get; set; }
    public T Data { get; set; }
    public object Error { get; set; }
    public PagingResponse Paging { get; set; }
    
    public RespondAPIPaging(string message, T data, object error, PagingResponse paging)
    {
        Message = message;
        Data = data;
        Error = error;
        Paging = paging;
    }

    public RespondAPIPaging()
    {
        
    }
}