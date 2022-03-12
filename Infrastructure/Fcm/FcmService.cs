using FCM.Net;
using Microsoft.Extensions.Options;

namespace Infrastructure.Fcm;

internal class FcmService : IFcmService
{
    private readonly Sender _sender;

    public FcmService(IOptions<Config> options)
    {
        _sender = new Sender(options.Value.Fcm.ServerKey);
    }

    public async Task SendData(List<string> tokens, object data)
    {
        await Send(tokens, data, null, null);
    }

    public async Task SendNotification(List<string> tokens, string title, string body)
    {
        await Send(tokens, null, title, body);
    }

    public async Task Send(List<string> tokens, object data, string title, string body)
    {
        Notification notification = null;

        if (title != null && body != null) {
            notification = new Notification {
                Title = title,
                Body = body,
            };
        }

        var message = new Message {
            RegistrationIds = tokens,
            Notification = notification,
            Data = data,
        };

        await _sender.SendAsync(message);
    }
}