namespace DO;

/// <summary>
/// Call Entity represents a call request with all its properties.
/// </summary>
/// <param name="Id">Unique identifier for the call</param>
/// <param name="Type">Type of the call (e.g., food preparation, transport)</param>
/// <param name="Description">Description of the call with additional details</param>
/// <param name="Address">Full address of the call location</param>
/// <param name="Latitude">Latitude of the call's location, used for calculating distances</param>
/// <param name="Longitude">Longitude of the call's location, used for calculating distances</param>
/// <param name="OpenedAt">Date and time when the call was opened</param>
/// <param name="MaxCompletionTime">Optional maximum completion time for the call</param>
public record Call
(
    int Id,
    CallType Type,
    string? Description,
    string Address,
    double Latitude,
    double Longitude,
    DateTime OpenedAt,
    DateTime? MaxCompletionTime = null
)
{
    /// <summary>
    // Default constructor for stage 3
    /// </summary>
    public Call() : this(0, CallType.MedicalEmergency, null, "", 0, 0, DateTime.Now) { }
}