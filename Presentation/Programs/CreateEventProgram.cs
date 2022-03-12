namespace Presentation.Programs;

public class CreateEventProgram : CreateModelProgram
{
    protected string EventName = "Event";
    protected string ProjectName = "Application";

    public CreateEventProgram()
    {
        Namespace = "";
        TemplateFilesDictionary = new Dictionary<string, string> {
            {"Event", "./Programs/CreateEventTemplates/event.template"},
            {"EventHandler", "./Programs/CreateEventTemplates/handler.template"},
        };

        PathDictionary = new Dictionary<string, string> {
            {"Event", "../#Path#/#Event#Event.cs"},
            {"EventHandler", "../#Path#/#Event#EventHandler.cs"},
        };
    }

    protected override string ReplaceTemplates(string template)
    {
        return template.Replace("#Event#", EventName)
            .Replace("#Namespace#", Namespace)
            .Replace("#Path#", Path);
    }

    protected override void ReadArgs(string[] args)
    {
        ReadEventName(args);
        UpdatePath();
    }

    protected virtual void ReadEventName(string[] args)
    {
        var eventFullname = args[1];

        EventName = eventFullname[(eventFullname.LastIndexOf('.') + 1)..];
        Namespace = $"{ProjectName}.{eventFullname}";
    }

    protected override void PrintUsage()
    {
        Console.WriteLine("Usage: create:event [event]");
    }

    public override string Name()
    {
        return "create:event";
    }

    public override string Description()
    {
        return "Creates an event with event handler.";
    }

    public override string Section()
    {
        return "Create";
    }
}