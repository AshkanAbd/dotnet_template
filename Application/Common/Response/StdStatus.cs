namespace Application.Common.Response;

public static class StdStatus
{
    private static readonly IDictionary<string, int> Status = new Dictionary<string, int> {
        {"success", 200},
        {"default", 200},
        {"error", 400},
        {"syntax-error", 400},
        {"unauthorized-error", 401},
        {"access-denied-error", 403},
        {"notfound-error", 404},
        {"validation-error", 422},
        {"internal-error", 500},
    };

    public static int Get(string status)
    {
        if (!Status.ContainsKey(status)) {
            status = "default";
        }

        return Status[status];
    }
}