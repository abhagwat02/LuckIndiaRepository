
using LuckIndia.DataModel.Interfaces;
using System.Security;
using System.Security.Principal;

namespace LuckIndia.DataModel.Authorizations

{
    /// <summary>
    /// Implements methods for basic functionality of a principal object.
    /// </summary>
    public class ApiPrincipal : IPrincipal
    {
        /// <summary>
        /// Constructor to set access token and instantiate identity.
        /// </summary>
        /// <param name="accessToken"></param>
        public ApiPrincipal(string accessToken, SecurityContext securityContext)
        {
            this.Identity = new ApiIdentity();
            this.AccessToken = accessToken;
            this.securityContext = securityContext;
        }

        /// <summary>
        /// Get/Set AccessToken property
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Get/Set Identity property
        /// </summary>
        public IIdentity Identity { get; set; }

        public SecurityContext securityContext { get; set; }

        /// <summary>
        /// This method is here only to honor IPrincipal.
        /// Returns false for anything because our system does not use this method.
        /// </summary>
        /// <param name="role"></param>
        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
