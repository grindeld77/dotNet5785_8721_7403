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

        var assignments = s_dal.Assignment.ReadAll()?.Where(a => a.CallId == call.Id) ?? Enumerable.Empty<DO.Assignment>();

        var assignmentsInList = AssignmentToCallAssignInList(assignments);
        if (call.Status == DO.CallStatus.InProgressAtRisk || call.Status == DO.CallStatus.InProgress)
        {
            return new BO.Call
            {
                Id = call.Id,
                Type = (BO.CallType)call.Type,
                Description = call.Description,
                FullAddress = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenedAt,
                MaxEndTime = call.MaxCompletionTime ?? default(DateTime),
                Status = (BO.CallStatus)call.Status,
                Assignments = null
            };
        }
        return new BO.Call
        {
            Id = call.Id,
            Type = (BO.CallType)call.Type,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenedAt,
            MaxEndTime = call.MaxCompletionTime ?? default(DateTime),
            Status = (BO.CallStatus)call.Status,
            Assignments = assignmentsInList
        };
    }

    private static IEnumerable<CallAssignInList> AssignmentToCallAssignInList(IEnumerable<DO.Assignment>? assignments)
    {
        if (assignments == null || !assignments.Any())
        {
            return Enumerable.Empty<CallAssignInList>();
        }

        return assignments.Select(a => new CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            EndTime = a.CompletionTime ?? default(DateTime),
            StartTime = a.EntryTime,
            VolunteerName = s_dal.Volunteer.Read(a.VolunteerId)?.FullName ?? "Unknown",
            Status = (BO.CompletionStatus)a.CompletionStatus
        });
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

