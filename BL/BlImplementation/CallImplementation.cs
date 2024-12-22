namespace BlImplementation;
using BlApi;
using Helpers;
using System;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    void ICall.AddCall(BO.Call boCall)
    {
        // Validate the call object
        if (boCall == null)
            throw new BO.BloesNotExistException(nameof(boCall));

        if (string.IsNullOrWhiteSpace(boCall.FullAddress))
            throw new BO.BlInvalidAddressException("Address is required.");

        if (boCall.OpenTime >= boCall.MaxEndTime)
            throw new BO.BlInvalidTimeException("Open time must be earlier than the maximum finish time.");

        if (!Tools.IsValidAddress(boCall.FullAddress, out double latitude, out double longitude))
            throw new BO.BlInvalidAddressException("Address is not valid.");

        // Assign latitude and longitude to the call object
        boCall.Latitude = latitude;
        boCall.Longitude = longitude;

        // Convert BO.Call to DO.Call
        var doCall = new DO.Call
        {
            Id = boCall.Id,
            Address = boCall.FullAddress,
            Latitude = (double)boCall.Latitude,
            Longitude = (double)boCall.Longitude,
            OpenedAt = boCall.OpenTime,
            MaxCompletionTime = boCall.MaxEndTime,
            Status = (DO.CallStatus)boCall.Status,
        };

        // Attempt to add the call to the data layer
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("A call with the same ID already exists.", ex);
        }
    }

    void ICall.AssignVolunteerToCall(int volunteerId, int callId)
    {
        // Validate inputs
        if (volunteerId < 200000000 || volunteerId > 400000000)
            throw new BO.BlInvalidIdentityNumberException("Invalid volunteer ID.", nameof(volunteerId));
        if (callId < 0)
            throw new BO.BlInvalidCallIdException("Invalid call ID.", nameof(callId));

        // Retrieve the call and volunteer from the data layer
        DO.Call doCall = (DO.Call)_dal.Call.Read(callId);
        if (doCall == null)
            throw new BO.BloesNotExistException($"Call with ID {callId} does not exist.");

        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {volunteerId} does not exist.");

        // Check if the call is valid for assignment
        if (doCall.Status != DO.CallStatus.Open)
            throw new BO.BlInvalidOperationException("The call is not open for assignment or already assigned.");

        if (doCall.MaxCompletionTime <= ClockManager.Now)
            throw new BO.BlInvalidOperationException("The call has expired.");

        // Create a new assignment entity
        var newAssignment = new DO.Assignment
        {
            VolunteerId = volunteerId,
            CallId = callId,
            EntryTime = ClockManager.Now,
            CompletionTime = null,
            CompletionStatus = null
        };

        // Add the new assignment to the data layer
        try
        {
            _dal.Assignment.Create(newAssignment);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to assign volunteer to the call.", ex);
        }
    }

    void ICall.CancelCallAssignment(int requesterId, int assignmentId)
    {
        // Validate input parameters
        if (requesterId < 200000000 || requesterId > 400000000)
            throw new BO.BlInvalidAssignmentIdException("Invalid requester ID.", nameof(requesterId));
        if (assignmentId < 0)
            throw new BO.BlInvalidAssignmentIdException("Invalid assignment ID.", nameof(assignmentId));

        // Retrieve the assignment from the data layer
        var assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
            throw new BO.BloesNotExistException($"Assignment with ID {assignmentId} does not exist.");

        // Retrieve the volunteer associated with the assignment
        var volunteer = _dal.Volunteer.Read(assignment.VolunteerId);
        if (volunteer == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {assignment.VolunteerId} does not exist.");

        // Check authorization: requester must be the volunteer or an admin
        var requester = _dal.Volunteer.Read(requesterId);
        if (requester == null)
            throw new BO.BloesNotExistException($"Requester with ID {requesterId} does not exist.");

        if (requester.Role != DO.Role.Admin && requesterId != volunteer.Id)
            throw new BO.BlInvalidRequestException("Requester is not authorized to cancel this assignment.");

        // Check that the assignment is still open (not completed or expired)
        if (assignment.CompletionTime != null)
            throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");

        if (ClockManager.Now > assignment.CompletionTime)
            throw new BO.BlInvalidOperationException("The assignment has expired.");

        var updatedAssignment = assignment with
        {
            CompletionTime = ClockManager.Now,
            CompletionStatus = requesterId == volunteer.Id
                ? DO.CompletionStatus.SelfCancel
                : DO.CompletionStatus.AdminCancel
        };

        try
        {
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to cancel the assignment.", ex);
        }
    }

    void ICall.CompleteCallAssignment(int volunteerId, int assignmentId)
    {
        try
        {
            // Validate input parameters
            if (volunteerId < 200000000 || volunteerId > 400000000)
                throw new BO.BlInvalidIdentityNumberException("Invalid volunteer ID.", nameof(volunteerId));
            if (assignmentId < 0)
                throw new BO.BlInvalidAssignmentIdException("Invalid assignment ID.", nameof(assignmentId));

            // Retrieve the assignment from the data layer
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BO.BloesNotExistException($"Assignment with ID {assignmentId} does not exist.");

            // Retrieve the volunteer associated with the assignment
            var volunteer = _dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.BloesNotExistException($"Volunteer with ID {volunteerId} does not exist.");

            // Check authorization: volunteer must be the one who took the assignment
            if (volunteer.Id != assignment.VolunteerId)
                throw new BO.BlInvalidRequestException("Volunteer is not authorized to complete this assignment.");

            // Check that the assignment is still open (not completed or expired)
            if (assignment.CompletionTime != null)
                throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");

            if (ClockManager.Now > assignment.CompletionTime)
                throw new BO.BlInvalidOperationException("The assignment has expired.");

            var updatedAssignment = assignment with
            {
                CompletionTime = ClockManager.Now,
                CompletionStatus = DO.CompletionStatus.Handled
            };
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to complete the assignment.", ex);
        }
    }
    void ICall.DeleteCall(int callId)
    {
        // Validate input parameter
        if (callId < 0)
            throw new ArgumentException("Invalid call ID.", nameof(callId));

        // Retrieve the call from the data layer
        DO.Call doCall = (DO.Call)_dal.Call.Read(callId);
        if (doCall == null)
            throw new InvalidOperationException($"Call with ID {callId} does not exist.");

        // Check that the call is open and unassigned
        if (doCall.Status != DO.CallStatus.Open)
            throw new InvalidOperationException("The call is not open for deletion.");

        // Check if the call was ever assigned to a volunteer
        if (_dal.Assignment.Read(callId).VolunteerId != null)
        {
            throw new BO.BlInvalidRequestException("Calls that were assigned to volunteers cannot be deleted.");
        }

        // Attempt to delete the call from the data layer
        try
        {
            _dal.Call.Delete(callId);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to delete the call.", ex);

        }
    }
    int[] ICall.GetCallCountsByStatus()
    {
        try
        {
            // Fetch all calls from the data layer
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();

            // Use LINQ to group by status and count occurrences
            var statusCounts = calls
                .GroupBy(call => (int)call.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionary(item => item.Status, item => item.Count);

            // Determine the maximum status index for array sizing
            int maxStatusIndex = Enum.GetValues(typeof(DO.CallStatus)).Cast<int>().Max();

            // Initialize the result array with zero counts
            int[] result = new int[maxStatusIndex + 1];

            // Fill the array based on the status counts
            foreach (var item in statusCounts)
            {
                result[item.Key] = item.Key; // Assign the count to the corresponding status index 
            }
            return result;
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve call counts by status.", ex);
        }
    }

    BO.Call ICall.GetCallDetails(int callId)
    {
        try
        {
            // Retrieve the call from the data layer
            DO.Call doCall = (DO.Call)_dal.Call.Read(callId);
            if (doCall == null)
                throw new BO.BloesNotExistException($"Call with ID {callId} does not exist.");

            // Convert the DO.Call object to a BO.Call object
            return new BO.Call
            {
                Id = doCall.Id,
                FullAddress = doCall.Address,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpenTime = doCall.OpenedAt,
                MaxEndTime = (DateTime)doCall.MaxCompletionTime,
                Status = (BO.CallStatus)doCall.Status
            };
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve call details.", ex);
        }
    }

    IEnumerable<BO.CallInList> ICall.GetCalls(BO.CallStatus? filterField, object? filterValue, BO.CallStatus? sortField)
    {
        try
        {
            // Fetch all calls from the data layer
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();

            // Filter the calls based on the specified criteria
            if (filterField != null)
            {
                calls = calls.Where(call =>
                {
                    switch (filterField)
                    {
                        case BO.CallStatus status:
                            return call.Status == (DO.CallStatus)status;
                        // Other filters can be added here
                        default:
                            return false;
                    }
                });
            }

            // Sort the calls based on the specified criteria
            if (sortField != null)
            {
                calls = calls.OrderBy(call =>
                {
                    switch (sortField)
                    {
                        case BO.CallStatus status:
                            return (int)call.Status;
                        // You can add other sort fields as needed
                        default:
                            return call.Id; // Default sorting by call Id
                    }
                });
            }

            // Convert the DO.Call objects to BO.CallInList objects
            return calls.Select(call => new BO.CallInList
            {
                Id = call.Id,
                OpenTime = call.OpenedAt,
                Status = (BO.CallStatus)call.Status
            });
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve calls.", ex);
        }
    }

    IEnumerable<BO.ClosedCallInList> ICall.GetClosedCallsByVolunteer(int volunteerId, BO.CallStatus? filterField, BO.CallStatus? sortField)
    {
        try
        {
            // Fetch all assignments for the given volunteer
            var assignments = _dal.Assignment.ReadAll()
                .Where(assignment => assignment.VolunteerId == volunteerId)
                .Select(assignment => assignment.CallId); // Get all the CallIds for assignments that are completed

            // Fetch all closed calls that are assigned to this volunteer
            IEnumerable<DO.Call> doCalls = _dal.Call.ReadAll()
                .Where(call => assignments.Contains(call.Id) && call.Status == DO.CallStatus.Closed);  // Filter for closed calls assigned to the volunteer

            // Filter the calls based on the specified call type (if provided)
            if (filterField != null)
            {
                doCalls = doCalls.Where(call =>
                {
                    switch (filterField)
                    {
                        case BO.CallStatus status:
                            return call.Status == (DO.CallStatus)status;  // Filter based on the status of the call
                        default:
                            return false;
                    }
                });
            }

            // Sort the calls based on the specified field (if provided)
            if (sortField != null)
            {
                doCalls = doCalls.OrderBy(call =>
                {
                    switch (sortField)
                    {
                        case BO.CallStatus status:
                            return (int)call.Status;  // Sort by status if required
                        default:
                            return call.Id;  // Default sorting by call Id
                    }
                });
            }

            // Convert DO.Call objects to BO.ClosedCallInList objects
            return doCalls.Select(call => new BO.ClosedCallInList
            {
                Id = call.Id,
                OpenTime = call.OpenedAt,
                ClosedTime = call.MaxCompletionTime,
                Status = BO.CompletionStatus.Handled,
            });
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve closed calls for the volunteer.", ex);
        }
    }

    IEnumerable<BO.OpenCallInList> ICall.GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.CallStatus? sortField)
    {
        try
        {
            // Fetch all assignments for the given volunteer
            var assignments = _dal.Assignment.ReadAll()
                .Where(assignment => assignment.VolunteerId == volunteerId)
                .Select(assignment => assignment.CallId); // Get all the CallIds for assignments that are completed

            // Fetch all open calls that are not assigned to this volunteer
            IEnumerable<DO.Call> doCalls = _dal.Call.ReadAll()
                .Where(call => !assignments.Contains(call.Id) && call.Status == DO.CallStatus.Open);  // Filter for open calls not assigned to the volunteer

            // Filter the calls based on the specified call type (if provided)
            if (filterType != null)
            {
                doCalls = doCalls.Where(call =>
                {
                    switch (filterType)
                    {
                        case BO.CallType type:
                            return call.Type == (DO.CallType)type;  // Filter based on the type of the call
                        default:
                            return false;
                    }
                });
            }

            // Sort the calls based on the specified field (if provided)
            if (sortField != null)
            {
                doCalls = doCalls.OrderBy(call =>
                {
                    switch (sortField)
                    {
                        case BO.CallStatus status:
                            return (int)call.Status;  // Sort by status if required
                        default:
                            return call.Id;  // Default sorting by call Id
                    }
                });
            }

            // Convert DO.Call objects to BO.OpenCallInList objects
            return doCalls.Select(call => new BO.OpenCallInList
            {
                Id = call.Id,
                OpenTime = call.OpenedAt,
                DistanceFromVolunteer = (double)Tools.CalculateDistance(call.Latitude, call.Longitude, volunteerId),
            });
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve open calls for the volunteer.", ex);
        }
    }

    void ICall.UpdateCall(BO.Call call)
    {
        // Validate the call object
        if (call == null)
            throw new ArgumentNullException(nameof(call));

        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new ArgumentException("Address is required.");

        if (call.OpenTime >= call.MaxEndTime)
            throw new ArgumentException("Open time must be earlier than the maximum finish time.");

        if (!Tools.IsValidAddress(call.FullAddress, out double latitude, out double longitude))
            throw new ArgumentException("Address is not valid.");

        // Assign latitude and longitude to the call object
        call.Latitude = latitude;
        call.Longitude = longitude;

        // Convert BO.Call to DO.Call
        var doCall = new DO.Call
        {
            Id = call.Id,
            Address = call.FullAddress,
            Latitude = (double)call.Latitude,
            Longitude = (double)call.Longitude,
            OpenedAt = call.OpenTime,
            MaxCompletionTime = call.MaxEndTime,
            Status = (DO.CallStatus)call.Status,
        };

        // Attempt to update the call in the data layer
        try
        {
            _dal.Call.Update(doCall);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to update the call.", ex);
        }
    }
}
