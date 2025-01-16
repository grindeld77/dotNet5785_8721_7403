namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; set; } // Volunteer ID
    public string? VolunteerName { get; set; } // Volunteer Name
    public DateTime StartTime { get; set; } // Start Time of the call
    public DateTime? EndTime { get; set; } // End Time of the call
    public CompletionStatus? Status { get; set; } // Status of the call
    public override string ToString()
    {
        return $"Volunteer ID: {VolunteerId?.ToString() ?? "N/A"}, " +
               $"Volunteer Name: {VolunteerName ?? "N/A"}, " +
               $"Start Time: {StartTime}, " +
               $"End Time: {EndTime?.ToString() ?? "N/A"}, " +
               $"Status: {Status}";
    }
}
