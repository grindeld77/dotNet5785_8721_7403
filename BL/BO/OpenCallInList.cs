namespace BO;

public class OpenCallInList
{
    public int Id { get; init; } // Call ID  
    public CallType Type { get; set; } // Call Type  
    public string? Description { get; set; } // Call Description  
    public string FullAddress { get; set; } // Full Address  
    public DateTime OpenTime { get; set; } // Call Open Time  
    public DateTime? MaxEndTime { get; set; } // Maximum End Time  
    public double DistanceFromVolunteer { get; set; } // Distance from Volunteer  
    public override string ToString()
    {
        return $"ID: {Id}, " +
               $"Type: {Type}, " +
               $"Description: {Description ?? "N/A"}, " +
               $"Full Address: {FullAddress}, " +
               $"Open Time: {OpenTime}, " +
               $"Max End Time: {MaxEndTime?.ToString() ?? "N/A"}, " +
               $"Distance From Volunteer: {DistanceFromVolunteer}";
    }
}
