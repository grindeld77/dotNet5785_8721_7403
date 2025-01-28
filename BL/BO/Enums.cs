namespace BO;

public enum Role//role
{
    Admin,
    Volunteer
}
public enum CallType//call type
{
    NotAllocated,        // Not allocated yet
    MedicalEmergency,   // Medical emergency
    PatientTransport,    // Patient transport
    TrafficAccident,     // Traffic accident
    FirstAid,            // First aid
    Rescue,              // Rescue operation
    FireEmergency,       // Fire emergency
    CardiacEmergency,    // Cardiac emergency
    Poisoning,           // Poisoning
    AllergicReaction,    // Allergic reaction
    MassCausalities,     // Mass casualties
    TerrorAttack,
    None
}

public enum CallStatus//call status
{
    Open,               //call is open
    InProgress,         //call is in progress
    Closed,             //call is closed
    Expired,            //call expired
    OpenAtRisk,         //call is open and at risk
    InProgressAtRisk,    //call is in progress and at risk
    ALL
}
public enum CompletionStatus//call completion status
{
    Handled, //call was handled
    SelfCancel, //volunteer canceled the call
    AdminCancel,    // admin canceled the call
    Expired,// call expired
    InTreatment // call is in treatment
}
public enum DistanceType//distance type
{
    Aerial, //air distance
    Walking, //walking distance
    Driving //driving distance
}

// Enumeration for time units to advance the clock.
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}



public enum VolunteerFieldVolunteerInList
{
    Id,
    FullName,
    TotalCompletedCalls,
    TotalCancelledCalls,
    TotalExpiredCalls,
    CurrentCallId,
    CurrentCallType,
}

public enum CallInListFields
{
    AssignmentId,
    CallId,
    CallType,
    OpenTime,
    RemainingTime,
    LastVolunteer,
    TotalHandlingTime,
    Status,
    TotalAssignments,
}

public enum ClosedCallInListFields
{
    Id,
    Type,
    CallType,
    OpenTime,
    AssignedTime,
    ClosedTime,
    FullAddress,
    Status,
}
public enum OpenCallInListFields
{
    Id,
    Type,
    Description,
    FullAddress,
    OpenTime,
    MaxEndTime,
    DistanceFromVolunteer,
}