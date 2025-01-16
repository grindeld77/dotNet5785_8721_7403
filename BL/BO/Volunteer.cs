using DO;

namespace BO
{

    public class Volunteer
    {
        public int Id { get; set; } // Volunteer ID
        public string FullName { get; set; } // Full name
        public string PhoneNumber { get; set; } // Phone number
        public string Email { get; set; } // Email address
        public string? PasswordHash { get; set; } // Encrypted password
        public string? FullAddress { get; set; } // Full address
        public double? Latitude { get; set; } // Latitude
        public double? Longitude { get; set; } // Longitude
        public Role Role { get; set; } // Role: Admin, Volunteer
        public bool IsActive { get; set; } // Is the volunteer active
        public double? MaxDistanceForCall { get; set; } // Maximum distance for call
        public DistanceType DistanceType { get; set; } = DistanceType.Aerial; // Distance type: Aerial, Road
        public int TotalCompletedCalls { get; set; } // Total completed calls
        public int TotalCancelledCalls { get; set; } // Total cancelled calls
        public int TotalExpiredCalls { get; set; } // Total expired calls
        public BO.CallInProgress? CurrentCall { get; set; } // Current call

        public override string ToString()
        {
            return
                   $"Volunteer: {FullName} (ID: {Id})\n" +
                   $"Phone: {PhoneNumber}\n" +
                   $"Email: {Email}\n" +
                   $"Address: {FullAddress}\n" +
                   $"Role: {(Role == Role.Admin ? "Admin" : "Volunteer")}\n" +
                   $"Active: {(IsActive ? "Yes" : "No")}\n" +
                   $"Max distance for call: {MaxDistanceForCall} {DistanceType}\n" +
                   $"Total Completed Calls: {TotalCompletedCalls}\n" +
                   $"Total Cancelled Calls: {TotalCancelledCalls}\n" +
                   $"Total Expired Calls: {TotalExpiredCalls}\n" +
                   $"Current call: {(CurrentCall != null ? CurrentCall.ToString() : "None")}";
        }
    }

    public class CallInProgress
    {
        public int Id { get; init; } // Assignment call ID
        public int CallId { get; init; } // Call ID
        public CallType Call { get; set; } // Call type: Food, Medicine, Other
        public string? Description { get; set; } // Call description
        public string FullAddress { get; set; } // Full address
        public DateTime OpenTime { get; set; } // Call open time
        public DateTime? MaxCompletionTime { get; set; } // Maximum completion time
        public DateTime StartTime { get; set; } // Start time of handling
        public double DistanceFromVolunteer { get; set; } // Distance from volunteer
        public CallStatus Status { get; set; } // Call status: InProgress, AtRisk

        public override string ToString()
        {
            return $"Call in Progress: {Id}\n" +
                   $"Call: {CallId}\n" +
                   $"Type: {Call}\n" +
                   $"Description: {Description}\n" +
                   $"Address: {FullAddress}\n" +
                   $"Open Time: {OpenTime}\n" +
                   $"Max Completion Time: {(MaxCompletionTime.HasValue ? MaxCompletionTime.Value : "N/A")}\n" +
                   $"Start Time: {StartTime}\n" +
                   $"Distance from Volunteer: {DistanceFromVolunteer} km\n" +
                   $"Status: {Status}";
        }
    }


}
