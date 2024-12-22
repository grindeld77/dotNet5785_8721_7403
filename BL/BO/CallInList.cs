namespace BO;

public class CallInList
{
    public int? Id { get; set; }//קוד קריאה
    public int CallId { get; init; }//קוד קריאה
    public string CallType { get; set; }//סוג קריאה
    public DateTime OpenTime { get; set; }//זמן פתיחת הקריאה
    public TimeSpan? RemainingTime { get; set; } // מחושב
    public string? LastVolunteer { get; set; }//שם המתנדב האחרון שטיפל בקריאה
    public TimeSpan? TotalHandlingTime { get; set; }//סה"כ זמן טיפול בקריאה
    public CallStatus Status { get; set; } // ENUM עבור סטטוס הקריאה
    public int TotalAssignments { get; set; }//סה"כ מתנדבים שהוקצו לקריאה

    public override string ToString()
    {
        return $"ID: {Id?.ToString() ?? "N/A"}, " +
               $"Call Type: {CallType ?? "N/A"}, " +
               $"Open Time: {OpenTime}, " +
               $"Remaining Time: {RemainingTime?.ToString(@"hh\:mm\:ss") ?? "N/A"}, " +
               $"Last Volunteer: {LastVolunteer ?? "N/A"}, " +
               $"Total Handling Time: {TotalHandlingTime?.ToString(@"hh\:mm\:ss") ?? "N/A"}, " +
               $"Status: {Status}, " +
               $"Total Assignments: {TotalAssignments}";
    }
}
