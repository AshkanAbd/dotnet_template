using Bogus;

namespace Infrastructure.Seeds;

public class BaseSeeder
{
    protected static short ALPHA_AND_DIGITS = 0;
    protected static short DIGITS = 1;
    protected static short ALPHA = 2;
    protected static short ALPHA_SPACE = 3;

    protected List<string> _chars = new() {
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
        "0123456789",
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ",
    };

    protected Faker Faker;
    protected Faker FakerFa;

    protected Random RandomNumber;

    public BaseSeeder(AppDbContext dbContext)
    {
        DbContext = dbContext;
        RandomNumber = new Random();
        Faker = new Faker();
        FakerFa = new Faker("fa");
    }

    public AppDbContext DbContext { get; set; }

    /**
    * Mod = 0 => number and alphabet,
    * Mod = 1 => number,
    * Mod = 2 => alphabet,
    * Mod = 3 => alphabet space,
    */
    protected string RandomString(int length, int mod = 0)
    {
        return new(
            Enumerable
                .Repeat(_chars[mod], length)
                .Select(s => s[RandomNumber.Next(s.Length)])
                .ToArray()
        );
    }
}