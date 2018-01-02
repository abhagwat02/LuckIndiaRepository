using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDAccountRunnable : IRunnable
    {
        private readonly string _name;
        private readonly int? _cmdPracticeID;
        private readonly string _fexTaxID;
        private readonly int? _cmdAccountTypeID;
        private readonly int? _netSuiteID;
        private readonly int? _originalBusinessUnitID;
        private readonly int? _originalDistributorID;
        private readonly bool _isActive;
        private readonly DateTime _updatedDate;
        private readonly DateTime _createdDate;
        private readonly string _updatedBy;
        private readonly int? _CMDBusinessUnitID;


        public CreateCMDAccountRunnable(string name, int? cmdPracticeID, string fexTaxID, int? cmdAccountTypeID, int? netSuiteID, int? originalBusinessUnitID, int? originalDistributorID, bool isActive, DateTime updatedDate, DateTime createdDate, string updateBy, int? CMDbusinessUnitID)
        {
            _name = name;
            _cmdPracticeID = cmdPracticeID;
            _fexTaxID = fexTaxID;
            _cmdAccountTypeID = cmdAccountTypeID;
            _netSuiteID = netSuiteID;
            _originalBusinessUnitID = originalBusinessUnitID;
            _originalDistributorID = originalDistributorID;
            _isActive = isActive;
            _updatedDate = updatedDate;
            _createdDate = createdDate;
            _updatedBy = updateBy;
            _CMDBusinessUnitID = CMDbusinessUnitID;
        }


        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var practiceAndAccount = new CMDAccount
              {

                  Name = _name,
                  CMDPracticeID = _cmdPracticeID,
                  FedTaxID = _fexTaxID,
                  CMDAccountTypeID = Convert.ToInt32(_cmdAccountTypeID),
                  NetSuiteID = _netSuiteID,
                  OriginalBusinessUnitID = _originalBusinessUnitID,
                  OriginalDistributorID = _originalDistributorID,
                  IsActive = _isActive,
                  UpdatedDate = _updatedDate,
                  CreatedDate = _createdDate,
                  UpdatedBy = _updatedBy,
                  CMDBusinessUnitId = _CMDBusinessUnitID
              };

            return context.Create(practiceAndAccount, false) as T;
        }
    }
}