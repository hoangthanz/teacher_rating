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
    Success = 0,
    Fail = 1,
    Error = 2,
    NotFound = 3,
    Unauthorized = 4,
    Forbidden = 5,
    BadRequest = 6,
    Conflict = 7,
    NoContent = 8,
    UnprocessableEntity = 9,
    Locked = 10,
    UnavailableForLegalReasons = 11,
    TooManyRequests = 12,
    InternalServerError = 13,
    NotImplemented = 14,
    BadGateway = 15,
    ServiceUnavailable = 16,
    GatewayTimeout = 17,
    HttpVersionNotSupported = 18,
    VariantAlsoNegotiates = 19,
    InsufficientStorage = 20,
    LoopDetected = 21,
    NotExtended = 22,
    NetworkAuthenticationRequired = 23,
    UnknownError = 24
    
}