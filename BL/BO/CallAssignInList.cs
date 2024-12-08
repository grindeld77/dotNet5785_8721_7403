namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; set; }//קוד מתנדב
    public string? VolunteerName { get; set; }//שם מתנדב
    public DateTime StartTime { get; set; }//זמן התחלת הטיפול
    public DateTime? EndTime { get; set; }//זמן סיום הטיפול
    public CompletionStatus? Status { get; set; }//סטטוס הטיפול
}
