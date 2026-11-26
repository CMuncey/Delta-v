namespace Content.Shared._DV.Janitor.Constants;

public static class JanitorServiceRequestConstants
{
    public enum Command
    {
        Request,
        Dismiss,
        Link
    };

    // Department for the request/dismiss commands
    public const string Department = "department";

    // Request state (bool) for request commands
    public const string Request = "request";
}
