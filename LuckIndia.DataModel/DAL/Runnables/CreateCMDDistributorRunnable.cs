using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDDistributorRunnable : IRunnable
    {

        private readonly string _name;
        private readonly int? _cmdContactID;
        private readonly string _description;
        private readonly DateTime _updatedDate;
        private readonly DateTime _createdDate;
        private readonly bool _isActive;
        private readonly string _updatedBy;


        public CreateCMDDistributorRunnable(string name, int? cmdContactId, string description, DateTime updatedDate, DateTime createdDate, bool isActive, string updateBy)
        {
            _name = name;
            _cmdContactID = cmdContactId;
            _description = description;
            _createdDate = createdDate;
            _updatedDate = updatedDate;
            _isActive = isActive;
            _updatedBy = updateBy;
        }

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var distributorLog = new CMDDistributor
              {

                  Name = _name.RemoveQuotes(),
                  CMDContactID = _cmdContactID,
                  Description = _description.RemoveQuotes(),
                  CreatedDate = _createdDate,
                  UpdatedDate = _updatedDate,
                  IsActive = _isActive,
                  UpdatedBy = _updatedBy.RemoveQuotes()

              };

            return context.Create(distributorLog, false) as T;
        }
    }
}