using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCustomerRunnable : IRunnable
    {
        private readonly string _firstName;
        private readonly string _middleName;
        private readonly string _lastName;
        private readonly DateTime? _dateOfBirth;
        private readonly string _defaultLanguage;
        private readonly string _email;
        private readonly string _phone;
        private readonly string _gender;
        private readonly bool? _isActive;
        private readonly bool _isDuplicate;
        private readonly string _nPI;
        private readonly int _businessUnit;
        private readonly int? _cmdCustomerTypeID;
        private readonly string _status;


        public CreateCustomerRunnable(string FirstName, string MiddleName, string LastName, DateTime? DateOfBirth, string DefaultLanguage, string Email, string Phone, string Gender, bool? IsActive, string NPI,
                    int BusinessUnit, int? CMDCustomerTypeID, bool IsDuplicate, string status)
        {
            _firstName = FirstName;
            _middleName = MiddleName;
            _lastName = LastName;
            _dateOfBirth = DateOfBirth;
            _defaultLanguage = DefaultLanguage;
            _email = Email;
            _phone = Phone;
            _gender = Gender;
            _isActive = IsActive;
            _nPI = NPI;
            _businessUnit = BusinessUnit;
            _cmdCustomerTypeID = CMDCustomerTypeID;
            _isDuplicate = IsDuplicate;
            _status = status;
        }


        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var cmdCustomer = new CMDCustomer
            {
                FirstName = _firstName.RemoveQuotes(),
                MiddleName = _middleName.RemoveQuotes(),
                LastName = _lastName.RemoveQuotes(),
                DOB = ALPHAEON.CMD.Common.CommonUtility.GetDate(_dateOfBirth),
                DefaultLanguage = _defaultLanguage.RemoveQuotes(),
                CMDCustomerTypeID = Convert.ToInt32(_cmdCustomerTypeID),
                Email = _email.RemoveQuotes(),
                Phone = _phone,
                Gender = _gender.RemoveQuotes(),
                IsActive = true,
                IsDuplicate = _isDuplicate,
                NPI = _nPI.RemoveQuotes(),

                OriginalBusinessUnitID = _businessUnit,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = ALPHAEON.CMD.Common.Constants.Configuration.UpdatedBy.RemoveQuotes(),
                UpdatedDate = DateTime.UtcNow,
                Status = _status
            };

            return context.Create(cmdCustomer, false) as T;
        }
    }
}