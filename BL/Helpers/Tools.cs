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
    public static async Task<DO.Volunteer> UpdateCoordinatesForVolunteerAsync(DO.Volunteer volunteer, string? address)
    {
        if (!string.IsNullOrWhiteSpace(address))
        {
            var coordinates = await Tools.GeocodingHelper.GetCoordinates(address);
            if (coordinates.Latitude != 0 && coordinates.Longitude != 0)
            {
                volunteer = volunteer with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };
            }
        }

        return volunteer;
    }

    public class GeocodingHelper
    {
        private const string BaseUrl = "https://nominatim.openstreetmap.org/search";

        /// <summary>
        /// Returns the coordinates (latitude and longitude) of a given address.
        /// </summary>
        public static async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return (0, 0);
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
                string url = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=xml";

                HttpResponseMessage response = await client.GetAsync(url); // await - קריאה אסינכרונית
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: Unable to get data from API. Status code: {response.StatusCode}");
                }

                string xml = await response.Content.ReadAsStringAsync(); // await - קריאה אסינכרונית
                XDocument xmlData = XDocument.Parse(xml);

                var firstResult = xmlData.Descendants("place").FirstOrDefault();
                if (firstResult == null)
                {
                    throw new Exception("No results found for the given address.");
                }

                double latitude = double.Parse(firstResult.Attribute("lat")?.Value ?? "0");
                double longitude = double.Parse(firstResult.Attribute("lon")?.Value ?? "0");

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