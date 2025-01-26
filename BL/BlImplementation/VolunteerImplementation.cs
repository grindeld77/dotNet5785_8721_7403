namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Security.AccessControl;


internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Logs in a volunteer to the system.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlInvalidIdentificationException"></exception>
    string IVolunteer.Login(int id, string password)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
        {
            throw new BO.BlInvalidIdentificationException("This ID is not found.");
        }

        // הצפנת הסיסמה שהוזנה והשוואה
        string hashedPassword = VolunteerManager.HashPassword(password);
        if (volunteer.Password != hashedPassword)
        {
            throw new BO.BlInvalidIdentificationException("The password is incorrect.");
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
    IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, BO.VolunteerFieldVolunteerInList? VolunteerParameter, BO.CallType? type)
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
    BO.Volunteer IVolunteer.GetVolunteerDetails(int id)
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
    void IVolunteer.UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // בדיקה אם הסימולטור פועל

        try
        {
            DO.Volunteer doVolunteere;
            lock (AdminManager.BlMutex) // שליפת נתוני המתנדב הקיים
                doVolunteere = _dal.Volunteer.Read(requesterId);

            // בדיקת הרשאות
            if (requesterId != volunteer.Id && doVolunteere.Role != DO.Role.Admin)
                throw new BO.BlInvalidRequestException("You are not authorized to update this volunteer.");

            VolunteerManager.ValidateVolunteerData(volunteer); // ולידציה לנתוני המתנדב
            DO.Role Pos = doVolunteere.Role;

            if ((doVolunteere.Role.ToString() != volunteer.Role.ToString()) && Pos != DO.Role.Admin)
            {
                throw new Exception("A volunteer could not change roles.");
            }

            // יצירת אובייקט מעודכן
            DO.Volunteer doVolunteerNew = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName ?? doVolunteere.FullName,
                MobilePhone = volunteer.PhoneNumber ?? doVolunteere.MobilePhone,
                Email = volunteer.Email ?? doVolunteere.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = doVolunteere.IsActive,
                Password = !string.IsNullOrEmpty(volunteer.PasswordHash)
                    ? VolunteerManager.HashPassword(volunteer.PasswordHash) // הצפנת סיסמה אם מעודכנת
                    : doVolunteere.Password, // אחרת שמירת הסיסמה הקיימת
                CurrentAddress = volunteer.FullAddress ?? doVolunteere.CurrentAddress,
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude == 0
                    ? doVolunteere.Latitude
                    : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude == 0
                    ? doVolunteere.Longitude
                    : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall ?? doVolunteere.MaxCallDistance,
                DistancePreference = doVolunteere.DistancePreference,
            };

            // שמירת העדכון בבסיס הנתונים
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(doVolunteerNew);

            // עדכון צופים
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteere.Id);
            VolunteerManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyListUpdated();
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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        VolunteerManager.ValidateVolunteerData(volunteer); // Validate volunteer data
        try
        {
            // הצפנת הסיסמה
            string hashedPassword = VolunteerManager.HashPassword(volunteer.PasswordHash);

            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                Password = hashedPassword, // שמירת הסיסמה המוצפנת
                CurrentAddress = volunteer.FullAddress,
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude == 0 ? null
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude == 0 ? null
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall,
                DistancePreference = (DO.DistanceType)volunteer.DistanceType,
            };

            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Create(doVolunteer); // Attempt to add the volunteer
            VolunteerManager.Observers.NotifyListUpdated(); // Notify observers that the volunteer list has been updated.
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id} already exists.");
        }
        catch (DO.DalException)
        {
            throw new BO.BlException("An error occurred while updating the volunteer.");
        }

        Console.WriteLine("Volunteer added successfully.");
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="BO.BlInvalidOperationException"></exception>
    /// <exception cref="BO.BloesNotExistException"></exception>
    /// <exception cref="BO.BlGeneralException"></exception>
    void IVolunteer.DeleteVolunteer(int id)
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
