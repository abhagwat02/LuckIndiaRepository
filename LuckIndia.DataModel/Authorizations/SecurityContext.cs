using LuckIndia.DataModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.Authorizations
{
   public class SecurityContext : ISecurityContext
    {
        public string AccessToken { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public IUser User { get; set; }

    }
}
