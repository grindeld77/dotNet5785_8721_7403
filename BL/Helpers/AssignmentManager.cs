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

    //internal static void AssignVolunteerToCall(int volunteerId, int callId)
    //{
    //}

    //internal static void UpdateCallForVolunteer(int volunteerId, int callId, DateTime completionTime, BO.CompletionStatus completionStatus)
    //{
    //}

    //internal static void CancelAssignment(int assignmentId)
    //{

    //}
}
