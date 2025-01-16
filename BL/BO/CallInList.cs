namespace BO;

public class CallInList
{
    public int? AssignmentId { get; set; } // Assignment code
    public int CallId { get; init; } // Call code
    public BO.CallType CallType { get; set; } // Call type
    public DateTime OpenTime { get; set; } // Call open time
    public TimeSpan? RemainingTime { get; set; } // Calculated remaining time
    public string? LastVolunteer { get; set; } // Name of the last volunteer who handled the call
    public TimeSpan? TotalHandlingTime { get; set; } // Total handling time for the call
    public CallStatus Status { get; set; } // Call status ENUM
    public int TotalAssignments { get; set; } // Total volunteers assigned to the call
    public override string ToString()
    {
        return $"ID: {AssignmentId?.ToString() ?? "N/A"}, " +
               $"Call Type: {CallType.ToString() ?? "N/A"}, " +
               $"Open Time: {OpenTime}, " +
               $"Remaining Time: {RemainingTime?.ToString(@"hh\:mm\:ss") ?? "N/A"}, " +
               $"Last Volunteer: {LastVolunteer ?? "N/A"}, " +
               $"Total Handling Time: {TotalHandlingTime?.ToString(@"hh\:mm\:ss") ?? "N/A"}, " +
               $"Status: {Status}, " +
               $"Total Assignments: {TotalAssignments}";
    }
}
