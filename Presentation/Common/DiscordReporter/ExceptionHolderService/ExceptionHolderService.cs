using ReportSharp.Models;

namespace Presentation.Common.DiscordReporter.ExceptionHolderService;

public class ExceptionHolderService : IExceptionHolderService
{
    private ReportSharpRequest Request { get; set; }

    public ReportSharpRequest GetReportSharpRequest()
    {
        return Request;
    }

    public void SetReportSharpRequest(ReportSharpRequest reportSharpRequest)
    {
        Request = reportSharpRequest;
    }
}