namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System;

internal class CallImplementation : ICall
{
    private const DO.CallStatus inProgress = DO.CallStatus.InProgress;
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    void ICall.AddCall(BO.Call boCall)
    {
        try
        {
            // Validate the call object
            if (boCall == null)
                throw new BO.BloesNotExistException(nameof(boCall));
            
            if (string.IsNullOrWhiteSpace(boCall.FullAddress))
                throw new BO.BlInvalidAddressException("Address is required.");

            if (boCall.OpenTime >= boCall.MaxEndTime)
                throw new BO.BlInvalidTimeException("Open time must be earlier than the maximum finish time.");

            (boCall.Latitude, boCall.Longitude) = Tools.GeocodingHelper.GetCoordinates(boCall.FullAddress);

            var call = new DO.Call
            {
                Id = boCall.Id,
                Status = (DO.CallStatus)boCall.Status,
                Type = (DO.CallType)boCall.Type,
                Address = boCall.FullAddress,
                Latitude = (double)boCall.Latitude,
                Longitude = (double)boCall.Longitude,
                OpenedAt = boCall.OpenTime,
                MaxCompletionTime = boCall.MaxEndTime,
                Description = boCall.Description
            };

            _dal.Call.Create(call);
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw;
        }
    }

    void ICall.AssignVolunteerToCall(int volunteerId, int callId)
    {
        ICall help = new CallImplementation();
        var calltamp = _dal.Call.Read(callId); // Check if the call exists
        IEnumerable<Assignment> assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        if (calltamp == null)
            throw new BO.BloesNotExistException($"Call with ID {callId} does not exist.");
        if (calltamp.Status != DO.CallStatus.Open&& calltamp.Status!= DO.CallStatus.OpenAtRisk)
            throw new BO.BlInvalidOperationException("Call is already assigned");
        if (_dal.Volunteer.Read(volunteerId) == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {volunteerId} does not exist.");
        if (CallManager.IsVolunteerBusy(volunteerId))
            throw new BO.BlInvalidOperationException("Volunteer is already assigned to a call.");
        if (callId < 0)
            throw new BO.BlInvalidCallIdException("Invalid call ID.", nameof(callId));
        foreach (var item in assignment)
        {
            if (item.CompletionStatus == DO.CompletionStatus.Handled||
               item.CompletionStatus ==null)
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

            _dal.Assignment.Create(newAssignment); // Assign the call to the volunteer
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5

            BO.CallStatus s = BO.CallStatus.InProgress;// Update the call status
            if (calltamp.Status == DO.CallStatus.Open)
                     s = BO.CallStatus.InProgress;
            else if(calltamp.Status == DO.CallStatus.OpenAtRisk)
                    s= BO.CallStatus.InProgressAtRisk;

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
                help.UpdateCall(newCall);
            
            CallManager.Observers.NotifyItemUpdated(callId); //stage 5
            return;
    }



    void ICall.CancelCallAssignment(int requesterId, int assignmentId)
    {
        // Retrieve the assignment from the data layer
        DO.Assignment assignment = _dal.Assignment.Read(a => a.Id == assignmentId);


        // Retrieve the volunteer associated with the assignment
        var volunteer = _dal.Volunteer.Read(a => a.Id == assignment.VolunteerId);
        if (volunteer == null)
            throw new BO.BloesNotExistException($"Volunteer with ID {assignment.VolunteerId} does not exist.");

        // Check authorization: requester must be the volunteer or an admin
        var requester = _dal.Volunteer.Read(requesterId);
        if (requester == null)
            throw new BO.BloesNotExistException($"Requester with ID {requesterId} does not exist.");

        if (requester.Role != DO.Role.Admin && requesterId != volunteer.Id)
            throw new BO.BlInvalidRequestException("Requester is not authorized to cancel this assignment.");

        // Check that the assignment is still open (not completed or expired)
        if (assignment.CompletionTime != null|| assignment.CompletionStatus!=null)
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

        ICall help = new CallImplementation();
        var calltamp = _dal.Call.Read(assignment.CallId); // Check if the call exists

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
            help.UpdateCall(newCall);

        try
        {
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

    void ICall.CompleteCallAssignment(int volunteerId, int assignmentId)
    {
        try
        {

            // Retrieve the assignment from the data layer
            DO.Assignment assignment = _dal.Assignment.Read(a => a.Id == assignmentId && a.VolunteerId == volunteerId);
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

            _dal.Assignment.Update(updateAssignment);  // Mark the assignment as completed
            AssignmentManager.Observers.NotifyItemUpdated(updateAssignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5

            ICall help = new CallImplementation();
            var calltamp = _dal.Call.Read(assignment.CallId); // Check if the call exists
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
            help.UpdateCall(newCall);
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
        var call = _dal.Call.Read(a => a.Id == callId);

        // Check if the call exists
        if (!_dal.Assignment.ReadAll(a => a.CallId == callId).Any())
        {
            throw new BO.BlInvalidOperationException("Assignment was not found");
        }

        // Check if the call is already assigned
        if (_dal.Assignment.ReadAll(a => a.CallId == callId).Any() && BO.CallStatus.Open != (BO.CallStatus)call.Status)
        {
            throw new BO.BlInvalidOperationException("Call is already assigned");
        }

        // Attempt to delete the call from the data layer
        try
        {
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5
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
            var calls = _dal.Call.ReadAll(); // Fetch all calls from the data layer
            var groupedCalls = calls.GroupBy(c => (int)c.Status) // Use LINQ to group by status and count occurrences
                                    .Select(g => new { Status = g.Key, Count = g.Count() });

            int maxStatus = Enum.GetValues(typeof(DO.CallStatus)).Cast<int>().Max(); // Determine the maximum status index for array sizing
            int[] result = new int[maxStatus + 1]; // Initialize the result array with zero counts


            foreach (var group in groupedCalls) // Fill the array based on the status counts
            {
                result[group.Status] = group.Count;
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
            DO.Call call = _dal.Call.Read(callId);
            return CallManager.ConvertDoCallToBoCall(call);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve call details.", ex);
        }
    }

    IEnumerable<BO.CallInList> ICall.GetCalls(BO.CallStatus? filterField, object? filterValue, BO.CallInListFields? sortField)
    {
        try
        {
            IEnumerable<DO.Call> doCalls;
            IEnumerable<BO.CallInList> callsList = null;

            doCalls = _dal.Call.ReadAll();
            

            if (doCalls != null)
            {
                callsList = doCalls.Select(c => CallManager.converterFromDoToBoCallInList(c));
            }

            if (filterField != null) // Filter the calls based on the specified field
            {
                switch (filterField) // Filter the calls based on the specified field
                {
                    case BO.CallStatus.Open:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.Open);
                        break;
                    case BO.CallStatus.OpenAtRisk:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.OpenAtRisk);
                        break;
                    case BO.CallStatus.Closed:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.Closed);
                        break;
                    case BO.CallStatus.InProgress:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.InProgress);
                        break;
                    case BO.CallStatus.Expired:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.Expired);
                        break;
                    case BO.CallStatus.InProgressAtRisk:
                        callsList = callsList.Where(call => call.Status == BO.CallStatus.InProgressAtRisk);
                        break;
                    case BO.CallStatus.ALL:
                        break;
                }
            }
            if (sortField != null)
            {
                switch (sortField) // Sort the calls based on the specified field
                {
                    case BO.CallInListFields.AssignmentId:
                            callsList = callsList.OrderBy(call => call.AssignmentId);
                        break;
                    case BO.CallInListFields.CallId:
                            callsList = callsList.OrderBy(call => call.CallId);
                        break;
                    case BO.CallInListFields.CallType:
                            callsList = callsList.OrderBy(call => call.CallType);
                        break;
                    case BO.CallInListFields.OpenTime:
                            callsList = callsList.OrderBy(call => call.OpenTime);
                        break;
                    case BO.CallInListFields.RemainingTime:
                            callsList = callsList.OrderBy(call => call.RemainingTime);
                        break;
                    case BO.CallInListFields.LastVolunteer:
                            callsList = callsList.OrderBy(call => call.LastVolunteer);
                        break;
                    case BO.CallInListFields.TotalHandlingTime:
                            callsList = callsList.OrderBy(call => call.TotalHandlingTime);
                        break;
                    case BO.CallInListFields.Status:
                            callsList = callsList.OrderBy(call => call.Status);
                        break;
                    case BO.CallInListFields.TotalAssignments:
                            callsList = callsList.OrderBy(call => call.TotalAssignments);
                        break;
                }
            }
            return callsList;
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve calls.", ex);
        }
    }

    IEnumerable<BO.ClosedCallInList> ICall.GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterField, BO.ClosedCallInListFields? sortField)
    {
        try
        {
            var calls = CallManager.GetAllClosedCalls(volunteerId);
            IEnumerable<BO.ClosedCallInList> closedCallsInList = calls;

            if (filterField != null)
            {
                closedCallsInList = closedCallsInList.Where(call => call.Type == filterField);
            }

            if (sortField != null)
            {
                switch (sortField)
                {
                    case BO.ClosedCallInListFields.Id:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.Id);
                        break;
                    case BO.ClosedCallInListFields.Type:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.Type);
                        break;
                    case BO.ClosedCallInListFields.FullAddress:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.FullAddress);
                        break;
                    case BO.ClosedCallInListFields.OpenTime:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.OpenTime);
                        break;
                    case BO.ClosedCallInListFields.AssignedTime:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.AssignedTime);
                        break;
                    case BO.ClosedCallInListFields.ClosedTime:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.ClosedTime);
                        break;
                    case BO.ClosedCallInListFields.Status:
                        closedCallsInList = closedCallsInList.OrderBy(call => call.Status);
                        break;
                }
            }
            return closedCallsInList;
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve closed calls for the volunteer.", ex);
        }
    }

    IEnumerable<BO.OpenCallInList> ICall.GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.OpenCallInListFields? sortField)
    {
            // Retrieve the volunteer from the DAL
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exists");

            var allCalls = _dal.Call.ReadAll();

            var allAssignments = _dal.Assignment.ReadAll();

            double lonVol = (double)volunteer.Longitude;
            double latVol = (double)volunteer.Latitude;

            var openCallsInList = from call in allCalls
                                let boCall = CallManager.ConvertDoCallToBoCall(call)
                                where (boCall.Status == BO.CallStatus.Open || boCall.Status == BO.CallStatus.OpenAtRisk) 
                                select new BO.OpenCallInList
                                {
                                    Id = call.Id,
                                    Type = (BO.CallType)call.Type,
                                    FullAddress = boCall.FullAddress,
                                    OpenTime  = call.OpenedAt,
                                    MaxEndTime = call.MaxCompletionTime,// ? AdminManager.Now.Add(call.MaxCompletionTime.Value - call.OpenedAt) : (DateTime?)null,
                                    DistanceFromVolunteer = volunteer?.CurrentAddress != null ?
                                    VolunteerManager.CalculateDistance(latVol, lonVol, (double)boCall.Latitude, (double)boCall.Longitude) : 0,
                                    Description = boCall.Description
                                };
        if (filterType != null)
            {
                openCallsInList = openCallsInList.Where(call => call.Type == filterType);
            }
        openCallsInList = openCallsInList.Where(call => call.DistanceFromVolunteer <= volunteer.MaxCallDistance);
        if (sortField != null)
        {
            openCallsInList = sortField switch
            {
                BO.OpenCallInListFields.Id => openCallsInList.OrderBy(call => call.Id),
                BO.OpenCallInListFields.Type => openCallsInList.OrderBy(call => call.Type),
                BO.OpenCallInListFields.FullAddress => openCallsInList.OrderBy(call => call.FullAddress),
                BO.OpenCallInListFields.OpenTime => openCallsInList.OrderBy(call => call.OpenTime),
                BO.OpenCallInListFields.MaxEndTime => openCallsInList.OrderBy(call => call.MaxEndTime),
                BO.OpenCallInListFields.DistanceFromVolunteer => openCallsInList.OrderBy(call => call.DistanceFromVolunteer),
                _ => openCallsInList
            };
        }
        else 
            openCallsInList = openCallsInList.OrderBy(call => call.Id);
        return openCallsInList;
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

        //if (!Tools.IsValidAddress(call.FullAddress, out double latitude, out double longitude))
        //   throw new ArgumentException("Address is not valid.");

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
            Type = (DO.CallType)call.Type,
            Description = call.Description
        };

        // Attempt to update the call in the data layer
        try
        {
            _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id); //stage 5
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to update the call.", ex);
        }
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}
