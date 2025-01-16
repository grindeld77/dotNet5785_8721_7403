namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; } // Call ID
    public CallType Type { get; set; } // Call Type
    public string FullAddress { get; set; } // Full Address
    public DateTime OpenTime { get; set; } // Call Open Time
    public DateTime AssignedTime { get; set; } // Call Assigned Time
    public DateTime? ClosedTime { get; set; } // Call Closed Time
    public CompletionStatus? Status { get; set; } // Completion Status

    public override string ToString()
    {
        return $"ID: {Id}, " +
               $"Type: {Type}, " +
               $"Full Address: {FullAddress}, " +
               $"Open Time: {OpenTime}, " +
               $"Assigned Time: {AssignedTime}, " +
               $"Closed Time: {ClosedTime?.ToString() ?? "N/A"}, " +
               $"Status: {Status}";
    }
}
