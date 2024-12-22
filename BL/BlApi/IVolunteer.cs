using BO;
using System.Reflection.Metadata;

namespace BlApi;
public interface IVolunteer
{
    // login to system method that return the volunteer's role 
    string Login(string username, string password);

    // get collection of active volunteers in the system, if isActive is null return all volunteers,
    // if callType is null return sorted by id
    IEnumerable<VolunteerInList> GetVolunteers(bool? isActive, VolunteerFieldVolunteerInList? VolunteerParameter);

    // get volunteer details by id, return the BO object, else if not found throw exception
    Volunteer GetVolunteerDetails(int id);

    // update volunteer details, if not found throw exception
    void UpdateVolunteer(int requesterId, BO.Volunteer updatedVolunteer);

    // delete volunteer from the system, if not found throw exception 
    void DeleteVolunteer(int id);

    // add new volunteer to the system, if already exist throw exception
    void AddVolunteer(Volunteer volunteer);
}
