namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public IVolunteer Volunteer => new VolunteerImplementation();

    public ICall Call => new CallImplementation();

    public IAdmin Admin => new AdminImplementation();


}
