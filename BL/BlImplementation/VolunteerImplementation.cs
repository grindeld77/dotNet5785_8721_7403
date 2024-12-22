namespace BlImplementation;
using BlApi;
using Helpers;
using System.Security.AccessControl;


 internal class VolunteerImplementation : IVolunteer
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    string IVolunteer.Login(string username, string password)
    {

        DO.Volunteer volunteer = _dal.Volunteer.ReadAll()
            .FirstOrDefault(v => v.FullName == username && v.Password == password)
            ?? throw new BO.BlInvalidIdentificationException("The username or ID entered is invalid.");
        return volunteer.Role.ToString();
    }




    BO.Volunteer IVolunteer.GetVolunteerDetails(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id);
            if (doVolunteer == null)
            {
                throw new BO.BloesNotExistException($"Volunteer with ID {id} does not exist.");
            }

            BO.Volunteer v = VolunteerManager.converterFromDoToBoVolunteer(doVolunteer);
            return v;

        }
        catch (DO.DalException ex)
        {
            throw new BO.BlGeneralException("An error occurred while retrieving volunteer details.", ex);
        }
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
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall,
                DistancePreference = (DO.DistanceType)volunteer.DistanceType,
            };

            _dal.Volunteer.Create(doVolunteer);   //נסיון בקשת הוספה
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id}is  already exist.");
        }
        catch (DO.DalException)
        {
            throw new BO.BlException("An error occurred while updating the volunteer.");
        }

    }



    void IVolunteer.UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            // בדיקת זהות המבקש
            if (!VolunteerManager.IsAuthorizedToUpdate(requesterId, volunteer.Id))
            {
                throw new BO.BlInvalidRequestException("You are not authorized to update this volunteer.");
            }
            var doVolunteere = _dal.Volunteer.Read(volunteer.Id);//בדיקה אם המתנדב קיים
            VolunteerManager.ValidateVolunteerData(volunteer);//בדיקת תקינות נתונים
            BO.Role Pos = volunteer.Role;
            if ((doVolunteere.Role.ToString() != volunteer.Role.ToString()) && Pos != BO.Role.Admin)            //בדיקה אם המתנדב יכול לשנות תפקיד
            {
                throw new Exception("A volunteer could not change roles");
            }
            DO.Volunteer doVolunteerNew = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                Password = volunteer.PasswordHash,
                CurrentAddress = volunteer.FullAddress,
                Latitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Latitude,
                Longitude = Tools.GeocodingHelper.GetCoordinates(volunteer.FullAddress).Longitude,
                MaxCallDistance = volunteer.MaxDistanceForCall,
                DistancePreference = (DO.DistanceType)volunteer.DistanceType,
            };
            _dal.Volunteer.Update(doVolunteerNew);
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

    IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, BO.VolunteerFieldVolunteerInList? VolunteerParameter)
    {
        IEnumerable<BO.VolunteerInList> volunteerInLists = Enumerable.Empty<BO.VolunteerInList>();
        IEnumerable<DO.Volunteer> doVolunteers;

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

        return volunteerInLists ?? Enumerable.Empty<BO.VolunteerInList>();
    }
}
