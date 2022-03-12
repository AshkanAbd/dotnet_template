namespace Presentation.Programs;

public abstract class AbstractProgram
{
    public IConfigurationRoot Configuration { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public static IDictionary<string, AbstractProgram> ProgramsDictionary { get; set; }
    public abstract void Run(string[] args);

    public abstract string Name();
    public abstract string Description();
    public abstract string Section();

    public void InitServices()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        Configuration = builder.Build();
    }

    public static IDictionary<string, AbstractProgram> RegisterPrograms()
    {
        ProgramsDictionary = new Dictionary<string, AbstractProgram>();

        var abstractProgramType = typeof(AbstractProgram);

        var programs = abstractProgramType.Assembly.GetTypes()
            .Where(t => string.Equals(t.Namespace, abstractProgramType.Namespace, StringComparison.Ordinal))
            .Where(t => t.Name != abstractProgramType.Name)
            .Where(t => t.IsVisible)
            .ToArray();

        foreach (var program in programs) {
            var instanceProgram = (AbstractProgram) Activator.CreateInstance(program);
            if (instanceProgram == null) {
                continue;
            }

            ProgramsDictionary[instanceProgram.Name().ToLower()] = instanceProgram;
        }

        return ProgramsDictionary;
    }
}