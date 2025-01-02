namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Security.AccessControl;


internal class VolunteerImplementation : IVolunteer
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    string IVolunteer.Login(int id, string password)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
        {
            throw new BO.BlInvalidIdentificationException("The email address is not found.");
        }

        if (volunteer.Password != password)
        {
            throw new BO.BlInvalidIdentificationException("The password is incorrect.");
        }

        return volunteer.Role.ToString();
    }



    IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, BO.VolunteerFieldVolunteerInList? VolunteerParameter, BO.CallType? type)
    {
        IEnumerable<DO.Volunteer> doVolunteers;
        IEnumerable<BO.VolunteerInList> volunteerInLists = null;

        if (isActive.HasValue)
        {
            doVolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive.Value);
        }
        else
        {
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
        //VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        return volunteerInLists ?? Enumerable.Empty<BO.VolunteerInList>();
    }


    BO.Volunteer IVolunteer.GetVolunteerDetails(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id);

            BO.Volunteer v = VolunteerManager.converterFromDoToBoVolunteer(doVolunteer);
            //VolunteerManager.Observers.NotifyItemUpdated(id); //stage 5
            return v;

        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while retrieving volunteer details.", ex);
        }
    }

    void IVolunteer.UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            var doVolunteere = _dal.Volunteer.Read(requesterId);//בדיקה אם המתנדב קיים
            // בדיקת זהות המבקש
            if (requesterId != volunteer.Id && doVolunteere.Role != DO.Role.Admin)
                throw new BO.BlInvalidRequestException("You are not authorized to update this volunteer.");
            VolunteerManager.ValidateVolunteerData(volunteer);//בדיקת תקינות נתונים
            DO.Role Pos = doVolunteere.Role;
            if ((doVolunteere.Role.ToString() != volunteer.Role.ToString()) && Pos != DO.Role.Admin)            //בדיקה אם המתנדב יכול לשנות תפקיד
            {
                throw new Exception("A volunteer could not change roles");
            }
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
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude == 0 ? doVolunteere.Latitude
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude == 0 ? doVolunteere.Longitude
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = doVolunteere.MaxCallDistance,
                DistancePreference = doVolunteere.DistancePreference,
            };
            _dal.Volunteer.Update(doVolunteerNew);
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteere.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
        }

        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BloesNotExistException($"Volunteer with ID {volunteer.Id} does not exist.");//בדיקה אם המתנדב קיים
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while updating the volunteer.", ex);
        }
    }

    public void AddVolunteer(BO.Volunteer volunteer)
    {

        VolunteerManager.ValidateVolunteerData(volunteer);         //בדיקת תקינות הנתונים
        try
        {

            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                Password = volunteer.PasswordHash,
                CurrentAddress = volunteer.FullAddress,
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude == 0 ? null
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude == 0 ? null
                : Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall,
                DistancePreference = (DO.DistanceType)volunteer.DistanceType,
            };

            _dal.Volunteer.Create(doVolunteer);   //נסיון בקשת הוספה
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id}is  already exist.");
        }
        catch (DO.DalException)
        {
            throw new BO.BlException("An error occurred while updating the volunteer.");
        }

        Console.WriteLine("Volunteer added successfully.");

    }
    void IVolunteer.DeleteVolunteer(int id)
    {
        DO.Assignment chak = _dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == id);
        if (chak != null)
        {
            throw new BO.BlInvalidOperationException("The volunteer is assigned to a call and cannot be deleted.");
        }
        try
        {
            _dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
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
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}