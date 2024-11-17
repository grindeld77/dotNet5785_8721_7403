namespace Dal;
using DalApi;
using DO;

using DalList;///למה צריך את זה?


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
        if(Read(id) != null)
            DataSource.Volunteers.Remove(DataSource.Volunteers.Find(Value => Value.Id == id));
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
        if(DataSource.Volunteers.Exists(Value => Value.Id == id))
            return DataSource.Volunteers.Find(Value => Value.Id == id);
        else
            return null;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        if(Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new Exception
                ($"An object of type Volunteer with such ID={item.Id}  does not existsst");
    }
}



