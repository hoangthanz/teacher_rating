namespace teacher_rating.Common.Models;

public class RespondApi<T>
{
    public ResultRespond Result { get; set; }
    public string Code { get; set; } = "00";
    public string Message { get; set; }
    public T Data { get; set; }
    public object Error { get; set; }
    
    public RespondApi(string message, T data, object error)
    {
        Message = message;
        Data = data;
        Error = error;
    }
    public RespondApi()
    {
    }
    
    public RespondApi(ResultRespond result, string code, string message, T data, object error = null )
    {
        Result = result;
        Code = code;
        Message = message;
        Data = data;
        Error = error;
    }
}

public enum ResultRespond
{
    Error, Succeeded, Failed, NotFound, Duplication
}