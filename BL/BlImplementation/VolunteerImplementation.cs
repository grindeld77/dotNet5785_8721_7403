using static Helpers.Tools;

namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;
using System;
using System.Security.AccessControl;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Logs in a volunteer to the system.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlInvalidIdentificationException"></exception>
    string BlApi.IVolunteer.Login(int id, string password)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
        {
            throw new BO.BlInvalidIdentificationException("This ID is not found.");
        }

        return volunteer.Role.ToString();
    }

    /// <summary>
    /// Retrieves a collection of active volunteers in the system.
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="VolunteerParameter"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerable<BO.VolunteerInList> BlApi.IVolunteer.GetVolunteers(bool? isActive, BO.VolunteerFieldVolunteerInList? VolunteerParameter, BO.CallType? type)
    {
        IEnumerable<DO.Volunteer> doVolunteers;
        IEnumerable<BO.VolunteerInList> volunteerInLists = null;

        if (isActive.HasValue)
        {
            lock (AdminManager.BlMutex) //stage 7
                doVolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive.Value);
        }
        else
        {
            lock (AdminManager.BlMutex) //stage 7
                doVolunteers = _dal.Volunteer.ReadAll();
        }

        if (doVolunteers != null)
        {
            volunteerInLists = doVolunteers.Select(v => VolunteerManager.converterFromDoToBoVolunteerInList(v));
        }

        if (volunteerInLists != null && VolunteerParameter.HasValue)
        {
            switch (VolunteerParameter.Value)
            {
                case BO.VolunteerFieldVolunteerInList.FullName:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.FullName);
                    break;
                case BO.VolunteerFieldVolunteerInList.TotalCompletedCalls:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.TotalCompletedCalls);
                    break;
                case BO.VolunteerFieldVolunteerInList.TotalCancelledCalls:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.TotalCancelledCalls);
                    break;
                case BO.VolunteerFieldVolunteerInList.TotalExpiredCalls:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.TotalExpiredCalls);
                    break;
                case BO.VolunteerFieldVolunteerInList.CurrentCallId:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.CurrentCallId);
                    break;
                case BO.VolunteerFieldVolunteerInList.CurrentCallType:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.CurrentCallType);
                    break;
                default:
                    volunteerInLists = volunteerInLists.OrderBy(v => v.Id);
                    break;
            }
        }
        if (volunteerInLists != null && type.HasValue)
        {
            switch (type.Value)
            {
                case BO.CallType.NotAllocated:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.NotAllocated);
                    break;
                case BO.CallType.MedicalEmergency:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.MedicalEmergency);
                    break;
                case BO.CallType.PatientTransport:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.PatientTransport);
                    break;
                case BO.CallType.TrafficAccident:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.TrafficAccident);
                    break;
                case BO.CallType.FirstAid:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.FirstAid);
                    break;
                case BO.CallType.Rescue:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.Rescue);
                    break;
                case BO.CallType.FireEmergency:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.FireEmergency);
                    break;
                case BO.CallType.CardiacEmergency:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.CardiacEmergency);
                    break;
                case BO.CallType.Poisoning:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.Poisoning);
                    break;
                case BO.CallType.AllergicReaction:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.AllergicReaction);
                    break;
                case BO.CallType.MassCausalities:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.MassCausalities);
                    break;
                case BO.CallType.TerrorAttack:
                    volunteerInLists = volunteerInLists.Where(v => v.CurrentCallType == BO.CallType.TerrorAttack);
                    break;
                case BO.CallType.None:
                    break;
            }
        }
        return volunteerInLists ?? Enumerable.Empty<BO.VolunteerInList>();
    }

    /// <summary>
    /// get the Volunteer details 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralException"></exception>
    BO.Volunteer BlApi.IVolunteer.GetVolunteerDetails(int id)
    {
        try
        {
            DO.Volunteer doVolunteer;
            lock (AdminManager.BlMutex) //stage 7
                doVolunteer = _dal.Volunteer.Read(id);

            BO.Volunteer v = VolunteerManager.converterFromDoToBoVolunteer(doVolunteer);
            return v;
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while retrieving volunteer details.", ex);
        }
    }

    /// <summary>
    /// Updates a volunteer in the system.
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="volunteer"></param>
    /// <exception cref="BO.BlInvalidRequestException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="BO.BloesNotExistException"></exception>
    /// <exception cref="BO.BlGeneralException"></exception>
    /// 
    public async Task UpdateVolunteer(int requesterId, Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Volunteer doVolunteere;
            lock (AdminManager.BlMutex)
                doVolunteere = _dal.Volunteer.Read(requesterId);

            if (requesterId != volunteer.Id && doVolunteere.Role != DO.Role.Admin)
                throw new BO.BlInvalidRequestException("You are not authorized to update this volunteer.");

            VolunteerManager.ValidateVolunteerData(volunteer);

            // יצירת אובייקט מעודכן
            DO.Volunteer doVolunteerNew = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName ?? doVolunteere.FullName,
                MobilePhone = volunteer.PhoneNumber ?? doVolunteere.MobilePhone,
                Email = volunteer.Email ?? doVolunteere.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = doVolunteere.IsActive,
                Password = doVolunteere.Password,
                CurrentAddress = volunteer.FullAddress ?? doVolunteere.CurrentAddress,
                Latitude = doVolunteere.Latitude,
                Longitude = doVolunteere.Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall ?? doVolunteere.MaxCallDistance,
                DistancePreference = doVolunteere.DistancePreference,
            };

            // שמירת העדכון בבסיס הנתונים ללא שינוי קואורדינטות
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(doVolunteerNew);

            // עדכון צופים
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteere.Id);
            VolunteerManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyListUpdated();

            // חישוב קואורדינטות בצורה אסינכרונית
            var updatedVolunteer = await Tools.UpdateCoordinatesForVolunteerAsync(doVolunteerNew, volunteer.FullAddress);

            // עדכון ה-DAL עם המתנדב המעודכן
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BloesNotExistException($"Volunteer with ID {volunteer.Id} does not exist.");
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while updating the volunteer.", ex);
        }
    }


    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer"></param>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    /// <exception cref="BO.BlException"></exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // stage 7
        VolunteerManager.ValidateVolunteerData(volunteer); // Validate volunteer data

        try
        {
            // יצירת אובייקט ה-DO ללא קואורדינטות
            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                Password = volunteer.PasswordHash, // שמירת הסיסמה כפי שהיא
                CurrentAddress = volunteer.FullAddress,
                Latitude = null, // ערכים ראשוניים כ-null
                Longitude = null,
                MaxCallDistance = volunteer.MaxDistanceForCall,
                DistancePreference = (DO.DistanceType)volunteer.DistanceType,
            };

            // שמירת הנתונים הראשוניים ב-DAL
            lock (AdminManager.BlMutex)
            {
                _dal.Volunteer.Create(doVolunteer);
            }

            // עדכון התצוגה
            VolunteerManager.Observers.NotifyListUpdated();

            // קריאה לחישוב הקואורדינטות בצורה אסינכרונית
            _ = UpdateCoordinatesForVolunteerAsync(doVolunteer, volunteer.FullAddress)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var updatedVolunteer = task.Result;
                        lock (AdminManager.BlMutex)
                        {
                            _dal.Volunteer.Update(updatedVolunteer); // עדכון הקואורדינטות ב-DAL
                        }
                        VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id); // עדכון תצוגה
                    }
                    else if (task.IsFaulted)
                    {
                        // טיפול בשגיאה (למשל: כתובת לא תקינה, בעיית רשת)
                        // אפשר להוסיף לוג או הודעה לממשק המשתמש
                        Console.WriteLine($"Failed to update coordinates for volunteer {volunteer.Id}: {task.Exception}");
                    }
                });
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id} already exists.");
        }
        catch (DO.DalException)
        {
            throw new BO.BlException("An error occurred while updating the volunteer.");
        }
    }




    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="BO.BlInvalidOperationException"></exception>
    /// <exception cref="BO.BloesNotExistException"></exception>
    /// <exception cref="BO.BlGeneralException"></exception>
    void BlApi.IVolunteer.DeleteVolunteer(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Assignment chak;
        lock (AdminManager.BlMutex) //stage 7
            chak = _dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == id);
        if (chak != null)
        {
            throw new BO.BlInvalidOperationException("The volunteer is assigned to a call and cannot be deleted.");
        }
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated(); // Notify observers that the volunteer list has been updated.
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BloesNotExistException($"Volunteer with ID {id} does not exist.");
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while deleting the volunteer.", ex);
        }
    }

    #region Stage 5
    // Adds an observer for the volunteer list.
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5

    // Adds an observer for a specific volunteer.
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5

    // Removes an observer for the volunteer list.
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5

    // Removes an observer for a specific volunteer.
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5



    #endregion Stage 5
}
