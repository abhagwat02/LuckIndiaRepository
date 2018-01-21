using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateExceptionLogRunnable : IRunnable
    {
        private readonly string _message;
        private readonly string _innerException;
        private readonly string _stackTrace;
        private readonly DateTime _exceptionLogDate;
        private readonly int _auditLogId;

        public CreateExceptionLogRunnable(string message, string innerException, string stackTrace, DateTime exceptionLogDate, int auditLogId)
        {
            _message = message;
            _innerException = innerException;
            _stackTrace = stackTrace;
            _exceptionLogDate = exceptionLogDate;
            _auditLogId = auditLogId;
        }
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var exceptionLog = new CMDExceptionLog
            {
                CMDAuditLogId = _auditLogId,
                ExceptionLogDate = _exceptionLogDate,
                InnerException = _innerException,
                Message = _message,
                StackTrace = _stackTrace
            };

            return context.Create(exceptionLog, false) as T;
        }
    }
}