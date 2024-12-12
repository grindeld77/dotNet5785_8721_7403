using BO;

namespace BlApi;

public interface ICall
{
    // return the count of calls by their status as an array where each index corresponds to the status ID.
    int[] GetCallCountsByStatus();

    // return a sorted and filtered list of calls based on specified criteria.
    IEnumerable<CallInList> GetCalls(CallStatus? filterField, object? filterValue, CallStatus? sortField); 

    // return details of a specific call by its ID. Throws an exception if the call is not found.
    Call GetCallDetails(int callId);

    // Updates the details of a specific call. Throws an exception if the call is not found.
    void UpdateCall(Call call);

    // Deletes a specific call by its ID. Ensures the call is open and unassigned. Throws an exception if deletion is not allowed or the call is not found.
    void DeleteCall(int callId);

    // Adds a new call to the system. Ensures the data is valid. Throws an exception if a call with the same ID already exists.
    void AddCall(Call call);

    // return a list of calls that are closed and have been handled by a specific volunteer.
    IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallStatus? filterField, CallStatus? sortField);

    // return a list of open calls available for a volunteer, including their distance from the volunteer.
    IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? filterType, CallStatus? sortField);

    // Marks the completion of handling a specific call assignment by a volunteer.
    void CompleteCallAssignment(int volunteerId, int assignmentId);

    // Cancels an active call assignment. Authorization checks apply for the requester.
    void CancelCallAssignment(int requesterId, int assignmentId);

    // Assigns a volunteer to a specific call, marking it for handling.
    void AssignVolunteerToCall(int volunteerId, int callId);
}
