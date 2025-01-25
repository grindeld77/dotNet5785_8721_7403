using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;
internal static class Config
{
    internal const int startCallId = 0;
    private static int nextCallId = startCallId;
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => Config.nextCallId++; 
    }

    internal const int startAssignmentId = 0;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId 
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextAssignmentId++; 
    }

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; 
    } = DateTime.Now;
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; 
    } = TimeSpan.FromHours(1); // Default Risk Range set to 1 hour

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;  // Reset the system clock to the current time
        RiskRange = TimeSpan.FromHours(1);  // Reset Risk Range to its default value
    }

}
