using BO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
namespace Helpers;

using DalApi;
using DO;
using System.Text;
using System.Xml.Linq;
internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "null";

        var sb = new StringBuilder();
        var type = t.GetType();
        sb.AppendLine($"Type: {type.Name}");

        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(t);
            if (value is IEnumerable enumerable && value is not string)
            {
                sb.AppendLine($"{property.Name}: [");

                foreach (var item in enumerable)
                {
                    sb.AppendLine($"  {item}");
                }

                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine($"{property.Name}: {value}");
            }
        }
        return sb.ToString();
    }
    //public class DistanceCalculator
    //{
    //    /// <summary>
    //    /// Returns the distance between two addresses based on the specified distance type.מחשבת את המרחק בין שתי כתובות לפי סוג המרחק שצוין (קו אווירי, הליכה או נהיגה).
    //    /// </summary>
    //    /// <param name="address1">The first address.</param>
    //    /// <param name="address2">The second address.</param>
    //    /// <param name="distanceType">The type of distance: "straight-line", "walking", or "driving".</param>
    //    /// <returns>The calculated distance in kilometers.</returns>
    //    public static double GetDistance(string address1, string address2, string distanceType)
    //    {
    //        var (lat1, lon1) = GeocodingHelper.GetCoordinates(address1);
    //        var (lat2, lon2) = GeocodingHelper.GetCoordinates(address2);

    //        switch (distanceType.ToLower())
    //        {
    //            case "air":
    //                return CalculateHaversineDistance(lat1, lon1, lat2, lon2);

    //            case "walk":
    //            case "drive":
    //                return GetRouteDistance(lat1, lon1, lat2, lon2, distanceType);

    //            default:
    //                throw new ArgumentException("Invalid distance type. Use \"air\", \"walk\", or \"drive\".");
    //        }
    //    }

    //    /// <summary>
    //    /// Calculates the Haversine distance between two coordinates.
    //    /// </summary>
    //    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    //    {
    //        const double EarthRadiusKm = 6371;

    //        double dLat = ToRadians(lat2 - lat1);
    //        double dLon = ToRadians(lon2 - lon1);

    //        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
    //                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

    //        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //        return EarthRadiusKm * c;
    //    }

    //    /// <summary>
    //    /// Converts degrees to radians.
    //    /// </summary>
    //    private static double ToRadians(double angleInDegrees)
    //    {
    //        return angleInDegrees * Math.PI / 180.0;
    //    }

    //    /// <summary>
    //    /// Fetches the route distance (walking or driving) using the Google Maps API.
    //    /// </summary>
    //    private static double GetRouteDistance(double lat1, double lon1, double lat2, double lon2, string mode)
    //    {
    //        string baseUrl = "https://maps.googleapis.com/maps/api/directions/json";
    //        string travelMode = mode.ToLower() == "walking" ? "walking" : "driving";
    //        string url = $"{baseUrl}?origin={lat1},{lon1}&destination={lat2},{lon2}&mode={travelMode}&key=AIzaSyC0e5W3Qku-5Tj5Ys-WH5ByrMVq_7T7KG0";

    //        using (HttpClient client = new HttpClient())
    //        {
    //            HttpResponseMessage response = client.GetAsync(url).Result;
    //            if (!response.IsSuccessStatusCode)
    //            {
    //                throw new Exception($"Error fetching route distance. Status code: {response.StatusCode}");
    //            }

    //            string json = response.Content.ReadAsStringAsync().Result;
    //            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

    //            if (data?.routes == null || data.routes.Count == 0 || data.routes[0]?.legs == null || data.routes[0].legs.Count == 0)
    //            {
    //                throw new Exception("Invalid response from the API. Route data not found.");
    //            }

    //            double distanceMeters = data.routes[0].legs[0].distance.value;
    //            return distanceMeters / 1000.0; // Convert meters to kilometers
    //        }
    //    }
    //}

    public class GeocodingHelper
    {
        private const string BaseUrl = "https://nominatim.openstreetmap.org/search";

        /// <summary>
        /// Returns the coordinates (latitude and longitude) of a given address.
        /// </summary>
        public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return (0, 0);

            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");

                string url = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=xml";

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: Unable to get data from API. Status code: {response.StatusCode}");
                }

                string xml = response.Content.ReadAsStringAsync().Result;
                XDocument xmlData = XDocument.Parse(xml);

                var firstResult = xmlData.Descendants("place").FirstOrDefault();
                if (firstResult == null)
                {
                    throw new Exception("No results found for the given address.");
                }

                var latAttribute = firstResult.Attribute("lat");
                var lonAttribute = firstResult.Attribute("lon");

                if (latAttribute == null || lonAttribute == null)
                {
                    throw new Exception("Latitude or Longitude attribute is missing in the API response.");
                }

                double latitude = double.Parse(latAttribute.Value);
                double longitude = double.Parse(lonAttribute.Value);

                return (latitude, longitude);
            }
        }

        internal static object CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2 , double radius = 6371)
        {
            double dLat = ToRadians(latitude2 - latitude1);
            double dLon = ToRadians(longitude2 - longitude1);

            latitude1 = ToRadians(latitude1);
            latitude2 = ToRadians(latitude2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(latitude1) * Math.Cos(latitude2) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = radius * c;

            return distance;
        }
        internal static double ToRadians(double x) 
        {
            return x * Math.PI / 180;
        }
    }
    /// <summary>
    /// function that checks if the coordinates of a call match the coordinates based on his address.
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static bool CheckAddressVolunteer(BO.Volunteer volunteer)
    {
        if (volunteer.Latitude == null || volunteer.Longitude == null)
        {
            throw new Exception("Latitude or Longitude is missing in the Volunteer object.");
        }
        try
        {
            var (lat, lon) = GeocodingHelper.GetCoordinates(volunteer.FullAddress);
            const double tolerance = 0.0001;
            bool isLatitudeMatch = Math.Abs(volunteer.Latitude.Value - lat) < tolerance;
            bool isLongitudeMatch = Math.Abs(volunteer.Longitude.Value - lon) < tolerance;
            return isLatitudeMatch && isLongitudeMatch;
        }
        catch (Exception e)
        {
            throw new Exception("Error fetching coordinates for the volunteer address.", e);
        }
    }

    /// <summary>
    /// returns the status of the call based on the current time and the call's properties
    /// </summary>
    public static bool CheckAddressCall(BO.Call call)
    {
        try
        {
            var (lat, lon) = GeocodingHelper.GetCoordinates(call.FullAddress);
            const double tolerance = 0.0001;
            bool isLatitudeMatch = Math.Abs((double)(call.Latitude - lat)) < tolerance;
            bool isLongitudeMatch = Math.Abs((double)(call.Longitude - lon)) < tolerance;
            return isLatitudeMatch && isLongitudeMatch;
        }
        catch (Exception e)
        {
            throw new Exception("Error fetching coordinates for the volunteer address.");
        }
    }


    public static BO.CallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var now = DateTime.Now;

        var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.Id && a.CompletionTime == null);
        if (activeAssignment != null)
        {
            return BO.CallStatus.InProgress;
        }

        if (call.MaxCompletionTime.HasValue && now > call.MaxCompletionTime.Value)
        {
            return BO.CallStatus.Expired;
        }

        var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.Id && a.CompletionTime.HasValue);
        if (finishedAssignment != null)
        {
            return BO.CallStatus.Closed;
        }

        if (call.MaxCompletionTime.HasValue && now > call.MaxCompletionTime.Value.AddHours(1))
        {
            return BO.CallStatus.OpenAtRisk;
        }

        return BO.CallStatus.Open;
    }


    private static readonly HttpClient clientInstance = new HttpClient();// A reusable HttpClient instance for making HTTP requests
    public static (double? Latitude, double? Longitude) RetrieveLocationData(string? locationQuery)// Get the latitude and longitude of a location using a geocoding service
    {
        if (string.IsNullOrWhiteSpace(locationQuery))
            return (null, null); try
        {
            var apiKey = "67682f9834e26342582965lfv4f746b";
            var url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(locationQuery)}&api_key={apiKey}";
            var result = clientInstance.GetAsync(url).Result;
            var content = result.Content.ReadAsStringAsync().Result;
            if (!result.IsSuccessStatusCode) throw new HttpRequestException();
            var parsedResponse = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(content);
            if (parsedResponse != null && parsedResponse.Count > 0)
            {
                var firstEntry = parsedResponse[0];
                var longitude = firstEntry.ContainsKey("lon") ? double.Parse(firstEntry["lon"].ToString()) : (double?)null;
                var latitude = firstEntry.ContainsKey("lat") ? double.Parse(firstEntry["lat"].ToString()) : (double?)null;
                return (latitude, longitude);
            }
        }
        catch (HttpRequestException) { return (null, null); }
        catch (Exception) { return (null, null); }
        return (null, null);
    }

    public static BO.CallType ConvertCallType(DO.CallType doCallType)
    {
        switch (doCallType)
        {
            case DO.CallType.NotAllocated:
                return BO.CallType.NotAllocated;
            case DO.CallType.MedicalEmergency:
                return BO.CallType.MedicalEmergency;
            case DO.CallType.PatientTransport:
                return BO.CallType.PatientTransport;
            case DO.CallType.TrafficAccident:
                return BO.CallType.TrafficAccident;
            case DO.CallType.FirstAid:
                return BO.CallType.FirstAid;
            case DO.CallType.Rescue:
                return BO.CallType.Rescue;
            case DO.CallType.FireEmergency:
                return BO.CallType.FireEmergency;
            case DO.CallType.CardiacEmergency:
                return BO.CallType.CardiacEmergency;
            case DO.CallType.Poisoning:
                return BO.CallType.Poisoning;
            case DO.CallType.AllergicReaction:
                return BO.CallType.AllergicReaction;
            case DO.CallType.MassCausalities:
                return BO.CallType.MassCausalities;
            case DO.CallType.TerrorAttack:
                return BO.CallType.TerrorAttack;
            default:
                return BO.CallType.None;
        }
    }
}