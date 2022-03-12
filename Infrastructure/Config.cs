using Infrastructure.Configs;
using Infrastructure.Fcm;
using Infrastructure.ImageUrlFormatter;
using Infrastructure.SMS;

namespace Infrastructure;

public class Config
{
    public SmsConfig Sms { get; set; } = null!;
    public FcmConfig Fcm { get; set; } = null!;
    public PaymentConfig Payment { get; set; } = null!;
    public JwtConfig Jwt { get; set; } = null!;
    public RedisConfig Redis { get; set; } = null!;
    public ImageUrlFormatterConfig ImageUrlFormatter { get; set; }
    public string Environment { get; set; } = null!;
    public string Url { get; set; } = null!;
}