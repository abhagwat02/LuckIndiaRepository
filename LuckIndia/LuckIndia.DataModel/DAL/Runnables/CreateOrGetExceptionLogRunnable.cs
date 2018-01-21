using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateOrGetExceptionLogRunnable : IRunnable
    {
        private readonly string _message;
        private readonly string _innerException;
        private readonly string _stackTrace;
        private readonly DateTime _exceptionLogDate;
        private readonly int _auditLogId;

        public CreateOrGetExceptionLogRunnable(string message, string innerException, string stackTrace, DateTime exceptionLogDate, int auditLogId)
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
            var createUser = new CreateExceptionLogRunnable(_message, _innerException, _stackTrace, _exceptionLogDate, _auditLogId);
            return context.ExecuteRunnable<T>(createUser);
        }
    }
}