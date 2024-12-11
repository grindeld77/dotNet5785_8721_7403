namespace BlImplementation;
using BlApi;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    string IVolunteer.Login(string username, string password)
    {
        try
        {
            return _dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.FullName == username && v.Password == password)
                .Role
                .ToString();
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlInvalidIdentificationException("The username or ID entered is invalid.");
        }
    }
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

    IEnumerable<BO.VolunteerInList> IVolunteer.GetVolunteers(bool? isActive, BO.CallType? callType)
    {
        try
        {
            IEnumerable<DO.Volunteer> doVolunteers;

            if (isActive.HasValue)
            {
                doVolunteers = _dal.Volunteer.GetAll(v => v.IsActive == isActive.Value);
            }
            else
            {
                doVolunteers = _dal.Volunteer.GetAll();
            }

            if (callType.HasValue)
            {
                doVolunteers = doVolunteers.Where(v => v.AvailableCallTypes.HasFlag(callType.Value));
            }

            return doVolunteers.OrderBy(v => v.Id).Select(v => new BO.VolunteerInList
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive
            });
        }
        catch (DO.DalException ex)
        {
            throw new BO.BlException("An error occurred while retrieving volunteers.", ex);
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