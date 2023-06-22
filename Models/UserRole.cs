using System.Text.Json.Serialization;

namespace SpaBookingApp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin = 1,
        Staff = 2,
        Customer = 3
    }
}