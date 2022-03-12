namespace Presentation.Programs;

public class CreateModelProgram : AbstractProgram
{
    protected string ModeName = "Model";

    protected string Namespace = "Domain";
    protected string Path = "Domain";

    protected Dictionary<string, string> PathDictionary = new() {
        {"Model", "../#Path#/Models/#ModelName#.cs"},
        {"Configuration", "../#Path#/Configurations/#ModelName#Configuration.cs"},
    };

    protected Dictionary<string, string> TemplateFilesDictionary = new() {
        {"Model", "./Programs/CreateModelTemplates/model.template"},
        {"Configuration", "./Programs/CreateModelTemplates/configuration.template"},
    };

    public override void Run(string[] args)
    {
        if (args.Length < 2) {
            PrintUsage();
            return;
        }

        ReadArgs(args);

        CreateFiles();
        Console.WriteLine("Done.");
    }

    protected virtual void CreateFiles()
    {
        var filesDictionary = LoadFiles();
        UpdateFiles(filesDictionary);
        PreparePaths();
        WriteToPaths(filesDictionary);
    }

    protected virtual void WriteToPaths(Dictionary<string, string> filesDictionary)
    {
        foreach (var key in filesDictionary.Keys) {
            File.WriteAllText(PathDictionary[key], filesDictionary[key]);
            Console.WriteLine($"{PathDictionary[key]} created.");
        }
    }

    protected virtual void PreparePaths()
    {
        foreach (var key in PathDictionary.Keys) {
            PathDictionary[key] = ReplaceTemplates(PathDictionary[key]);
            CreatePathIfNotExists(GetDirPath(PathDictionary[key]));
        }
    }

    protected virtual string GetDirPath(string filePath)
    {
        return filePath[..filePath.LastIndexOf("/", StringComparison.Ordinal)];
    }

    protected virtual void CreatePathIfNotExists(string dirPath)
    {
        if (Directory.Exists(dirPath)) {
            return;
        }

        Directory.CreateDirectory(dirPath!);
    }

    protected virtual void UpdateFiles(Dictionary<string, string> filesDictionary)
    {
        foreach (var key in filesDictionary.Keys) {
            filesDictionary[key] = ReplaceTemplates(filesDictionary[key]);
        }
    }

    protected virtual Dictionary<string, string> LoadFiles()
    {
        var files = new Dictionary<string, string>();

        foreach (var (key, value) in TemplateFilesDictionary) {
            files[key] = File.ReadAllText(value);
        }

        return files;
    }

    protected virtual string ReplaceTemplates(string template)
    {
        return template.Replace("#ModelName#", ModeName)
            .Replace("#Namespace#", Namespace)
            .Replace("#Path#", Path);
    }

    protected virtual void ReadArgs(string[] args)
    {
        ReadModelName(args);
        ReadNamespace(args);
    }

    protected virtual void ReadModelName(string[] args)
    {
        ModeName = args[1];
    }

    protected virtual void ReadNamespace(string[] args)
    {
        if (args.Length >= 4 && args[2] == "--namespace") {
            Namespace = args[3];
        }

        UpdatePath();
    }

    protected virtual void UpdatePath()
    {
        Path = Namespace.Replace(".", "/");
    }

    protected virtual void PrintUsage()
    {
        Console.WriteLine("Usage: create:model [model] [options]");
        Console.WriteLine("Options: ");
        Console.WriteLine("   --namespace [namespace]         Use custom namespace. 'Domain' is default.");
    }

    public override string Name()
    {
        return "create:model";
    }

    public override string Description()
    {
        return "Creates a model with configuration file.";
    }

    public override string Section()
    {
        return "Create";
    }
}