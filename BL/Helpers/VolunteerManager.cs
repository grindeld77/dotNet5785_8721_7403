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
using static Helpers.Tools;

namespace Helpers;

internal static class VolunteerManager
{
    private static DalApi.IDal _dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 

    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, double radius = 6371)//פונקציה שמחשבת את המרחק בין שתי נקודות
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        lat1 = ToRadians(lat1);
        lat2 = ToRadians(lat2);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                  Math.Cos(lat1) * Math.Cos(lat2) *
                  Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = radius * c;

        return distance;
    }

    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    public static BO.VolunteerInList converterFromDoToBoVolunteerInList(DO.Volunteer v)
    {
        IEnumerable<DO.Call> calls;
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
        {
            assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == v.Id);
            calls = _dal.Call.ReadAll().Where(c => assignments.Any(a => a.CallId == c.Id));
        }

        BO.VolunteerInList volunteerInList = new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalCompletedCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.Handled),
            TotalCancelledCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel),
            TotalExpiredCalls = assignments.Count(a => a.CompletionStatus == DO.CompletionStatus.Expired),
            CurrentCallId = calls.FirstOrDefault(c => c.Status == DO.CallStatus.InProgress)?.Id ?? null,
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

        IEnumerable<DO.Assignment> assignment;
        lock (AdminManager.BlMutex) //stage 7
            assignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == v.Id);
        DO.Assignment assignmentOpen = null;
        if (assignment.Any())
        {
            lock (AdminManager.BlMutex) //stage 7
                assignmentOpen = _dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == v.Id && a.CompletionStatus == null);
        }

        DO.Call callOpen = null;
        if (assignmentOpen != null)
        {
            lock (AdminManager.BlMutex) //stage 7
                callOpen = (DO.Call)_dal.Call.Read(assignmentOpen.CallId);
        }
        if (callOpen != null)
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
                TotalCancelledCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel || a.CompletionStatus == DO.CompletionStatus.AdminCancel),
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
                TotalCancelledCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.SelfCancel || a.CompletionStatus == DO.CompletionStatus.AdminCancel),
                TotalExpiredCalls = assignment.Count(a => a.CompletionStatus == DO.CompletionStatus.Expired),
                CurrentCall = null
            };
            Observers.NotifyItemUpdated(v.Id);
            return volunteer;
        };
    }


    public static bool IsValidID(int ID)
    {
        string idStr = ID.ToString("D9");
        int sum = 0;
        int[] weights = { 1, 2, 1, 2, 1, 2, 1, 2 }; 

        for (int i = 0; i < idStr.Length - 1; i++)
        {
            int digit = int.Parse(idStr[i].ToString());
            int product = digit * weights[i];
            sum += product > 9 ? product - 9 : product;
        }

        return (10 - (sum % 10)) % 10 == int.Parse(idStr[8].ToString());
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
    {
        string hashedInput = HashPassword(inputPassword);
        return hashedInput == storedHashedPassword;
    }

    public static bool IsValidEmail(string email)
    {
        if (email == null)
            return true;
        string[] validDomains = { "com", "net", "org", "edu", "gov", "mil", "co.il", "fr", "de", "uk" };

        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.(" + string.Join("|", validDomains) + ")$");
        return emailRegex.IsMatch(email);
    }

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
        string pattern = @"^[a-zA-Zאבגדהוזחטיכךלמנסעפצקרשתךםןףץצדק]+(\s+[a-zA-Zאבגדהוזחטיכךלמנסעפצקרשתךםןףץצדק]+)*$";

        Regex regex = new Regex(pattern);
        return regex.IsMatch(name);
    }


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
    public static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(ch => "!@#$%^&*".Contains(ch));
        bool isLongEnough = password.Length >= 8;

        return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && isLongEnough;
    }

    public static string GetPositionBypas(string Password)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
             volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Password == Password);
        return volunteer.Role.ToString(); 
    }


    public static string GetPositionById(int ID)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
           volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == ID);
        return volunteer.Role.ToString();    

    }


    public static void ValidateVolunteerData(BO.Volunteer volunteer)
    {
        if (!IsPasswordStrong(volunteer.PasswordHash))
        {
            throw new ArgumentException("Password is not strong enough");
        }
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


    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    internal static void SimulateCourseRegistrationAndGrade() // stage 7
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        LinkedList<int> volunteersToUpdate = new(); // stage 7
        List<DO.Volunteer> doVolunteerList;

        lock (AdminManager.BlMutex) // stage 7
            doVolunteerList = _dal.Volunteer.ReadAll(st => st.IsActive).ToList();

        foreach (var doVolunteer in doVolunteerList)
        {
            int volunteerId = 0;

            lock (AdminManager.BlMutex) // stage 7
            {
                // אם אין למתנדב קריאה בטיפולו
                if (!CallManager.IsVolunteerBusy(doVolunteer.Id))
                {
                    // בחירה רנדומלית של קריאה לטיפול בהסתברות של 20%
                    if (s_rand.NextDouble() < 0.2)
                    {
                        var openCalls = CallManager.GetOpenCallForVolunteer(doVolunteer.Id);
                        if (openCalls.Any())
                        {
                            int randomIndex = s_rand.Next(0, openCalls.Count());
                            var selectedCall = openCalls.ElementAt(randomIndex);

                            // הקצאת קריאה למתנדב
                            //AssignmentManager.AssignVolunteerToCall(doVolunteer.Id, selectedCall.Id);
                            volunteerId = doVolunteer.Id;
                        }
                    }
                }
                else // אם למתנדב יש קריאה בטיפולו
                {
                    var activeAssignment = _dal.Assignment
                        .ReadAll(a => a.VolunteerId == doVolunteer.Id && a.CompletionTime == null)
                        .FirstOrDefault();

                    if (activeAssignment != null)
                    {
                        var elapsedTime = DateTime.Now - activeAssignment.EntryTime;

                        // בדיקת אם עבר "מספיק זמן"
                        var relatedCall = _dal.Call.Read(activeAssignment.CallId);
                        var estimatedTime = CalculateEstimatedTime(doVolunteer, relatedCall);

                        if (elapsedTime >= estimatedTime) // מספיק זמן
                        {
                            // סיום הקריאה
                            AssignmentManager.UpdateCallForVolunteer(doVolunteer.Id, activeAssignment.Id);

                            volunteerId = doVolunteer.Id;
                        }
                        else if (s_rand.NextDouble() < 0.1) // הסתברות של 10% לביטול
                        {
                            volunteerId = doVolunteer.Id;
                            AssignmentManager.CancelAssignment(volunteerId , activeAssignment.Id);
                        }
                    }
                }
            } // lock

            if (volunteerId != 0)
                volunteersToUpdate.AddLast(volunteerId);
        }

        foreach (int id in volunteersToUpdate)
            Observers.NotifyItemUpdated(id);
    }

    /// <summary>
    /// מחשב את הזמן המוערך לטיפול בקריאה, בהתבסס על מרחק וזמן רנדומלי נוסף
    /// </summary>
    /// <param name="volunteer"></param>
    /// <param name="call"></param>
    /// <returns></returns>
    private static TimeSpan CalculateEstimatedTime(DO.Volunteer volunteer, DO.Call call)
    {
        // חישוב מרחק (אפשר להוסיף כאן חישוב מרחק גיאוגרפי אם יש פונקציה קיימת)
        double distance = VolunteerManager.CalculateDistance(
            (double)volunteer.Latitude, (double)volunteer.Longitude,
            (double)call.Latitude, (double)call.Longitude
        );

        // תוספת זמן רנדומלית בין 10 ל-30 דקות
        double randomMinutes = s_rand.Next(10, 30);

        // זמן מוערך: זמן מבוסס מרחק + זמן רנדומלי
        return TimeSpan.FromMinutes(distance / 10 + randomMinutes); // לדוגמה: 10 קמ"ש
    }
}