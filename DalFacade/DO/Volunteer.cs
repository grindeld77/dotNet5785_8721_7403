using System.Data;

namespace DO;

/// <summary>
/// Volunteer Entity represents a volunteer with all its properties.
/// </summary>
/// <param name="Id"> Unique ID of the volunteer </param>
/// <param name="FullName">Full name of the volunteer </param>
/// <param name="MobilePhone">Mobile phone number - 10 digits, starting with 0</param>
/// <param name="Email">Volunteer’s email address </param>
/// <param name="Role">Role of the volunteer - enum </param>
/// <param name="IsActive">is the volunteer active in the organization</param>
/// <param name="Password">Volunteer’s password - can be null</param>
/// <param name="CurrentAddress">Full current address of the volunteer - can be null</param>
/// <param name="Latitude">Latitude of the volunteer's location - updates if address exists</param>
/// <param name="Longitude">Longitude of the volunteer's location - updates if address exists</param>
/// <param name="MaxCallDistance">Maximum distance within which the volunteer can accept calls</param>
/// <param name="DistancePreference">Preferred type of distance calculation - default is Aerial</param>
public record Volunteer
(
    int Id,
    string FullName,
    string MobilePhone,
    string Email,
    Role Role,
    bool IsActive,
    string? Password = null,
    string? CurrentAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    double? MaxCallDistance = null,
    DistanceType DistancePreference = DistanceType.Aerial
)
{
    /// <summary>
    // Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", Role.Volunteer, true) { }
}