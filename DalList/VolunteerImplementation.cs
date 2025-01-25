
using DalApi;
using DO;
using DalList;
using System.Runtime.CompilerServices;
namespace Dal;


internal class VolunteerImplementation : IVolunteer 
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Volunteer item)
    {
        if(Read(item.Id) != null)
            throw new DalAlreadyExistsException
                ($"An object of type Volunteer with such ID={item.Id} already exists");
        else
            DataSource.Volunteers.Add(item);

    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        if(Read(id) != null)
            DataSource.Volunteers.Remove(DataSource.Volunteers.Find(Value => Value.Id == id));
        else
            throw new DalDoesNotExistException
                ($"An object of type Volunteer with such ID={id} does not exist");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(Value => Value.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
          => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
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

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}



