namespace DO;

/// <summary>
/// Assignment Entity represents the linkage between a call and a volunteer who chose to handle it.
/// </summary>
/// <param name="Id">Unique assignment ID, representing the unique identifier for the assignment entity.</param>
/// <param name="CallId">ID of the call that the volunteer has chosen to handle.</param>
/// <param name="VolunteerId">ID of the volunteer who chose to handle the call.</param>
/// <param name="EntryTime">Date and time when the call was first taken by the volunteer.</param>
/// <param name="CompletionTime">Date and time when the volunteer completed handling the call, can be null if the call is not yet completed.</param>
/// <param name="CompletionStatus">Status of how the call was completed: Handled, SelfCancel, AdminCancel, Expired.</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTime,
    DateTime? CompletionTime = null,
    CompletionStatus? CompletionStatus = null
)
{
    /// <summary>
    /// Default constructor for stage 3, initializes the assignment with default values.
    /// </summary>
    public Assignment() : this(0, 0, 0, DateTime.MinValue) { }
}