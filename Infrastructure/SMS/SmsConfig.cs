namespace Infrastructure.SMS;

public class SmsConfig
{
    public string ApiKey { get; set; }
    public bool ShouldSend { get; set; }
    public string Sender { get; set; }
}