using System.Security.Principal;

namespace LuckIndia.DataModel.Authorizations

{
    /// <summary>
    /// This class isn't used anywhere except to honor the contract needed for ApiPrincipal
    /// </summary>
    public class ApiIdentity : IIdentity
    {
        /// <summary>
        /// Get property for Authentication Type
        /// </summary>
        public string AuthenticationType
        {
            get { return "AccessToken"; }
        }

        /// <summary>
        /// boolean property as isAuthenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get { return true; }
        }

        /// <summary>
        /// Get property for ApiUser name
        /// </summary>
        public string Name
        {
            get { return "ApiUser"; }
        }
    }
}
