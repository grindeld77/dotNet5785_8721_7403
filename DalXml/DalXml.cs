namespace Dal;
using DalApi;
using System.Diagnostics;

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
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
