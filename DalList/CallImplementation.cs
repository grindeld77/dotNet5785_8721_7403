using DalApi;
using DalList;
using DO;
using System.Runtime.CompilerServices;
namespace Dal;
internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        if (Read(id) != null)
            DataSource.Calls.Remove(DataSource.Calls.Find(Value => Value.Id == id));
        else
            throw new DalDoesNotExistException
                ($"An object of type Call with such ID={id} does not exist");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(Value => Value.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
          => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call item)
    {
        if (Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Calls.Add(item);
        }
        else
            throw new DalDoesNotExistException
                ($"An object of type Call with such ID={item.Id}  does not existsst");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
