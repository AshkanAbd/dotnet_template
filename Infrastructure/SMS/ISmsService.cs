namespace Infrastructure.SMS;

public interface ISmsService
{
    public SmsConfig SmsConfig { get; }
    public Task<bool> SendAsync(string msg, string receiver);
    public bool Send(string msg, string receiver);
    public Task<bool> LookupAsync(string template, string receiver, params string[] token);
    public bool Lookup(string template, string receiver, params string[] token);
}