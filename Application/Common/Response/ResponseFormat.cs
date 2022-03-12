using Microsoft.AspNetCore.Mvc;

namespace Application.Common.Response;

public static class ResponseFormat
{
    public static JsonResult Base<T>(StdResponse<T> stdResponse, long? logId = null)
    {
        stdResponse.LogId = logId;
        return new JsonResult(stdResponse) {
            StatusCode = StdStatus.Get(stdResponse.Status),
        };
    }

    public static JsonResult Base<T>(string status = null, string msg = null, T data = default, long? logId = null)
    {
        return new(new StdResponse<T> {
            Status = status,
            Message = msg,
            Data = data
        }) {
            StatusCode = StdStatus.Get(status)
        };
    }

    public static JsonResult Ok<T>(T data = default, string msg = null, long? logId = null)
    {
        return Base("success", msg, data, logId);
    }

    public static JsonResult OkMsg<T>(string msg = null, T data = default, long? logId = null)
    {
        return Ok(data, msg, logId);
    }

    public static JsonResult NotFound<T>(T data = default, string msg = "Notfound", long? logId = null)
    {
        return Base("notfound-error", msg, data, logId);
    }

    public static JsonResult NotFoundMsg<T>(string msg = "Notfound", T data = default, long? logId = null)
    {
        return NotFound(data, msg, logId);
    }

    public static JsonResult PermissionDenied<T>(T data = default, string msg = null, long? logId = null)
    {
        return Base("access-denied-error", msg, data, logId);
    }

    public static JsonResult PermissionDeniedMsg<T>(string msg = null, T data = default, long? logId = null)
    {
        return PermissionDenied(data, msg, logId);
    }

    public static JsonResult NotAuth<T>(T data = default, string msg = "Not authorized.", long? logId = null)
    {
        return Base("unauthorized-error", msg, data, logId);
    }

    public static JsonResult NotAuthMsg<T>(string msg = "Not authorized.", T data = default,
        long? logId = null)
    {
        return NotAuth(data, msg, logId);
    }

    public static JsonResult BadRequest<T>(T data = default, string msg = null, long? logId = null)
    {
        return Base("validation-error", msg, data, logId);
    }

    public static JsonResult BadRequestMsg<T>(string msg = null, T data = default, long? logId = null)
    {
        return BadRequest(data, msg, logId);
    }

    public static JsonResult InternalError<T>(T data = default, string msg = null, long? logId = null)
    {
        return Base("internal-error", msg, data, logId);
    }

    public static JsonResult InternalErrorMsg<T>(string msg = null, T data = default, long? logId = null)
    {
        return InternalError(data, msg, logId);
    }

    public static JsonResult SyntaxError<T>(T data = default, string msg = null, long? logId = null)
    {
        return Base("syntax-error", msg, data, logId);
    }

    public static JsonResult SyntaxErrorMsg<T>(string msg = null, T data = default, long? logId = null)
    {
        return SyntaxError(data, msg, logId);
    }
}