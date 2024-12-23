namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }//קוד קריאה
    public CallType Type { get; set; }//סוג קריאה
    public string FullAddress { get; set; }//כתובת מלאה
    public DateTime OpenTime { get; set; }//זמן פתיחת הקריאה
    public DateTime AssignedTime { get; set; }//זמן התחלת הטיפול בקריאה
    public DateTime? ClosedTime { get; set; } // זמן סיום הטיפול בקריאה
    public CompletionStatus? Status { get; set; } // ENUM עבור סוגי סיום
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
