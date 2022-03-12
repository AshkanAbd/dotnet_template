namespace Application.Common.Response;

public static class StdResponseFactory
{
    public static StdResponse<TValue> Ok<TValue>(object data = default, string message = null) =>
        new("success", message, data);

    public static StdResponse<TValue> OkMsg<TValue>(string message = null) =>
        new("success", message);

    public static StdResponse<TValue> NotFound<TValue>(object data = default, string message = null) =>
        new("notfound-error", message, data);

    public static StdResponse<TValue> NotFoundMsg<TValue>(string message = null) =>
        new("notfound-error", message);

    public static StdResponse<TValue> PermissionDenied<TValue>(object data = default, string message = null) =>
        new("access-denied-error", message, data);

    public static StdResponse<TValue> PermissionDeniedMsg<TValue>(string message = null) =>
        new("access-denied-error", message);

    public static StdResponse<TValue> NotAuth<TValue>(object data = default, string message = null) =>
        new("unauthorized-error", message, data);

    public static StdResponse<TValue> NotAuthMsg<TValue>(string message = null) =>
        new("unauthorized-error", message);

    public static StdResponse<TValue> BadRequest<TValue>(object data = default, string message = null) =>
        new("validation-error", message, data);

    public static StdResponse<TValue> BadRequestMsg<TValue>(string message = null) =>
        new("validation-error", message);

    public static StdResponse<TValue> InternalError<TValue>(object data = default, string message = null) =>
        new("internal-error", message, data);

    public static StdResponse<TValue> InternalErrorMsg<TValue>(string message = null) =>
        new("internal-error", message);

    public static StdResponse<TValue> SyntaxError<TValue>(object data = default, string message = null) =>
        new("syntax-error", message, data);

    public static StdResponse<TValue> SyntaxErrorMsg<TValue>(string message = null) =>
        new("syntax-error", message);
}