
using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CachingServices
{
    public sealed class AccessTokenCacheModel
    {
        public AccessToken AccessToken { get; set; }

        public int UserId { get; set; }

        public int ApplicationId { get; set; }
    }
}
