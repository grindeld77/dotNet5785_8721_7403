using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helpers;

internal static class VolunteerManager
{
    private static DalApi.IDal _dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 

    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, double radius = 6371)//פונקציה שמחשבת את המרחק בין שתי נקודות
    {
        // המרה לרדיאנים
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        lat1 = ToRadians(lat1);
        lat2 = ToRadians(lat2);

        // חישוב ההאברסין
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                  Math.Cos(lat1) * Math.Cos(lat2) *
                  Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // חישוב המרחק
        double distance = radius * c;

        return distance;
    }

    private static double ToRadians(double angle)//המרה לרדיאנים
    {
        return angle * (Math.PI / 180);
    }

    public static BO.VolunteerInList converterFromDoToBoVolunteerInList(DO.Volunteer v)
    {
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == v.Id);
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll().Where(c => assignments.Any(a => a.CallId == c.Id));

        BO.VolunteerInList volunteerInList = new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName, 
            IsActive = v.IsActive,
            TotalCompletedCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.Handled),
            TotalCancelledCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel),
            TotalExpiredCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.Expired),
            CurrentCallId = calls.FirstOrDefault(c => c.Status == DO.CallStatus.InProgress) ?.Id ?? null, 
            CurrentCallType = (BO.CallType)(calls.FirstOrDefault(c => c.Status == DO.CallStatus.InProgress)?.Type ?? DO.CallType.None)
        };

        Observers.NotifyItemUpdated(v.Id); // stage 5

        return volunteerInList;
    }

    public static BO.Volunteer converterFromDoToBoVolunteer(DO.Volunteer v)
    {
        if (v == null)
        {
            return null;
        }

        IEnumerable<DO.Assignment> assignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == v.Id);
        DO.Assignment assignmentOpen = null;
        if (assignment.Any())
        {
            assignmentOpen = _dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == v.Id && a.CompletionTime == null);
        }

        DO.Call callOpen = null;
        if (assignmentOpen != null)
        {
            callOpen = (DO.Call)_dal.Call.Read(assignmentOpen.Id);
        }
        if (callOpen != null) {
            BO.Volunteer volunteer = new BO.Volunteer
            {
                Id = v.Id,
                FullName = v.FullName,
                PhoneNumber = v.MobilePhone,
                Email = v.Email,
                PasswordHash = v.Password,
                FullAddress = v.CurrentAddress,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                Role = (BO.Role)v.Role,
                IsActive = v.IsActive,
                MaxDistanceForCall = v.MaxCallDistance,
                DistanceType = (BO.DistanceType)v.DistancePreference,
                TotalCompletedCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.Handled),
                TotalCancelledCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel),
                TotalExpiredCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.Expired),
                CurrentCall = new BO.CallInProgress
                {
                    Id = assignmentOpen.Id,
                    CallId = assignmentOpen.CallId,
                    Call = (BO.CallType)callOpen.Type,
                    Description = callOpen.Description,
                    FullAddress = callOpen.Address,
                    OpenTime = callOpen.OpenedAt,
                    MaxCompletionTime = callOpen.MaxCompletionTime,
                    StartTime = assignmentOpen.EntryTime,
                    DistanceFromVolunteer = CalculateDistance((double)v.Latitude, (double)v.Longitude, callOpen.Latitude, callOpen.Longitude),
                    Status = (BO.CallStatus)callOpen.Status
                }
            };
            Observers.NotifyItemUpdated(v.Id);
            return volunteer;
        }
        else
        {
            BO.Volunteer volunteer = new BO.Volunteer
            {
                Id = v.Id,
                FullName = v.FullName,
                PhoneNumber = v.MobilePhone,
                Email = v.Email,
                PasswordHash = v.Password,
                FullAddress = v.CurrentAddress,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                Role = (BO.Role)v.Role,
                IsActive = v.IsActive,
                MaxDistanceForCall = v.MaxCallDistance,
                DistanceType = (BO.DistanceType)v.DistancePreference,
                TotalCompletedCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.Handled),
                TotalCancelledCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel),
                TotalExpiredCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.Expired),
                CurrentCall = null
            };
            Observers.NotifyItemUpdated(v.Id);
            return volunteer;
        };
    }

    /// <summary>
    /// בדיקת תקינות תעודת זהות
    /// </summary>
    public static bool IsValidID(int ID)
    {
        string idStr = ID.ToString("D9");
        int sum = 0;
        int[] weights = { 1, 2, 1, 2, 1, 2, 1, 2 }; // משקלים לספרות

        for (int i = 0; i < idStr.Length - 1; i++)
        {
            int digit = int.Parse(idStr[i].ToString());
            int product = digit * weights[i];
            sum += product > 9 ? product - 9 : product;
        }

        return (10 - (sum % 10)) % 10 == int.Parse(idStr[8].ToString());
    }

    /// <summary>
    /// בדיקת תקינות כתובת מייל
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (email == null)
            return true;
        // רשימת סיומות אימייל נפוצות 
        string[] validDomains = { "com", "net", "org", "edu", "gov", "mil", "co.il", "fr", "de", "uk" };

        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.(" + string.Join("|", validDomains) + ")$");
        return emailRegex.IsMatch(email);
    }

    /// <summary>
    /// בדיקה שמספר הטלפון תקין
    /// </summary>

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (phoneNumber == null)
            return true;
        string pattern = @"^(\+972|0)5[0-9]{8}$|^(0[2-9][0-9]{7})$";

        Regex regex = new Regex(pattern);
        return regex.IsMatch(phoneNumber);
    }


    public static bool IsValidName(string name)
    {
        if (name == null)
            return true;
        // ביטוי רגולרי לבדיקה האם המחרוזת מכילה רק אותיות עבריות או אנגליות, ומאפשר רווחים מרובים
        string pattern = @"^[a-zA-Zאבגדהוזחטיכךלמנסעפצקרשתךםןףץצדק]+(\s+[a-zA-Zאבגדהוזחטיכךלמנסעפצקרשתךםןףץצדק]+)*$";

        Regex regex = new Regex(pattern);
        return regex.IsMatch(name);
    }

    /// <summary>
    /// המרת מרחק למחרוזת Convert a distance type to a string
    /// </summary>
    public static string convertToString(DO.DistanceType distanceType)
    {
        return distanceType switch
        {
            DO.DistanceType.Aerial => "air",
            DO.DistanceType.Walking => "byCar",
            DO.DistanceType.Driving => "walk",

            _ => throw new NotImplementedException(),
        };
    }




    public static string ConvertAsciiToString(int[] numbers)
    {
        StringBuilder result = new StringBuilder();
        foreach (int number in numbers)
            result.Append((char)number);
        return result.ToString();
    }
    public static string crypto(string password)
    {
        if (password == null)
            return null;
        int[] asciiArray = password.Select(c => (int)c).ToArray();
        asciiArray = H(asciiArray);
        return ConvertAsciiToString(asciiArray);
    }
    //ehHash the password
    public static string decrypt(string password)
    {
        if (password == null)
            return null;
        int[] asciiArray = password.Select(c => (int)c).ToArray();
        asciiArray = h(asciiArray);
        return ConvertAsciiToString(asciiArray);
    }
    //Hash function
    public static int[] H(int[] password)
    {
        for (int i = 0; i < password.Length; i++)
            password[i] = (password[i] * 3) + 7;
        return password;
    }
    //enhance the password
    public static int[] h(int[] password)
    {
        for (int i = 0; i < password.Length; i++)
            password[i] = (password[i] - 7) / 3;
        return password;
    }
    public static bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
            return false;
        string specialCharPattern = @"[!@#$%^&*(),.?\:{ }|<>]";
        string upperCasePattern = @"[A-Z]";
        string numberPattern = @"[0-9]";

        bool hasSpecialChar = Regex.IsMatch(password, specialCharPattern);
        bool hasUpperCase = Regex.IsMatch(password, upperCasePattern);
        bool hasNumber = Regex.IsMatch(password, numberPattern);

        return hasSpecialChar && hasUpperCase && hasNumber;
    }
    /// <summary>
    /// מחזירה תפקיד משתמש עי ססמא
    /// </summary>
    /// <param name="Password"></param>
    /// <returns></returns>
    public static string GetPositionBypas(string Password)
    {
        var volunteer = _dal.Volunteer.ReadAll()
                                    .FirstOrDefault(v => v.Password == Password);//בדיקה בעזרת LINQ אם קיים מתנדב עם שם וסססמא 
        return volunteer.Role.ToString();    //   החזרת תפקיד המשתמש
    }

    /// <summary>
    /// מחזירה תפקיד משתמש עי תעודת זהות
    /// </summary>
    public static string GetPositionById(int ID)
    {
        var volunteer = _dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.Id == ID);//בדיקה בעזרת LINQ אם קיים מתנדב עם שם וסססמא 
        return volunteer.Role.ToString();    //   החזרת תפקיד המשתמש

    }

    /// <summary>
    ///  וזריקת חריגות פנימיות בהתאם בןדקת תקינות איימיל טלפון ותז
    /// </summary>
    /// <param name="volunteer"></param>
    public static void ValidateVolunteerData(BO.Volunteer volunteer)
    {
        if (!IsValidEmail(volunteer.Email))
        {
            throw new BO.InvalidEmailException("Invalid email address");
        }
        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
        {
            throw new BO.BlPoneNomber("Invalid phone number");
        }
        if (!IsValidID(volunteer.Id))
        {
            throw new BO.InvalidEmailException("Invalid ID number");
        }
        if (!IsValidName(volunteer.FullName))
        {
            throw new BO.InvalidNameException("Invalid Name");
        }
    }
    


    internal static void PeriodicVolunteersUpdates(object oldClock, DateTime newClock)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
        foreach (var volunteer in volunteers)
        {
            if (volunteer.IsActive)
            {
                IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteer.Id);
                IEnumerable<DO.Call> calls = _dal.Call.ReadAll().Where(c => assignments.Any(a => a.CallId == c.Id));
                BO.VolunteerInList volunteerInList = converterFromDoToBoVolunteerInList(volunteer);
                Observers.NotifyItemUpdated(volunteer.Id);
            }
        }
    }

}


