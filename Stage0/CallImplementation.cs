namespace Dal;
using DalApi;
using DO;
using System;
//using System.Collections.Generic;
//using System.Linq;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        Call Item = item with { Id = Config.NextCallId }; // creates new item with the correct ID
        calls.Add(Item); // add new call to list 
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // save updated list of calls to XML
    }

    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        if (calls.RemoveAll(it => it.Id == id) == 0) // remove all calls with ID==id
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist"); // if no calls were removed, throw exception
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // save updated list of calls to XML
    }

    public void DeleteAll()
    {
         XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml); // save empty list of calls to XML 

        //List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        //calls.Clear(); // Remove all elements from the list
        //XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // save updated list (empty in this case)


    }

    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        Call? toReturn = calls.FirstOrDefault(Value => Value.Id == id);
        if (toReturn == null) // if call with ID==id does not exist
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist"); // throw exception
        return toReturn; // return call with ID==id
    }

    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        Call? toReturn = calls.FirstOrDefault(filter);
        if (toReturn == null) // if call does not exist
            throw new DalDoesNotExistException("Call that matches the filter does Not exist"); // throw exception
        return toReturn; // return call that matches the filter
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML
        if (filter == null) // if no filter was given 
            return calls.Select(item => item); // return all calls
        else
            return calls.Where(filter); // return all calls
    }

    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml); // load list of calls from XML    
        if (calls.RemoveAll(it => it.Id == item.Id) == 0) // remove all calls with ID==item.Id
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");     // if no calls were removed, throw exception
        calls.Add(item); // add updated calls to list 
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // save updated list of calls to XML
    }
}