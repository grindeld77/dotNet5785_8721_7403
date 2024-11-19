namespace Dal;
using DalApi;
using DalList;
using DO;
public class CallImplementation : ICall
{
    public int Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
        return id;
    }

    public void Delete(int id)
    {
        if(Read(id) != null)
            DataSource.Calls.Remove(DataSource.Calls.Find(Value => Value.Id == id));
        else
            throw new Exception
                ($"An object of type Call with such ID={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        if (DataSource.Calls.Exists(Value => Value.Id == id))
            return DataSource.Calls.Find(Value => Value.Id == id);
        else
            return null;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        if(Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Calls.Add(item);
        }
        else
            throw new Exception
                ($"An object of type Call with such ID={item.Id}  does not existsst");
    }
}
