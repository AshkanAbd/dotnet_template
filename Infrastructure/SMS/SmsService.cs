using Kavenegar;
using Microsoft.Extensions.Options;

namespace Infrastructure.SMS;

internal class SmsService : ISmsService
{
    public SmsService(IOptions<Config> options)
    {
        SmsConfig = options.Value.Sms;
    }

    public SmsConfig SmsConfig { get; }

    public async Task<bool> SendAsync(string msg, string receiver)
    {
        if (!SmsConfig.ShouldSend) {
            return true;
        }

        try {
            var api = new KavenegarApi(SmsConfig.ApiKey);
            await api.Send(SmsConfig.Sender, receiver, msg);
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public bool Send(string msg, string receiver)
    {
        if (!SmsConfig.ShouldSend) {
            return true;
        }

        try {
            var api = new KavenegarApi(SmsConfig.ApiKey);
            api.Send(SmsConfig.Sender, receiver, msg).Wait();
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public async Task<bool> LookupAsync(string template, string receiver, params string[] token)
    {
        if (!SmsConfig.ShouldSend) {
            return true;
        }

        try {
            var api = new KavenegarApi(SmsConfig.ApiKey);
            switch (token.Length) {
                case 0:
                    break;
                case 1:
                    await api.VerifyLookup(receiver, token[0], "", "", template);
                    break;
                case 2:
                    await api.VerifyLookup(receiver, token[0], token[1], "", template);
                    break;
                default:
                    await api.VerifyLookup(receiver, token[0], token[1], token[2], template);
                    break;
            }

            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public bool Lookup(string template, string receiver, params string[] token)
    {
        if (!SmsConfig.ShouldSend) {
            return true;
        }

        try {
            var api = new KavenegarApi(SmsConfig.ApiKey);
            switch (token.Length) {
                case 0:
                    break;
                case 1:
                    api.VerifyLookup(receiver, token[0], "", "", template).Wait();
                    break;
                case 2:
                    api.VerifyLookup(receiver, token[0], token[1], "", template).Wait();
                    break;
                default:
                    api.VerifyLookup(receiver, token[0], token[1], token[2], template).Wait();
                    break;
            }

            return true;
        }
        catch (Exception) {
            return false;
        }
    }
}