using DalApi;
using DalList;
using DO;
namespace Dal;
internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    public void Delete(int id)
    {
        if(Read(id) != null)
            DataSource.Assignments.Remove(DataSource.Assignments.Find(Value => Value.Id == id));
        else
            throw new DalDoesNotExistException
                ($"An object of type Assignment with such ID={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(Value => Value.Id == id);

    }
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
      => filter == null
        ? DataSource.Assignments.Select(item => item)
        : DataSource.Assignments.Where(filter);


    public void Update(Assignment item)
    {
        if(Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Assignments.Add(item);
        }
        else
            throw new DalDoesNotExistException
                ($"An object of type Assignment with such ID={item.Id}  does not existsst");
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
}
