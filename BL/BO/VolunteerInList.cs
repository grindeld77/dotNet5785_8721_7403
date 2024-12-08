namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }//קוד מתנדב
    public string FullName { get; set; }//שם מלא
    public bool IsActive { get; set; }//האם המתנדב פעיל
    public int TotalCompletedCalls { get; set; }//סה"כ שיחות שהתנדב להן
    public int TotalCancelledCalls { get; set; }//סה"כ שיחות שביטל
    public int TotalExpiredCalls { get; set; }//סה"כ שיחות שלא טופלו בזמן
    public int? CurrentCallId { get; init; }//קוד הקריאה הנוכחית של המתנדב אם קיימת
    public CallType CurrentCallType { get; set; }//סוג הקריאה הנוכחית של המתנדב
}
