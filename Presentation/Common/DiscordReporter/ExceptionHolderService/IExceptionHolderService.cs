using ReportSharp.Models;

namespace Presentation.Common.DiscordReporter.ExceptionHolderService;

public interface IExceptionHolderService
{
    public ReportSharpRequest GetReportSharpRequest();
    public void SetReportSharpRequest(ReportSharpRequest reportSharpRequest);
}