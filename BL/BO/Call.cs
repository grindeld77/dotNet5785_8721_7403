namespace BO;

public class Call
{
    public int Id { get; init; } // Call ID
    public CallType Type { get; set; } // Call type
    public string? Description { get; set; } // Call description
    public string? FullAddress { get; set; } // Full address
    public double? Latitude { get; set; } // Latitude
    public double? Longitude { get; set; } // Longitude
    public DateTime OpenTime { get; set; } // Call open time
    public DateTime MaxEndTime { get; set; } // Maximum end time
    public CallStatus Status { get; set; } // Call status
    public IEnumerable<BO.CallAssignInList>? Assignments { get; set; } = null; // List of assignments

    public override string ToString()
    {
        return $"Call: {Id}\n" +
               $"Type: {Type}\n" +
               $"Description: {Description}\n" +
               $"Address: {FullAddress}\n" +
               $"Open Time: {OpenTime}\n" +
               $"Max End Time: {MaxEndTime}\n" +
               $"Status: {Status}\n" +
               $"Assignments: {(Assignments != null ? string.Join("\n", Assignments) : "None")}";
    }
}
