using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.Interfaces
{
    public interface IUser
    {
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        int Id { get; set; }
    }
}
