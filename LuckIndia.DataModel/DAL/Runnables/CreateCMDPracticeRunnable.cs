using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDPracticeRunnable : IRunnable
    {
        private readonly string _practiceName;
        private readonly int? _primaryOwnerID;
        private readonly int? _cmdLegalJurisdictionID;
        private readonly int? _originalBusinessUnitID;
        private readonly bool _isActive;
        private readonly string _phone;
        private readonly string _email;
        private readonly DateTime _createdDate;
        private readonly DateTime _updatedDate;
        private readonly string _updatedBy;
        private readonly bool _isDuplicate;

        public CreateCMDPracticeRunnable(string practiceName, int? primaryOwnerID, int? cmdLegalJurisdictionID, int? originalBusinessUnitID, bool isActive, string Phone, string Email, DateTime updatedDate, DateTime createdDate, string updateBy, bool isDuplicate)
        {
            _practiceName = practiceName;
            _primaryOwnerID = primaryOwnerID;
            _cmdLegalJurisdictionID = cmdLegalJurisdictionID;
            _originalBusinessUnitID = originalBusinessUnitID;
            _isActive = isActive;
            _phone = Phone;
            _email = Email;
            _isDuplicate = isDuplicate;
            _updatedDate = updatedDate;
            _createdDate = createdDate;
            _updatedBy = updateBy;
        }
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var practice = new CMDPractice
            {
                PracticeName = _practiceName,
                PrimaryOwnerID = _primaryOwnerID,
                CMDLegalJurisdictionID = _cmdLegalJurisdictionID,
                OriginalBusinessUnitID = _originalBusinessUnitID,
                IsActive = _isActive,
                Email = _email,
                Phone = _phone,
                IsDuplicate = _isDuplicate,
                UpdatedDate = _updatedDate,
                CreatedDate = _createdDate,
                UpdatedBy = _updatedBy
            };

            return context.Create(practice, false) as T;
        }
    }
}