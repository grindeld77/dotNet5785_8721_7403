namespace BlImplementation;
using BlApi;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    void ICall.AddCall(BO.Call call)
    {
        throw new NotImplementedException();
    }

    void ICall.AssignVolunteerToCall(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    void ICall.CancelCallAssignment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    void ICall.CompleteCallAssignment(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    void ICall.DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    int[] ICall.GetCallCountsByStatus()
    {
        throw new NotImplementedException();
    }

    BO.Call ICall.GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    IEnumerable<BO.CallInList> ICall.GetCalls(BO.CallStatus? filterField, object? filterValue, BO.CallStatus? sortField)
    {
        throw new NotImplementedException();
    }

    IEnumerable<BO.CallInList> ICall.GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.CallStatus? sortField)
    {
        throw new NotImplementedException();
    }

    IEnumerable<BO.OpenCallInList> ICall.GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.CallStatus? sortField)
    {
        throw new NotImplementedException();
    }

    void ICall.UpdateCall(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
