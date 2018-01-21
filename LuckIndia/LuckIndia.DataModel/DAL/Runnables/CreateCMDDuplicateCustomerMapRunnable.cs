using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDDuplicateCustomerMapRunnable : IRunnable
    {
        private readonly int _duplicateCustomerID;
        private readonly int _existingCustomerID;
        private readonly bool? _isActive;

        public CreateCMDDuplicateCustomerMapRunnable(int DuplicateCustomerID, int ExistingCustomerID, bool? IsActive)
        {
            _duplicateCustomerID = DuplicateCustomerID;
            _isActive = IsActive;
            _existingCustomerID = ExistingCustomerID;
           
        }


        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var cmdCustomer = new CMDCustomerDuplicateMap
            {
                DuplicateCustomerID = _duplicateCustomerID,
                ExistingCustomerID = _existingCustomerID,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = ALPHAEON.CMD.Common.Constants.Configuration.UpdatedBy,
                UpdatedDate = DateTime.UtcNow
            };

            return context.Create(cmdCustomer, false) as T;
        }
    }
}