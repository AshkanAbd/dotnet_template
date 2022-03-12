using System.Net.Mime;
using Application.Common.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Presentation.Common;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/")]
public class ControllerExtension : Controller
{
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected ActionResult ServeFileStream(string filePath)
    {
        if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), filePath))) {
            return NotFoundMsg<string>();
        }

        if (filePath.StartsWith("/")) {
            filePath = filePath.Substring(1);
        }

        var fileName = filePath.Substring(
            filePath.LastIndexOf("/", StringComparison.Ordinal) + 1
        );

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType)) {
            contentType = "application/octet-stream";
        }

        if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), filePath))) {
            throw new FileNotFoundException();
        }

        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), filePath), contentType, fileName);
    }

    protected ActionResult<StdResponse<T>> Base<T>(StdResponse<T> response)
    {
        return ResponseFormat.Base(response);
    }

    protected ActionResult<StdResponse<T>> Ok<T>(T data = default, string msg = null)
    {
        return ResponseFormat.Ok(data, msg);
    }

    protected ActionResult OkMsg<T>(string msg = null)
    {
        return ResponseFormat.OkMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> NotFound<T>(T data = default, string msg = "Notfound")
    {
        return ResponseFormat.NotFound(data, msg);
    }

    protected ActionResult NotFoundMsg<T>(string msg = "Notfound")
    {
        return ResponseFormat.NotFoundMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> PermissionDenied<T>(T data = default, string msg = null)
    {
        return ResponseFormat.PermissionDenied(data, msg);
    }

    protected ActionResult PermissionDeniedMsg<T>(string msg = null)
    {
        return ResponseFormat.PermissionDeniedMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> NotAuth<T>(T data = default, string msg = "Not authorized.")
    {
        return ResponseFormat.NotAuth(data, msg);
    }

    protected ActionResult NotAuthMsg<T>(string msg = "Not authorized.")
    {
        return ResponseFormat.NotAuthMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> BadRequest<T>(T data = default, string msg = null)
    {
        return ResponseFormat.BadRequest(data, msg);
    }

    protected ActionResult BadRequestMsg<T>(string msg = null)
    {
        return ResponseFormat.BadRequestMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> SyntaxError<T>(T data = default, string msg = null)
    {
        return ResponseFormat.SyntaxError(data, msg);
    }

    protected ActionResult SyntaxErrorMsg<T>(string msg = null)
    {
        return ResponseFormat.SyntaxErrorMsg<T>(msg);
    }

    protected ActionResult<StdResponse<T>> InternalError<T>(T data = default, string msg = null)
    {
        return ResponseFormat.InternalError(data, msg);
    }

    protected ActionResult InternalErrorMsg<T>(string msg = null)
    {
        return ResponseFormat.InternalErrorMsg<T>(msg);
    }
}