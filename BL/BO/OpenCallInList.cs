namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }//קוד קריאה
    public CallType Type { get; set; }//סוג קריאה
    public string? Description { get; set; }//תיאור הקריאה
    public string FullAddress { get; set; }//כתובת מלאה
    public DateTime OpenTime { get; set; }//זמן פתיחת הקריאה
    public DateTime? MaxEndTime { get; set; }//זמן סיום מירבי
    public double DistanceFromVolunteer { get; set; }//מרחק מהמתנדב
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
