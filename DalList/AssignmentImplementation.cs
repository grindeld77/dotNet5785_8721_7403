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
        Assignment tamp = Read(id);
        if (tamp != null)
            DataSource.Assignments.Remove(tamp);
        else
            throw new Exception
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

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        if(Read(item.Id) != null)
        {
            Delete(item.Id);
            DataSource.Assignments.Add(item);
        }
        else
            throw new Exception
                ($"An object of type Assignment with such ID={item.Id}  does not existsst");
    }
}
