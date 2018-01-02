namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public interface IRunnable
    {
        void Execute(CMDDatabaseContext context);

        T Execute<T>(CMDDatabaseContext context) where T : class;
    }
}
