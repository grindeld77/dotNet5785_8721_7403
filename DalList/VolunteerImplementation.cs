using DalApi;
using DO;
using DalList;
namespace Dal;


internal class VolunteerImplementation : IVolunteer 
{
    public void Create(Volunteer item)
    {
        if(Read(item.Id) != null)
            throw new Exception
                ($"An object of type Volunteer with such ID={item.Id} already exists");
        else
            DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        Volunteer tamp = Read(id);
        if (tamp != null)
            DataSource.Volunteers.Remove(tamp);
        else
            throw new Exception
                ($"An object of type Volunteer with such ID={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
            return DataSource.Volunteers.FirstOrDefault(Value => Value.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        Volunteer tamp = Read(item.Id);
        if (tamp != null)
        {
            DataSource.Volunteers.Remove(tamp);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new Exception
                ($"An object of type Volunteer with such ID={item.Id}  does not existsst");
    }
}



