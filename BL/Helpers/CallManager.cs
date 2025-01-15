using BO;
using DalApi;
using DO;
using System.Collections.Generic;

namespace Helpers;
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 

    internal static BO.Call ConvertDoCallToBoCall(DO.Call? call)
    {
        if (call == null)
        {
            throw new BO.BloesNotExistException("Call does not exist.");
        }
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id); //to fix!
        IEnumerable<BO.CallAssignInList> assignmentsInList = CallManager.AssignmentToCallAssignInList(assignments);
        return new BO.Call
        {
            Id = call.Id,
            Type = (BO.CallType)call.Type,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenedAt,
            MaxEndTime = (DateTime)call.MaxCompletionTime,
            Status = (BO.CallStatus)call.Status,
            Assignments = assignmentsInList
        };
    }

    private static IEnumerable<CallAssignInList> AssignmentToCallAssignInList(IEnumerable<Assignment> assignments)
    {
        IEnumerable<CallAssignInList> list;

    }

    internal static IEnumerable<ClosedCallInList> GetAllClosedCalls(int volunteerId)
    {
        IEnumerable<ClosedCallInList> list = (from call in s_dal.Call.ReadAll()
                                              join assignment in s_dal.Assignment.ReadAll(x => x.VolunteerId == volunteerId) on call.Id equals assignment.CallId
                                              where assignment.CompletionStatus == DO.CompletionStatus.Handled
                                              select new BO.ClosedCallInList
                                              {
                                                  Id = call.Id,
                                                  Type = (BO.CallType)call.Type,
                                                  FullAddress = call.Address,
                                                  OpenTime = call.OpenedAt,
                                                  AssignedTime = assignment.EntryTime,
                                                  ClosedTime = assignment.CompletionTime,
                                                  Status = (BO.CompletionStatus?)assignment.CompletionStatus
                                              }).ToList();
        // Notify the observers about the updated list of closed calls.
        Observers.NotifyListUpdated(); // Notify that the list of closed calls was updated.

        return list;
    }

    internal static IEnumerable<OpenCallInList> GetOpenCallInList(int volunteerId)
    {
        IEnumerable<DO.Call> list = s_dal.Call.ReadAll();

        return from call in list
               let status = GetStatus(call.Id)
               where status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk
               select new BO.OpenCallInList()
               {
                   Id = call.Id,
                   Type = (BO.CallType)call.Type,
                   FullAddress = call.Address,
                   OpenTime = call.OpenedAt,
                   MaxEndTime = call.MaxCompletionTime,
                   Description = call.Description,
               };
    }


    private static BO.CallStatus GetStatus(int id)
    {
        var call = s_dal.Call.Read(id) ?? throw new BloesNotExistException("Call does not exist.");
        return (BO.CallStatus)call.Status;
        //var assignments = s_dal.Assignment.ReadAll(a => a.CallId == id).ToList();
        //if (assignments.Count == 0)
        //{
        //    return BO.CallStatus.Open;
        //}
        //var lastAssignment = assignments.OrderByDescending(a => a.EntryTime).First();
        //if (lastAssignment.CompletionStatus == DO.CompletionStatus.Handled)
        //{
        //    return BO.CallStatus.Closed;
        //}
        //if (lastAssignment.CompletionStatus == DO.CompletionStatus.AdminCancel)
        //{
        //    return BO.CallStatus.Closed;
        //}
        //if (lastAssignment.CompletionStatus == DO.CompletionStatus.SelfCancel)
        //{
        //    return BO.CallStatus.Closed;
        //}
        //if (lastAssignment.CompletionStatus == DO.CompletionStatus.Expired)
        //{
        //    return BO.CallStatus.Expired;
        //}
        //return DateTime.Now > lastAssignment.EntryTime.AddMinutes(30) ? BO.CallStatus.OpenAtRisk : BO.CallStatus.Open;
    }

    internal static CallInList converterFromDoToBoCallInList(DO.Call c)
    {
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == c.Id);
        BO.CallInList callInList = new BO.CallInList
        {
            AssignmentId = assignments.Any() ? assignments.Last().Id : 0,
            CallId = c.Id,
            CallType = (BO.CallType)c.Type,
            OpenTime = c.OpenedAt,
            RemainingTime = c.MaxCompletionTime - DateTime.Now > TimeSpan.Zero ? c.MaxCompletionTime - DateTime.Now : null,
            LastVolunteer = assignments.Any() ? assignments.Last().VolunteerId.ToString() : "None",
            TotalHandlingTime = c.MaxCompletionTime - c.OpenedAt,
            Status = (BO.CallStatus)c.Status,
            TotalAssignments = assignments.Count()
        };

        Observers.NotifyItemUpdated(c.Id); // stage 5
        return callInList;
    }
}

