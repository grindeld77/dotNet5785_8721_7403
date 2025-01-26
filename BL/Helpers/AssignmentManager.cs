using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Reflection.Metadata;

namespace Helpers;

internal static class AssignmentManager
{ 
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    private static readonly Random s_rand = new();

    internal static void AssignVolunteerToCall(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Call? calltamp;
        lock (AdminManager.BlMutex) //stage 7
            calltamp = _dal.Call.Read(callId); // Check if the call exists
        IEnumerable<Assignment> assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        if (calltamp == null)
            throw new BO.BloesNotExistException($"Call with ID {callId} does not exist.");
        if (calltamp.Status != DO.CallStatus.Open && calltamp.Status != DO.CallStatus.OpenAtRisk)
            throw new BO.BlInvalidOperationException("Call is already assigned");
        if (_dal.Volunteer.Read(volunteerId) == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {volunteerId} does not exist.");
        if (CallManager.IsVolunteerBusy(volunteerId))
            throw new BO.BlInvalidOperationException("Volunteer is already assigned to a call.");
        if (callId < 0)
            throw new BO.BlInvalidCallIdException("Invalid call ID.", nameof(callId));
        foreach (var item in assignment)
        {
            if (item.CompletionStatus == DO.CompletionStatus.Handled ||
               item.CompletionStatus == null)
                throw new BO.BlInvalidOperationException("Call is already assigned");
        }

        var newAssignment = new DO.Assignment // Create a new assignment
        {
            Id = -1,
            CallId = callId,
            VolunteerId = volunteerId,
            EntryTime = DateTime.Now,
            CompletionTime = null,
            CompletionStatus = null
        };
        lock (AdminManager.BlMutex) //stage 7
            _dal.Assignment.Create(newAssignment); // Assign the call to the volunteer
        AssignmentManager.Observers.NotifyListUpdated(); //stage 5

        BO.CallStatus s = BO.CallStatus.InProgress;// Update the call status
        if (calltamp.Status == DO.CallStatus.Open)
            s = BO.CallStatus.InProgress;
        else if (calltamp.Status == DO.CallStatus.OpenAtRisk)
            s = BO.CallStatus.InProgressAtRisk;
        var newCall = new BO.Call
        {
            Id = callId,
            Type = (BO.CallType)calltamp.Type,
            Description = calltamp?.Description ?? null,
            FullAddress = calltamp?.Address ?? null,
            Latitude = calltamp?.Latitude ?? 0.0,
            Longitude = calltamp?.Longitude ?? 0.0,
            OpenTime = calltamp.OpenedAt,
            MaxEndTime = (DateTime)calltamp.MaxCompletionTime,
            Status = s,
            Assignments = null
        };
        CallManager.UpdateCall(newCall);

        CallManager.Observers.NotifyItemUpdated(callId); //stage 5
        return;
    }

    internal static void UpdateCallForVolunteer(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {

            // Retrieve the assignment from the data layer
            DO.Assignment assignment;
            lock (AdminManager.BlMutex) //stage 7
                assignment = _dal.Assignment.Read(a => a.Id == assignmentId && a.VolunteerId == volunteerId) ?? throw new BO.BloesNotExistException($"Assignment with ID {assignmentId} does not exist.");

            // Retrieve the volunteer associated with the assignment
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BloesNotExistException($"Volunteer with ID {volunteerId} does not exist.");

            // Check authorization: volunteer must be the one who took the assignment
            if (volunteer.Id != assignment.VolunteerId)
                throw new BO.BlInvalidRequestException("Volunteer is not authorized to complete this assignment.");

            // Check that the assignment is still open (not completed or expired)
            if (assignment.CompletionTime != null)
                throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");

            if (AdminManager.Now > assignment.CompletionTime)
                throw new BO.BlInvalidOperationException("The assignment has expired.");

            var updateAssignment = new Assignment // Update the assignment in the data layer
            {
                Id = assignmentId,
                CallId = assignment.CallId,
                VolunteerId = assignment.VolunteerId,
                EntryTime = assignment.EntryTime,
                CompletionTime = AdminManager.Now,
                CompletionStatus = DO.CompletionStatus.Handled
            };

            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(updateAssignment);  // Mark the assignment as completed
            AssignmentManager.Observers.NotifyItemUpdated(updateAssignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5

            DO.Call calltamp;
            lock (AdminManager.BlMutex) //stage 7
                calltamp = _dal.Call.Read(assignment.CallId); // Check if the call exists
            var newCall = new BO.Call
            {
                Id = assignment.CallId,
                Type = (BO.CallType)calltamp.Type,
                Description = calltamp?.Description ?? null,
                FullAddress = calltamp?.Address ?? null,
                Latitude = calltamp?.Latitude ?? 0.0,
                Longitude = calltamp?.Longitude ?? 0.0,
                OpenTime = calltamp.OpenedAt,
                MaxEndTime = (DateTime)calltamp.MaxCompletionTime,
                Status = BO.CallStatus.Closed,
                Assignments = null
            };
            CallManager.UpdateCall(newCall);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to complete the assignment.", ex);
        }
    }

    internal static void CancelAssignment(int requesterId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        // Retrieve the assignment from the data layer
        DO.Assignment assignment;
        lock (AdminManager.BlMutex) //stage 7
            assignment = _dal.Assignment.Read(a => a.Id == assignmentId);


        // Retrieve the volunteer associated with the assignment
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(a => a.Id == assignment.VolunteerId);
        if (volunteer == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {assignment.VolunteerId} does not exist.");

        // Check authorization: requester must be the volunteer or an admin
        DO.Volunteer requester;
        lock (AdminManager.BlMutex) //stage 7
            requester = _dal.Volunteer.Read(requesterId);
        if (requester == null)
            throw new BO.BloesNotExistException($"Requester with ID {requesterId} does not exist.");

        if (requester.Role != DO.Role.Admin && requesterId != volunteer.Id)
            throw new BO.BlInvalidRequestException("Requester is not authorized to cancel this assignment.");

        // Check that the assignment is still open (not completed or expired)
        if (assignment.CompletionTime != null || assignment.CompletionStatus != null)
            throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");

        if (AdminManager.Now > assignment.CompletionTime)
            throw new BO.BlInvalidOperationException("The assignment has expired.");

        DO.CompletionStatus completionStatus;
        if (requester.Role == DO.Role.Admin)
        {
            completionStatus = DO.CompletionStatus.AdminCancel;
        }
        else
        {
            completionStatus = DO.CompletionStatus.SelfCancel;
        }
        Assignment updateAssignment = new Assignment // Update the assignment in the data layer
        {
            Id = assignmentId,
            CallId = assignment.CallId,
            VolunteerId = assignment.VolunteerId,
            EntryTime = assignment.EntryTime,
            CompletionTime = DateTime.Now,
            CompletionStatus = completionStatus
        };

        DO.Call calltamp;
        lock (AdminManager.BlMutex) //stage 7
            calltamp = _dal.Call.Read(assignment.CallId); // Check if the call exists

        BO.CallStatus s = BO.CallStatus.InProgress;// Update the call status
        if (calltamp.Status == DO.CallStatus.InProgress)
            s = BO.CallStatus.Open;
        else if (calltamp.Status == DO.CallStatus.InProgressAtRisk)
            s = BO.CallStatus.OpenAtRisk;

        var newCall = new BO.Call
        {
            Id = assignment.CallId,
            Type = (BO.CallType)calltamp.Type,
            Description = calltamp?.Description ?? null,
            FullAddress = calltamp?.Address ?? null,
            Latitude = calltamp?.Latitude ?? 0.0,
            Longitude = calltamp?.Longitude ?? 0.0,
            OpenTime = calltamp.OpenedAt,
            MaxEndTime = (DateTime)calltamp.MaxCompletionTime,
            Status = s,
            Assignments = null
        };
        CallManager.UpdateCall(newCall);

        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Update(updateAssignment);
            AssignmentManager.Observers.NotifyItemUpdated(updateAssignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to cancel the assignment.", ex);
        }

    }
}
