using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateAuditLogRunnable : IRunnable
    {
        private readonly string _message;
        private readonly string _componentName;
        private readonly string _applicationName;
        private readonly DateTime _auditLogDate;

        public CreateAuditLogRunnable(string message, string componentName, string applicationName, DateTime auditLogDate)
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
            var auditLog = new CMDAuditLog
            {
                Message = _message,
                ApplicationName = _applicationName,
                ComponentName = _componentName,
                AuditLogDate = _auditLogDate
            };

            return context.Create(auditLog, false) as T;
        }
    }
}