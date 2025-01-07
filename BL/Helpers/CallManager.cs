using BO;
using DalApi;
using DO;

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
            Status = (BO.CallStatus)call.Status
        };
    }
    public static List<BO.CallInList> GetAllCalls()
    {
        var calls = s_dal.Call.ReadAll();
        var assignments = s_dal.Assignment.ReadAll();

        var callInList = calls.Select(call =>
        {
            var callAssignments = assignments.Where(a => a.CallId == call.Id).ToList();
            var lastAssignment = callAssignments == null ? null : callAssignments.OrderByDescending(a => a.EntryTime).FirstOrDefault();

            return new BO.CallInList
            {
                AssignmentId = s_dal.Assignment.ReadAll() == null
                ? null
                : s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == call.Id)?.Id,
                CallId = call.Id,
                CallType = call.Type.ToString(),
                OpenTime = call.OpenedAt,
                RemainingTime = call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.Value - DateTime.Now : (TimeSpan?)null,
                LastVolunteer = lastAssignment != null ? s_dal.Volunteer.Read(lastAssignment.VolunteerId)?.FullName : null,
                TotalHandlingTime = lastAssignment?.CompletionTime.HasValue == true ? lastAssignment.CompletionTime.Value - lastAssignment.EntryTime : (TimeSpan?)null,
                Status = (BO.CallStatus)call.Status,
                TotalAssignments = assignments == null ? 0 : callAssignments!.Count
            };
        }).ToList();

        // Notify the observers about the updated call list.
        Observers.NotifyListUpdated(); // Notify that the list of calls was updated.

        return callInList;
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

    internal static List<OpenCallInList> GetOpenCallInList(int volunteerId)
    {
        List<DO.Call> list = s_dal.Call.ReadAll().ToList();
        DO.Volunteer volunteer = s_dal.Volunteer.Read(volunteerId) ?? throw new BloesNotExistException("Volunteer does not exist.");
        return (from assignment in list
                let call = s_dal.Call.Read(c => c.Id == assignment.Id) ?? throw new BloesNotExistException("Call does not exist.")
                let status = CallManager.GetStatus(call.Id)
                where status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk
                select new BO.OpenCallInList()
                {
                    Id = call.Id,
                    Type = (BO.CallType)call.Type,
                    FullAddress = call.Address,
                    OpenTime = call.OpenedAt,
                    MaxEndTime = call.MaxCompletionTime,
                    Description = call.Description,
                    DistanceFromVolunteer = VolunteerManager.CalculateDistance(call.Latitude, call.Longitude, (double)volunteer.Latitude, (double)volunteer.Longitude),
                }).ToList();
    }

    private static BO.CallStatus GetStatus(int id)
    {
        var call = s_dal.Call.Read(id) ?? throw new BloesNotExistException("Call does not exist.");
        var assignments = s_dal.Assignment.ReadAll(a => a.CallId == id).ToList();
        if (assignments.Count == 0)
        {
            return BO.CallStatus.Open;
        }
        var lastAssignment = assignments.OrderByDescending(a => a.EntryTime).First();
        if (lastAssignment.CompletionStatus == DO.CompletionStatus.Handled)
        {
            return BO.CallStatus.Closed;
        }
        if (lastAssignment.CompletionStatus == DO.CompletionStatus.AdminCancel)
        {
            return BO.CallStatus.Closed;
        }
        if (lastAssignment.CompletionStatus == DO.CompletionStatus.SelfCancel)
        {
            return BO.CallStatus.Closed;
        }
        if (lastAssignment.CompletionStatus == DO.CompletionStatus.Expired)
        {
            return BO.CallStatus.Expired;
        }
        return DateTime.Now > lastAssignment.EntryTime.AddMinutes(30) ? BO.CallStatus.OpenAtRisk : BO.CallStatus.Open;
    }
}

