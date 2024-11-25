
using DalApi;
using DO;
using DalList;
namespace Dal;


internal class VolunteerImplementation : IVolunteer 
{
    public void Create(Volunteer item)
    {
        if(Read(item.Id) != null)
            throw new DalAlreadyExistsException
                ($"An object of type Volunteer with such ID={item.Id} already exists");
        else
            DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        if(Read(id) != null)
            DataSource.Volunteers.Remove(DataSource.Volunteers.Find(Value => Value.Id == id));
        else
            throw new DalDoesNotExistException
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

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
          => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);


    public void Update(Volunteer item)
    {
        if(Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new DalDoesNotExistException
                ($"An object of type Volunteer with such ID={item.Id}  does not existsst");
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}



