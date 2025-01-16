namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public int TotalCompletedCalls { get; set; }
    public int TotalCancelledCalls { get; set; }
    public int TotalExpiredCalls { get; set; }
    public int? CurrentCallId { get; init; }
    public CallType CurrentCallType { get; set; }

    public override string ToString()
    {
        return $"ID: {Id}, Full Name: {FullName ?? "N/A"}, " +
               $"Total Completed Calls: {TotalCompletedCalls.ToString() ?? "N/A"}, " +
               $"Total Cancelled Calls: {TotalCancelledCalls.ToString() ?? "N/A"}, " +
               $"Total Expired Calls: {TotalExpiredCalls.ToString() ?? "N/A"}, " +
               $"Current Call ID: {CurrentCallId?.ToString() ?? "N/A"}, " +
               $"Current Call Type: {CurrentCallType.ToString() ?? "N/A"}";
    }
}
