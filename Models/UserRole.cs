using System.Text.Json.Serialization;
using SpaBookingApp.Models;

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