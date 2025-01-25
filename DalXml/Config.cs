using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";

    internal const int startCallId = 0;
    internal const int startAssignmentId = 0;

    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; } = TimeSpan.FromHours(1); // Default Risk Range set to 1 hour

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void Reset()
    {
        NextCallId = startCallId;
        NextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;  // Reset the system clock to the current time
        RiskRange = TimeSpan.FromHours(1);  // Reset Risk Range to its default value
    }

}
