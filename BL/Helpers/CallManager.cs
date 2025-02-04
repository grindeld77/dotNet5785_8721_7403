using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace Helpers;
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 


    /// <summary>
    /// Converts a DO.Call object to a BO.Call object.
    /// </summary>
    /// <param name="call"></param>
    /// <returns></returns>
    /// <exception cref="BO.BloesNotExistException"></exception>
    internal static BO.Call ConvertDoCallToBoCall(DO.Call? call)
    {
        if (call == null)
        {
            throw new BO.BloesNotExistException("Call does not exist.");
        }

        IEnumerable<Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll()?.Where(a => a.CallId == call.Id) ?? Enumerable.Empty<DO.Assignment>();

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

    /// <summary>
    /// Converts a list of DO.Assignment objects to a list of BO.CallAssignInList objects.
    /// </summary>
    /// <param name="assignments"></param>
    /// <returns></returns>
    private static IEnumerable<CallAssignInList> AssignmentToCallAssignInList(IEnumerable<DO.Assignment>? assignments)
    {
        if (assignments == null || !assignments.Any())
        {
            return Enumerable.Empty<CallAssignInList>();
        }

        string Name;
        lock (AdminManager.BlMutex) //stage 7
            Name = s_dal.Volunteer.Read(assignments.Last().VolunteerId)?.FullName ?? "Unknown";

        return assignments.Select(a => new CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            EndTime = a.CompletionTime ?? default(DateTime),
            StartTime = a.EntryTime,
            VolunteerName = Name,
            Status = (BO.CompletionStatus)a.CompletionStatus
        });
    }

    /// <summary>
    /// Gets all the open calls.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    internal static IEnumerable<ClosedCallInList> GetAllClosedCalls(int volunteerId)
    {
        IEnumerable<DO.Assignment> list = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
        lock (AdminManager.BlMutex) //stage 7
        {
            return from a in list
                   let c = s_dal.Call.Read(a.CallId)
                   where a.CompletionStatus != null
                    select new BO.ClosedCallInList
                    {
                        Id = c.Id,
                        Type = (BO.CallType)c.Type,
                        FullAddress = c.Address,
                        OpenTime = c.OpenedAt,
                        AssignedTime = a.EntryTime,
                        ClosedTime = a.CompletionTime,
                        Status = (BO.CompletionStatus?)a.CompletionStatus
                    };
        }
    }
    /// <summary>
    /// Converts a DO.Call object to a BO.CallInList object.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static CallInList converterFromDoToBoCallInList(DO.Call c)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) // stage 7
        {
            assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == c.Id).ToList();
        }

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

    /// <summary>
    /// return if the volunteer is busy or not
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    internal static bool IsVolunteerBusy(int volunteerId)
    {
        lock (AdminManager.BlMutex) // stage 7
        {
            var v = s_dal.Volunteer.Read(volunteerId);
            var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId && a.CompletionStatus == null);
            return assignments.Any();
        }
    }

    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        // Set thread name for easier debugging
        // Thread.CurrentThread.Name = $"PeriodicCallUpdates"; ??? to review

        // Local list to store IDs for notifications outside the lock
        List<int> expiredCallIds = new();

        // Step 1: Retrieve all calls from the data source
        List<DO.Call> activeCalls;
        lock (AdminManager.BlMutex) // Lock for data retrieval
        {
            activeCalls = s_dal.Call.ReadAll()
                                   .Where(call => call.MaxCompletionTime > newClock && call.MaxCompletionTime <= newClock)
                                   .ToList();
        }

        // Step 2: Process calls and perform updates
        foreach (var call in activeCalls)
        {
            // Assuming these are expired calls that require updates
            lock (AdminManager.BlMutex) // Lock for database updates
            {
                s_dal.Call.Update(call with { MaxCompletionTime = null }); // Update the call
            }

            // Add the call ID to the local list for notifications
            expiredCallIds.Add(call.Id);
        }

        // Step 3: Send notifications outside the lock
        foreach (var callId in expiredCallIds)
        {
            Observers.NotifyItemUpdated(callId); // Notify about the specific updated item
        }

        // Step 4: Check if the list requires a global update notification
        if (oldClock.Year != newClock.Year || expiredCallIds.Any())
        {
            Observers.NotifyListUpdated(); // Notify about a global list update
        }
    }

    public static IEnumerable <BO.OpenCallInList>  GetOpenCallForVolunteer(int id)
    {
        // Retrieve the volunteer from the DAL

        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exists");
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex) //stage 7
            allCalls = s_dal.Call.ReadAll();

        IEnumerable<Assignment> allAssignments;
        lock (AdminManager.BlMutex) //stage 7
            allAssignments = s_dal.Assignment.ReadAll();

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
                                  OpenTime = call.OpenedAt,
                                  MaxEndTime = call.MaxCompletionTime,// ? AdminManager.Now.Add(call.MaxCompletionTime.Value - call.OpenedAt) : (DateTime?)null,
                                  DistanceFromVolunteer = volunteer?.CurrentAddress != null ?
                                  VolunteerManager.CalculateDistance(latVol, lonVol, (double)boCall.Latitude, (double)boCall.Longitude) : 0,
                                  Description = boCall.Description
                              };
        openCallsInList = openCallsInList.Where(call => call.DistanceFromVolunteer <= volunteer.MaxCallDistance);

        return openCallsInList;
    }
    internal static void UpdateCall(BO.Call call)
    {
        if (call == null)
            throw new ArgumentNullException(nameof(call));

        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new ArgumentException("Address is required.");

        if (call.OpenTime >= call.MaxEndTime)
            throw new ArgumentException("Open time must be earlier than the maximum finish time.");

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
                s_dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id); //stage 5
        }
        catch (Exception ex)
        {
            // Handle exceptions from the data layer and rethrow as a BO exception
            throw new BO.BlGeneralException("Failed to update the call.", ex);
        }
    }

    internal static async Task SendCancelationMail(DO.Assignment a)
    {
        var fromAddress = new MailAddress("shimon78900@gmail.com");
        MailAddress? toAddress = null;
        lock (AdminManager.BlMutex)
            toAddress = new MailAddress(s_dal.Volunteer.Read(a.VolunteerId)!.Email, s_dal.Volunteer.Read(a.VolunteerId)!.FullName);
        const string fromPassword = "yhmg gvrn twft wqsx";
        const string subject = "Assignment Cancelation";
        string body = "Your assignment is no longer under your treatment!\nThank you for your service.\n";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };
        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            await smtp.SendMailAsync(message);
        }
    }

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    internal static async Task SendCallOpenMail(BO.Call call)
    {
        IEnumerable<DO.Volunteer> doVolunteers;
        await _semaphore.WaitAsync();
        try
        {
            doVolunteers = s_dal.Volunteer.ReadAll();
        }
        finally
        {
            _semaphore.Release();
        }

        var Volunteers = doVolunteers.Where(volunteer => volunteer.MaxCallDistance >= VolunteerManager.CalculateDistance((double)volunteer.Latitude, (double)volunteer.Longitude, (double)call.Latitude, (double)call.Longitude))
                                     .Where(volunteer => volunteer.IsActive);
        foreach (var Volunteer in Volunteers)
        {
            var fromAddress = new MailAddress("shimon78900@gmail.com");
            MailAddress? toAddress = null;
            await _semaphore.WaitAsync(); 
            try
            {
                var volunteerData = s_dal.Volunteer.Read(Volunteer.Id);
                if (volunteerData == null)
                    continue;

                toAddress = new MailAddress(volunteerData.Email, volunteerData.Email);
            }
            finally
            {
                _semaphore.Release(); // שחרור הנעילה
            }
            toAddress = new MailAddress(s_dal.Volunteer.Read(Volunteer.Id)!.Email, s_dal.Volunteer.Read(Volunteer.Id)!.FullName);
            const string fromPassword = "yhmg gvrn twft wqsx";
            const string subject = "New Call Open in your area";
            string body = "This call is open in your area!\n" + call.ToString();

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }

    internal static IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exists");
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex) //stage 7
            allCalls = s_dal.Call.ReadAll();

        IEnumerable<Assignment> allAssignments;
        lock (AdminManager.BlMutex) //stage 7
            allAssignments = s_dal.Assignment.ReadAll();

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
                                  OpenTime = call.OpenedAt,
                                  MaxEndTime = call.MaxCompletionTime,// ? AdminManager.Now.Add(call.MaxCompletionTime.Value - call.OpenedAt) : (DateTime?)null,
                                  DistanceFromVolunteer = volunteer?.CurrentAddress != null ?
                                  VolunteerManager.CalculateDistance(latVol, lonVol, (double)boCall.Latitude, (double)boCall.Longitude) : 0,
                                  Description = boCall.Description
                              };
        openCallsInList = openCallsInList.Where(call => call.DistanceFromVolunteer <= volunteer.MaxCallDistance);
        return openCallsInList;
    }

    //internal static async Task AddressCalc(DO.Call call)
    //{
    //    (double latitude, double longitude) = await Tools.GeocodingHelper.GetCoordinates(call.Address);

    //    DO.Call newcall = new DO.Call();

    //    newcall = call with { Latitude = latitude, Longitude = longitude };

    //}
}

