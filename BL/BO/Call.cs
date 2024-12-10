namespace BO;

public class Call
{
    public int Id { get; init; } //קוד קריאה
    public CallType Type { get; set; }//סוג קריאה
    public string? Description { get; set; }//תיאור הקריאה
    public string? FullAddress { get; set; }//כתובת מלאה
    public double? Latitude { get; set; }//קו רוחב
    public double? Longitude { get; set; }//קו אורך
    public DateTime OpenTime { get; set; }//זמן פתיחת הקריאה
    public DateTime MaxEndTime { get; set; }//זמן סיום מירבי
    public CallStatus Status { get; set; }// ENUM עבור סטטוס הקריאה
    public List<BO.CallAssignInList>? Assignments { get; set; } =null;//רשימת הקצאות
}
