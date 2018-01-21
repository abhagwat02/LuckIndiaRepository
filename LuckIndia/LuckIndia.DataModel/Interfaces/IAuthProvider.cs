using LuckIndia.DataModel.Authorizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.Interfaces
{
    public interface IAuthProvider
    {
        SecurityContext Verify(string token);
    }
}
