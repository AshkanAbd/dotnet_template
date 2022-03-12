namespace Infrastructure.Fcm;

public interface IFcmService
{
    public Task SendData(List<string> tokens, object data);
    public Task SendNotification(List<string> tokens, string title, string body);
    public Task Send(List<string> tokens, object data, string title, string body);
}