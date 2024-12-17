namespace BlImplementation;
using BlApi;
using BO;
/*
 *     int Id,
string FullName,
string MobilePhone,
string Email,
Role Role,
bool IsActive,
string? Password = null,
string? CurrentAddress = null,
double? Latitude = null,
double? Longitude = null,
double? MaxCallDistance = null,
DistanceType DistancePreference = DistanceType.Aerial
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
public BO.CallInProgress? CurrentCall { get; set; }//קריאה נוכחית*/
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    string IVolunteer.Login(string username, string password)
    {

            var volunteer = _dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.FullName == username && v.Password == password)
                ?? throw new BO.BlInvalidIdentificationException("The username or ID entered is invalid.");
            return volunteer.Role.ToString();
    }
    IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, VolunteerFieldVolunteerInList? VolunteerParameter)
    {
        List<BO.Volunteer> volunteers;
        IEnumerable<DO.Volunteer> doVolunteers;
        try
        {
            

            if (isActive.HasValue)
            {
                doVolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive.Value);
            }
            else
            {
                doVolunteers = _dal.Volunteer.ReadAll();
            }
            volunteers= doVolunteers is null ? null : doVolunteers.Select(v => new BO.Volunteer
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive,
                PhoneNumber = v.MobilePhone,
                Email = v.Email,
                Role = (Role)v.Role,
                PasswordHash = v.Password,
                FullAddress = v.CurrentAddress,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                MaxDistanceForCall = v.MaxCallDistance,
                DistanceType = (DistanceType)v.DistancePreference,
            }).ToList();


            if (VolunteerParameter.HasValue)
            {
                switch (VolunteerParameter.Value)
                {
                    case VolunteerFieldVolunteerInList.TotalCompletedCalls:
                        // סדר לפי סך השיחות שהושלמו בהצלחה
                        volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Completed));
                        break;
                    case VolunteerFieldVolunteerInList.TotalCancelledCalls:
                        // סדר לפי סך השיחות שבוטלו
                        volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Cancelled));
                        break;
                    case VolunteerFieldVolunteerInList.TotalExpiredCalls:
                        // סדר לפי סך השיחות שפג תוקפן
                        volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Expired));
                        break;
                    // הוסף מקרים נוספים לפי שדות ה-VolunteerField
                    case VolunteerFieldVolunteerInList.CurrentCallId:
                        // מיון לפי מזהה השיחה הנוכחית (צריך להגדיר לוגיקה)
                        break;
                    case VolunteerFieldVolunteerInList.CurrentCallType:
                        // מיון לפי סוג השיחה הנוכחית (צריך להגדיר לוגיקה)
                        break;
                    default:
                        break;
                }



            }
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while retrieving volunteers.", ex);
        }
    }
    /*IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, VolunteerFieldVolunteerInList? VolunteerParameter)
{
    List<BO.Volunteer> volunteers;

    try
    {
        // קבל את רשימת המתנדבים מה-DAL
        if (isActive.HasValue)
        {
            doVolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive.Value);
        }
        else
        {
            doVolunteers = _dal.Volunteer.ReadAll();
        }

        // המרת רשימת המתנדבים ל-BO.Volunteer (אופציונלי)
        volunteers = doVolunteers?.Select(v => new BO.Volunteer
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            PhoneNumber = v.MobilePhone,
            Email = v.Email,
            Role = (Role)v.Role,
            PasswordHash = v.Password,
            FullAddress = v.CurrentAddress,
            Latitude = v.Latitude,
            Longitude = v.Longitude,
            MaxDistanceForCall = v.MaxCallDistance,
            DistanceType = (DistanceType)v.DistancePreference,
        }).ToList();

        // סינון ומיון לפי VolunteerParameter
        if (VolunteerParameter.HasValue)
        {
            switch (VolunteerParameter.Value)
            {
                case VolunteerFieldVolunteerInList.TotalCompletedCalls:
                    // סדר לפי סך השיחות שהושלמו בהצלחה
                    volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Completed));
                    break;
                case VolunteerFieldVolunteerInList.TotalCancelledCalls:
                    // סדר לפי סך השיחות שבוטלו
                    volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Cancelled));
                    break;
                case VolunteerFieldVolunteerInList.TotalExpiredCalls:
                    // סדר לפי סך השיחות שפג תוקפן
                    volunteers = volunteers.OrderByDescending(v => v.Calls.Count(c => c.Status == DO.CallStatus.Expired));
                    break;
                // הוסף מקרים נוספים לפי שדות ה-VolunteerField
                case VolunteerFieldVolunteerInList.CurrentCallId:
                    // מיון לפי מזהה השיחה הנוכחית (צריך להגדיר לוגיקה)
                    break;
                case VolunteerFieldVolunteerInList.CurrentCallType:
                    // מיון לפי סוג השיחה הנוכחית (צריך להגדיר לוגיקה)
                    break;
                default:
                    break;
            }
        }

        // המרת רשימת המתנדבים ל-BO.VolunteerInList
        var result = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            // הוסף שדות נוספים לפי הצורך
            TotalCompletedCalls = v.Calls.Count(c => c.Status == DO.CallStatus.Completed)
        });

        return result;
    }
    catch (DO.DalException ex)
    {
        throw new BO.BlException("An error occurred while retrieving volunteers.", ex);
    }
}*/
    void IVolunteer.AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                Password = volunteer.PasswordHash, // חשוב להאריך כאן את הטיפול בסיסמאות (הצפנה, בדיקות תקפות)
                Role = volunteer.Role,
                IsActive = volunteer.IsActive,
                // ... הוסף את שאר המאפיינים של DO.Volunteer 
            };
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException("Volunteer with this ID already exists.");
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while adding the volunteer.", ex);
        }
    }

    void IVolunteer.DeleteVolunteer(int id)
    {
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.");
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while deleting the volunteer.", ex);
        }
    }

    BO.Volunteer IVolunteer.GetVolunteerDetails(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id);
            if (doVolunteer == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.");
            }

            return new BO.Volunteer
            {
                Id = doVolunteer.Id,
                FullName = doVolunteer.FullName,
                // ... הוסף את שאר המאפיינים של BO.Volunteer 
            };
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while retrieving volunteer details.", ex);
        }
    }



    void IVolunteer.UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                // ... הוסף את שאר המאפיינים של DO.Volunteer 
            };
            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteer.Id} does not exist.");
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while updating the volunteer.", ex);
        }
    }
}