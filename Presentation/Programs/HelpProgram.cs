using System.Text;

namespace Presentation.Programs;

public class HelpProgram : AbstractProgram
{
    public override void Run(string[] args)
    {
        var maxCommandLenght = ProgramsDictionary.Keys
            .OrderByDescending(x => x.Length)
            .Select(x => x.Length)
            .First() + 5;

        ProgramsDictionary
            .OrderBy(x => x.Value.Section())
            .Select(x => x.Value.Section())
            .Distinct()
            .ToList()
            .ForEach(x => PrintSection(x, maxCommandLenght));
    }

    private static void PrintSection(string section, int maxCommandLenght)
    {
        Console.WriteLine("{0}:", section);

        foreach (var (key, value) in ProgramsDictionary
                     .Where(x => x.Value.Section() == section)) {
            var builder = new StringBuilder();

            builder.Append('\t');
            builder.Append(key);
            for (var i = maxCommandLenght; i > key.Length; i--) {
                builder.Append(' ');
            }

            builder.Append(value.Description());

            Console.WriteLine(builder.ToString());
        }
    }

    public override string Name()
    {
        return "Help";
    }

    public override string Description()
    {
        return "Shows this messaage";
    }

    public override string Section()
    {
        return "Help";
    }
}