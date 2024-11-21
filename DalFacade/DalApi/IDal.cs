namespace DalApi;

public interface IDal
{
    IVolunteer Volunteer { get; }
    IConfig Config { get; }
    IAssignment Assignment { get; }
    ICall Call { get; }
    void ResetDB();
        
}