/*
 using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class VolunteerManager
    {
        private static IDal s_dal = Factory.Get;

        #region checking functions

        // Check if the ID is valid according to the checksum algorithm
        public static bool IsValidID(int ID)
        {
            string IDStr = ID.ToString("D9");
            int sum = 0;
            for (int i = 0; i < IDStr.Length; i++)
            {
                int digit = int.Parse(IDStr[i].ToString());
                if (i % 2 == 1) digit *= 2;
                sum += (digit > 9) ? digit - 9 : digit;
            }
            return sum % 10 == 0;
        }

        // Check if the email is valid according to a simple regex
        public static bool IsValidEmail(string email)
        {
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+<span class="math-inline">"\);
return emailRegex\.IsMatch\(email\);
\}
/// <summary\>
/// /// Check if the phone number is valid according to a simple regex
/// </summary\>
/// <param name\="phoneNumber"\></param\>
/// <returns\></returns\>
public static bool IsValidPhoneNumber\(string phoneNumber\)
\{
string pattern \= @"^\(\\\+972\|0\)5\[0\-9\]\{8\}</span>|^(0[2-9][0-9]{7})$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }

        #endregion

        #region Getters

        // Get the number of treated open calls for a volunteer
        public static int getNumOCTreated(int volID)
        {
            var volunteerAssignmentList = s_dal.Assignment.ReadAll().Where(a => a.Volunteerid == volID);
            return volunteerAssignmentList.Count(a => a.EndType == DO.EndType.Finished);
        }

        // Get the number of canceled open calls for a volunteer
        public static int getNumOCCanceled(int volID)
        {
            var volunteerAssignmentList = s_dal.Assignment.ReadAll().Where(a => a.Volunteerid == volID);
            return volunteerAssignmentList.Count(a => a.EndType == DO.EndType.SelfCancellation);
        }

        // Get the number of expired open calls for a volunteer
        public static int getNumOCExpired(int volID)
        {
            var volunteerAssignmentList = s_dal.Assignment.ReadAll().Where(a => a.Volunteerid == volID);
            return volunteerAssignmentList.Count(a => a.EndType == DO.EndType.Expired);
        }

        /// <summary>
        /// /// Get the call in progress for a volunteer
        /// </summary>
        /// <param name="vol"></param>
        /// <returns></returns>
        public static BO.CallInProgress? buildCallInProgress(int vol)
        {
            int callID = GetNOCInProgress(vol) ?? -1;
            if (callID == -1) return null;
            var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.Volunteerid == vol && a.Callid == callID);
            if (assignment == null) return null;
            var volunteer = s_dal.Volunteer.Read(vol);
            var call = s_dal.Call.Read(callID);
            if (volunteer == null || call == null) return null;
            return new BO.CallInProgress
            {
                Id = assignment.Id,
                CallId = call.Id,
                CallType = (BO.CallType)call.CallType,
                ReadingDescription = call.ReadingDescription,
                Address = call.Address,
                OpeningTime = call.OpeningTime,
                MaxTime = call.MaxTime,
                Beginning = assignment.Beginning,
                Status = (CallStatus)CallManager.GetCallStatus(call, s_dal.Assignment.ReadAll(a => a.Volunteerid == vol)),
                Distance = Tools.DistanceCalculator
 */