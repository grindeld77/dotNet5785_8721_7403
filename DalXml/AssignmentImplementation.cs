namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML
        int id = Config.NextAssignmentId; // get next assignment id
        Assignment copy = item with { Id = id }; // create a copy of the assignment with the new id
        assignments.Add(item); // add new assignment to list 
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // save updated list of assignments to XML
    }

    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML
        if (assignments.RemoveAll(it => it.Id == id) == 0) // remove all assignments with ID==id
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist"); // if no assignments were removed, throw exception
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // save updated list of assignments to XML
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml); // save empty list of assignments to XML 
    }

    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML
        Assignment? toReturn = assignments.FirstOrDefault(Value => Value.Id == id);
        if (toReturn == null) // if assignment with ID==id does not exist
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist"); // throw exception
        return toReturn; // return assignment with ID==id
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML
        Assignment? toReturn = assignments.FirstOrDefault(filter);
        if (toReturn == null) // if assignment does not exist
            throw new DalDoesNotExistException("Call that matches the filter does Not exist"); // throw exception
        return toReturn; // return assignment that matches the filter
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML
        if (filter == null) // if no filter was given 
            return assignments.Select(item => item); // return all assignments
        else
            return assignments.Where(filter); // return all assignments
    }

    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml); // load list of assignments from XML    
        if (assignments.RemoveAll(it => it.Id == item.Id) == 0) // remove all assignments with ID==item.Id
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");     // if no assignments were removed, throw exception
        assignments.Add(item); // add updated assignments to list 
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // save updated list of assignments to XML
    }
}
