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

    internal static IEnumerable<OpenCallInList> GetOpenCallInList(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);

        List<BO.OpenCallInList> list = s_dal.Call
            .ReadAll((DO.Call call) => call.Status.ToString() == "Open")
            .Where((DO.Call call) =>
                (volunteer.CurrentAddress == null) || Tools.DistanceCalculator.GetDistance(volunteer.CurrentAddress!, call.Address, volunteer.DistancePreference.ToString()) <= volunteer.MaxCallDistance)
            .Select((DO.Call call) => new BO.OpenCallInList()
            {
                Id = call.Id,
                Type = (BO.CallType)call.Type,
                Description = call.Description,
                FullAddress = call.Address,
                OpenTime = call.OpenedAt,
                MaxEndTime = call.MaxCompletionTime,
                DistanceFromVolunteer = Tools.DistanceCalculator.GetDistance(volunteer.CurrentAddress, call.Address, volunteer.DistancePreference.ToString())
            }).ToList();

        return list;
    }
}

