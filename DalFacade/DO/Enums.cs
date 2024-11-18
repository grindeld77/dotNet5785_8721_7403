namespace DO;
public enum Role
{
    Manager, 
    Volunteer 
}

public enum DistanceType
{
    Aerial, //air distance
    Walking, //walking distance
    Driving //driving distance
}

public enum CallType
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
    TerrorAttack         // Terror attack           
}
public enum CompletionStatus
{
    Handled, //call was handled
    SelfCancel, //volunteer canceled the call
    AdminCancel,    // admin canceled the call
    Expired // call expired
}