using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateOrGetAuditLogRunnable : IRunnable
    {
        private readonly string _message;
        private readonly string _componentName;
        private readonly string _applicationName;
        private readonly DateTime _auditLogDate;

        public CreateOrGetAuditLogRunnable(string message, string componentName, string applicationName, DateTime auditLogDate)
        {
            _message = message;
            _componentName = componentName;
            _applicationName = applicationName;
            _auditLogDate = auditLogDate;

        }
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var createAuditLog = new CreateAuditLogRunnable(_message, _componentName, _applicationName, _auditLogDate);
            return context.ExecuteRunnable<T>(createAuditLog);
        }
    }
}