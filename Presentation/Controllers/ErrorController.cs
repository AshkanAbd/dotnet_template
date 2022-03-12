using Application.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult Error()
    {
        return ResponseFormat.InternalErrorMsg<string>("مشکلی در سرور بوجود آمده است.");
    }
}