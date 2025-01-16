namespace Dal;
internal static class Config
{
    internal const int startCallId = 0;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmentId = 0;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1); // Default Risk Range set to 1 hour
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;  // Reset the system clock to the current time
        RiskRange = TimeSpan.FromHours(1);  // Reset Risk Range to its default value
    }

}
