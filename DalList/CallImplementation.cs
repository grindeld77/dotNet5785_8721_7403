using DalApi;
using DalList;
using DO;
namespace Dal;
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        Call tamp = Read(id);
        if (tamp != null)
            DataSource.Calls.Remove(tamp);
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
         return DataSource.Calls.FirstOrDefault(Value => Value.Id == id);
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
