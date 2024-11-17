namespace Dal;
using DalApi;
using DalList;///למה צריך את זה?
using DO;
internal class AssignmentImplementation : IAssignment
{
    public int Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
        return id;
    }

    public void Delete(int id)
    {
        if(Read(id) != null)
            DataSource.Assignments.Remove(DataSource.Assignments.Find(Value => Value.Id == id));
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
        if (DataSource.Assignments.Exists(Value => Value.Id == id))
            return DataSource.Assignments.Find(Value => Value.Id == id);
        else
            return null;
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
