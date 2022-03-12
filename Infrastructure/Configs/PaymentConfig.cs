namespace Infrastructure.Configs;

public class PaymentConfig
{
    public ZarinPal ZarinPal { get; set; } = null!;
}

public class ZarinPal
{
    public string MerchantId { get; set; } = null!;
    public bool Sandbox { get; set; } = true;
    public string CallbackUrl { get; set; } = null!;
}