using Presentation.Common.DiscordReporter.ExceptionHolderService;
using ReportSharp.Models;

namespace Presentation.Common.DiscordReporter;

public class CustomDiscordReporter : ReportSharp.DiscordReporter.Reporters.DiscordReporter.DiscordReporter
{
    public CustomDiscordReporter(IServiceProvider provider, IExceptionHolderService exceptionHolderService)
        : base(provider)
    {
        ExceptionHolderService = exceptionHolderService;
    }

    public IExceptionHolderService ExceptionHolderService { get; set; }

    public override async Task ReportException(HttpContext httpContext, ReportSharpRequest request,
        Exception exception)
    {
        if (!(ExceptionHolderService.GetReportSharpRequest() == null ||
              ExceptionHolderService.GetReportSharpRequest().ReportSharpResponse.Content !=
              request.ReportSharpResponse.Content)) {
            await DiscordService.SendMessage("Previous bug happened again.");
            return;
        }

        ExceptionHolderService.SetReportSharpRequest(request);

        await base.ReportException(httpContext, request, exception);
    }
}