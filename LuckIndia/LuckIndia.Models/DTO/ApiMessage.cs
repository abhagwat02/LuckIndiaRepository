
using LuckIndia.Models.Interfaces;

namespace LuckIndia.APIs.DTO
{
    /// <summary>
    /// ApiMessage class used for Get/Set property like message, data object
    /// </summary>
    public sealed class ApiMessage : IDTO
    {
        /// <summary>
        /// Get/set property for message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get/set property for object Data
        /// </summary>
        public object Data { get; set; }
    }
}
