using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.Interfaces
{
    public interface ISecurityContext
    {
         string AccessToken { get; set; }
         DateTime EndDate { get; set; }
         DateTime StartDate { get; set; }
         IUser User { get; set; }
    }         

}
