using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;
namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDContactRunnable : IRunnable
    {

        private readonly int? _id;
        private readonly string _address1;
        private readonly string _address2;
        private readonly string _city;
        private readonly int? _cmdStateId;
        private readonly string _zipCode;
        private readonly string _publicEmail;
        private readonly string _privateEmail;
        private readonly string _phone;
        private readonly string _fax;
        private readonly string _cell;
        private readonly string _contactName;
        private readonly bool _isDefault;
        private readonly string _websiteURL;
        private readonly string _title;
        private readonly DateTime _updatedDate;
        private readonly DateTime _createdDate;
        private readonly bool _isActive;
        private readonly string _updatedBy;
        private readonly string _StreetNumber;
        public CreateCMDContactRunnable(int? id,
                                                            string address1,
                                                            string address2,
                                                            string city,
                                                            int? cmdStateId,
                                                            string zipCode,
                                                            string publicEmail,
                                                            string privateEmail,
                                                            string phone,
                                                            string fax,
                                                            string cell,
                                                            string contactName,
                                                            bool isDefault,
                                                            string websiteURL,
                                                            DateTime updatedDate,
                                                            DateTime createdDate,
                                                            bool isActive, string title, string updatedBy, string StreetNumber)
        {

            _id = Convert.ToInt32(id);
            _address1 = address1;
            _address2 = address2;
            _city = city;
            _cmdStateId = cmdStateId;
            _zipCode = zipCode;
            _privateEmail = privateEmail;
            _publicEmail = publicEmail;
            _phone = phone;
            _fax = fax;
            _cell = cell;
            _contactName = contactName;
            _isDefault = isDefault;
            _websiteURL = websiteURL;
            _updatedDate = updatedDate;
            _createdDate = createdDate;
            _isActive = isActive;
            _title = title;
            _updatedBy = updatedBy;
            _StreetNumber = StreetNumber;
        }

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var cmdContact = new CMDContact()
            {
                Id = Convert.ToInt32(_id),
                Address1 = _address1.RemoveQuotes(),
                Address2 = _address2.RemoveQuotes(),
                City = _city.RemoveQuotes(),
                CMDStateID = _cmdStateId,
                ZipCode = _zipCode.RemoveQuotes(),
                PrivateEmail = _privateEmail.RemoveQuotes(),
                PublicEmail = _publicEmail.RemoveQuotes(),
                Phone = _phone.RemoveQuotes(),
                Fax = _fax.RemoveQuotes(),
                Cell = _cell.RemoveQuotes(),
                ContactName = _contactName.RemoveQuotes(),
                IsDefault = _isDefault,
                WebsiteURL = _websiteURL.RemoveQuotes(),
                UpdatedDate = _updatedDate,
                CreatedDate = _createdDate,
                IsActive = _isActive,
                Title = _title.RemoveQuotes(),
                UpdatedBy = _updatedBy.RemoveQuotes(),
                StreetNumber = _StreetNumber
            };
            return context.Create(cmdContact, false) as T;
        }
    }
}