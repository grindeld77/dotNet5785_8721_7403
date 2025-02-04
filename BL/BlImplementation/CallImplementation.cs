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

    /// <summary>
    /// Add a new call to the system.
    /// </summary>
    /// <param name="boCall"></param>
    /// 

    public async Task AddCall(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // שלב 7

        try
        {
            // בדוק אם האובייקט אינו null
            if (boCall == null)
                throw new BO.BloesNotExistException(nameof(boCall));

            // בדוק אם הכתובת תקינה
            if (string.IsNullOrWhiteSpace(boCall.FullAddress))
                throw new BO.BlInvalidAddressException("Address is required.");

            // בדוק אם זמן פתיחה תקין
            if (boCall.OpenTime >= boCall.MaxEndTime)
                throw new BO.BlInvalidTimeException("Open time must be earlier than the maximum finish time.");

            // קבל את הקורדינטות באופן אסינכרוני
            (boCall.Latitude, boCall.Longitude) = await Tools.GeocodingHelper.GetCoordinates(boCall.FullAddress);

            // צור אובייקט חדש מהנתונים
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

            // נעילה לצורך עדכון אטומי
            lock (AdminManager.BlMutex)
            {
                _dal.Call.Create(call);
            }

            // שלח מייל וידע את התצפיתנים
            CallManager.SendCallOpenMail(boCall);
            CallManager.Observers.NotifyListUpdated(); // שלב 5
        }
        catch (Exception ex)
        {
            // טיפול בחריגות וזריקתן מחדש
            throw char.IsDigit(ex.Message[0]) ? new BO.BlGeneralException("Failed to add the call.", ex) : ex;
        }
    }


    /// <summary>
    /// Assign a volunteer to a call.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="callId"></param>
    void ICall.AssignVolunteerToCall(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
         
        AssignmentManager.AssignVolunteerToCall(volunteerId, callId);
    }


    /// <summary>
    /// Cancel a call assignment.
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="assignmentId"></param>
    void ICall.CancelCallAssignment(int requesterId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        AssignmentManager.CancelAssignment(requesterId, assignmentId);
    }

    /// <summary>
    /// Complete a call assignment.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="assignmentId"></param>
    void ICall.CompleteCallAssignment(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        AssignmentManager.UpdateCallForVolunteer(volunteerId, assignmentId);
    }

    /// <summary>
    /// Delete a call from the system.
    /// </summary>
    /// <param name="callId"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="BO.BlInvalidOperationException"></exception>
    /// <exception cref="BO.BlGeneralException"></exception>
    void ICall.DeleteCall(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        // Validate input parameter
        if (callId < 0)
            throw new ArgumentException("Invalid call ID.", nameof(callId));

        // Retrieve the call from the data layer
        DO.Call call;
        lock (AdminManager.BlMutex) //stage 7
            call = _dal.Call.Read(a => a.Id == callId);

        bool any;
        lock (AdminManager.BlMutex) //stage 7
            any = _dal.Assignment.ReadAll(a => a.CallId == callId).Any();
        // Check if the call is already assigned
        if (any || ( call.Status!=DO.CallStatus.Open && call.Status !=DO.CallStatus.OpenAtRisk))
        {
            throw new BO.BlInvalidOperationException("Call is already assigned");
        }

        // Attempt to delete the call from the data layer
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }

        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to delete the call.", ex);

        }
    }

    /// <summary>
    /// Retrieve a list of calls based on specified criteria.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralException"></exception>
    int[] ICall.GetCallCountsByStatus()
    {
        try
        {
            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex) //stage 7
                calls = _dal.Call.ReadAll(); // Fetch all calls from the data layer
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

    /// <summary>
    /// Retrieve the details of a specific call.
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralException"></exception>
    BO.Call ICall.GetCallDetails(int callId)
    {
        try
        {
            DO.Call call;
            lock (AdminManager.BlMutex) //stage 7
                call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");
            return CallManager.ConvertDoCallToBoCall(call);
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to retrieve call details.", ex);
        }
    }


    /// <summary>
    /// Retrieve a list of calls based on specified criteria.
    /// </summary>
    /// <param name="filterField"></param>
    /// <param name="filterValue"></param>
    /// <param name="sortField"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralException"></exception>
    IEnumerable<BO.CallInList> ICall.GetCalls(BO.CallStatus? filterField, object? filterValue, BO.CallInListFields? sortField, BO.CallType? filter2)
    {
        try
        {
            IEnumerable<DO.Call> doCalls;
            IEnumerable<BO.CallInList> callsList = null;

            lock (AdminManager.BlMutex) //stage 7
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
            if  (filter2 != null)
            {
                switch (filter2) // Filter the calls based on the specified field
                {
                    case BO.CallType.NotAllocated:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.NotAllocated);
                        break;
                    case BO.CallType.MedicalEmergency:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.MedicalEmergency);
                        break;
                    case BO.CallType.PatientTransport:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.PatientTransport);
                        break;
                    case BO.CallType.TrafficAccident:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.TrafficAccident);
                        break;
                    case BO.CallType.FirstAid:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.FirstAid);
                        break;
                    case BO.CallType.Rescue:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.Rescue);
                        break;
                    case BO.CallType.FireEmergency:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.FireEmergency);
                        break;
                    case BO.CallType.CardiacEmergency:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.CardiacEmergency);
                        break;
                    case BO.CallType.Poisoning:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.Poisoning);
                        break;
                    case BO.CallType.AllergicReaction:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.AllergicReaction);
                        break;
                    case BO.CallType.MassCausalities:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.MassCausalities);
                        break;
                    case BO.CallType.TerrorAttack:
                        callsList = callsList.Where(call => call.CallType == BO.CallType.TerrorAttack);
                        break;
                    case BO.CallType.None:
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

    /// <summary>
    /// Retrieve a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="filterField"></param>
    /// <param name="sortField"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralException"></exception>
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

    /// <summary>
    /// Retrieve a list of open calls available for a volunteer.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="filterType"></param>
    /// <param name="sortField"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    IEnumerable<BO.OpenCallInList> ICall.GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.OpenCallInListFields? sortField)
    {
        // Retrieve the volunteer from the DAL

        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exists");
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex) //stage 7
            allCalls = _dal.Call.ReadAll();

        IEnumerable<Assignment> allAssignments;
        lock (AdminManager.BlMutex) //stage 7
            allAssignments = _dal.Assignment.ReadAll();

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

    /// <summary>
    /// Update an existing call in the system.
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="BO.BlGeneralException"></exception>
    void ICall.UpdateCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Update(doCall);
            
            CallManager.SendCallOpenMail(call);
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
