using ReportSharp.Builder.ReporterOptionsBuilder;
using ReportSharp.DiscordReporter.Services.DiscordService;

namespace Presentation.Common.DiscordReporter;

public class CustomDiscordReporterOptionsBuilder : IExceptionReporterOptionsBuilder<CustomDiscordReporter>
{
    private readonly HashSet<ulong> _channelIds = new();
    private string _token;

    public void Build(IServiceCollection serviceCollection)
    {
        serviceCollection.Configure((Action<DiscordConfig>) (config => {
            config.Token = _token;
            config.Channels = _channelIds;
        }));
        serviceCollection.AddSingleton<IDiscordService, DiscordService>();
    }

    public CustomDiscordReporterOptionsBuilder SetToken(string token)
    {
        _token = token;
        return this;
    }

    public CustomDiscordReporterOptionsBuilder AddChannelId(ulong channelId)
    {
        _channelIds.Add(channelId);
        return this;
    }
}