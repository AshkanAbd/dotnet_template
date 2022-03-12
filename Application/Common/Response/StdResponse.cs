namespace Application.Common.Response;

public class StdResponse<TValue>
{
    protected internal StdResponse(string status = null, string message = null, object data = default)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public StdResponse(string status, string message, object data, long? logId)
    {
        Status = status;
        Message = message;
        Data = data;
        LogId = logId;
    }

    public object Data { get; set; }
    public TValue DataStruct { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public long? LogId { get; set; }

    public TValue DataAsDataStruct() => (TValue) Data;
}