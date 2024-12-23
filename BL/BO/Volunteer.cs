using DO;

namespace BO
{

    public class Volunteer
    {
        public int Id { get; init; }//קוד מתנדב
        public string FullName { get; set; }//שם מלא
        public string PhoneNumber { get; set; }//טלפון
        public string Email { get; set; }//כתובת מייל   
        public string? PasswordHash { get; set; }//סיסמה מוצפנת
        public string? FullAddress { get; set; }//כתובת מלאה
        public double? Latitude { get; set; }//קו רוחב
        public double? Longitude { get; set; }//קו אורך
        public Role Role { get; set; }//ENUM: Admin, Volunteer
        public bool IsActive { get; set; }//האם המתנדב פעיל
        public double? MaxDistanceForCall { get; set; }//מרחק מקסימלי לקריאה
        public DistanceType DistanceType { get; set; } = DistanceType.Aerial;//ENUM: Aerial, Road
        public int TotalCompletedCalls { get; set; }//סה"כ שיחות
        public int TotalCancelledCalls { get; set; }//סה"כ שיחות שבוטלו
        public int TotalExpiredCalls { get; set; }//סה"כ שיחות שלא טופלו בזמן
        public BO.CallInProgress? CurrentCall { get; set; }//קריאה נוכחית

        public override string ToString()
        {
            return 
                   $"Volunteer: {FullName} (ID: {Id})\n" +
                   $"Phone: {PhoneNumber}\n" +
                   $"Email: {Email}\n" +
                   $"Address: {FullAddress}\n" +
                   $"Role: {(Role == Role.Admin ? "Admin" : "Volunteer")}\n" +
                   $"Active: {(IsActive ? "Yes" : "No")}\n" +
                   $"Max distance for call: {MaxDistanceForCall} {DistanceType}\n" +
                   $"Total Completed Calls: {TotalCompletedCalls}\n" +
                   $"Total Cancelled Calls: {TotalCancelledCalls}\n" +
                   $"Total Expired Calls: {TotalExpiredCalls}\n" +
                   $"Current call: {(CurrentCall != null ? CurrentCall.ToString() : "None")}";
        }
    }

    public class CallInProgress
    {
        public int Id { get; init; }// קוד קריאה של ההקצאה
        public int CallId { get; init; }// קוד קריאה
        public CallType Call { get; set; } // ENUM: Food, Medicine, Other
        public string? Description { get; set; }// תיאור הקריאה
        public string FullAddress { get; set; }// כתובת מלאה
        public DateTime OpenTime { get; set; }// זמן פתיחת הקריאה
        public DateTime? MaxCompletionTime { get; set; } // זמן סיום מירבי
        public DateTime StartTime { get; set; }// זמן התחלת הטיפול
        public double DistanceFromVolunteer { get; set; } // מרחק מהמתנדב
        public CallStatus Status { get; set; } // ENUM: InProgress, AtRisk


        public override string ToString()
        {
            return $"Call in Progress: {Id}\n" +
                   $"Call: {CallId}\n" +
                   $"Type: {Call}\n" +
                   $"Description: {Description}\n" +
                   $"Address: {FullAddress}\n" +
                   $"Open Time: {OpenTime}\n" +
                   $"Max Completion Time: {(MaxCompletionTime.HasValue ? MaxCompletionTime.Value : "N/A")}\n" +
                   $"Start Time: {StartTime}\n" +
                   $"Distance from Volunteer: {DistanceFromVolunteer} km\n" +
                   $"Status: {Status}";
        }
    }


}
