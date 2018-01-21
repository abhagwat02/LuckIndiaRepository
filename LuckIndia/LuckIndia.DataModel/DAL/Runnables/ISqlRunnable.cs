using System.Data.SqlClient;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public interface ISqlRunnable
    {
        SqlCommand GetSqlCommand();
    }
}
