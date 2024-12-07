using DalApi;
namespace Dal;
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();  // Delete all volunteers
        Assignment.DeleteAll();  // Delete all assignments
        Call.DeleteAll();   // Delete all calls
        Config.Reset();  // Reset the configuration    
    }
}
