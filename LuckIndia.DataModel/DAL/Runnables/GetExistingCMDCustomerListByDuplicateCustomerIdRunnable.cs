using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetExistingCMDCustomerListByDuplicateCustomerIdRunnable : BaseApiController, IRunnable
    {
        #region Public Methods
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string id = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            List<CMDDashboardCustomer> existingCustomerListResult = null;
            context.Configuration.LazyLoadingEnabled = false;

            try
            {
                int customerId = Convert.ToInt32(id);

                IQueryable<CMDDashboardCustomer> cmdDashboardExistingCustomerList =
                                                context.CMDCustomers
                                                .Where(cmdCust => cmdCust.Id == customerId && cmdCust.IsActive && cmdCust.IsDuplicate)
                                                .Join(
                                                context.CMDCustomerDuplicateMap,
                                                recCustomer => recCustomer.Id,
                                                cmdCustomerDuplicateMap => cmdCustomerDuplicateMap.DuplicateCustomerID,
                                                (recCustomer, cmdCustomerDuplicateMap) => cmdCustomerDuplicateMap)
                                                .Where(recCMDCustomerDuplicateMap => recCMDCustomerDuplicateMap.IsActive && recCMDCustomerDuplicateMap.ResolveAction == null)
                                                .Select(recCustomerDuplicateMap => recCustomerDuplicateMap)
                                                .Join(
                                                context.CMDCustomers,
                                                recordDuplicateCustomer => recordDuplicateCustomer.ExistingCustomerID,
                                                recordCustomer => recordCustomer.Id,
                                                (recordDuplicateCustomer, recordCustomer) => new CMDDashboardCustomer()
                                                {
                                                    CustomerID = recordCustomer.Id,
                                                    FirstName = recordCustomer.FirstName,
                                                    MiddleName = recordCustomer.MiddleName,
                                                    LastName = recordCustomer.LastName,
                                                    Email = recordCustomer.Email,
                                                    Phone = recordCustomer.Phone,
                                                    DOB = recordCustomer.DOB,
                                                    IsActive = recordCustomer.IsActive,
                                                    CustomerTypeID = recordCustomer.CMDCustomerTypeID,
                                                    CustomerTypeName = recordCustomer.CMDCustomerType.Name,
                                                    OriginalBusinessUnitID = recordCustomer.OriginalBusinessUnitID,
                                                    OriginalBusinessUnitName = context.CMDBusinessUnits
                                                                                .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == recordCustomer.OriginalBusinessUnitID)
                                                                                .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

                                                    // Get the list of emails assigned to customer
                                                    CMDCustomerEmailList = context.CMDCustomerEmails
                                                                            .Where(cmdCustomerEmail => cmdCustomerEmail.IsActive && cmdCustomerEmail.CMDCustomerID == recordCustomer.Id)
                                                                            .Select(cmdCustEmailList => cmdCustEmailList).ToList(),

                                                    // Get the list of contacts assigned to customer
                                                    CMDContactList = from cmdContact in context.CMDContacts
                                                                     where cmdContact.IsActive
                                                                     join cmdCustomerContactMap in context.CMDCustomerContactMaps
                                                                     on cmdContact.Id equals cmdCustomerContactMap.CMDContactID
                                                                     where cmdCustomerContactMap.CMDCustomerID == recordCustomer.Id && cmdCustomerContactMap.IsActive
                                                                     select new CMDDashboardContact
                                                                     {
                                                                         Address1 = cmdContact.Address1,
                                                                         Address2 = cmdContact.Address2,
                                                                         City = cmdContact.City,
                                                                         Cell = cmdContact.Cell,
                                                                         CMDStateID = cmdContact.CMDState.Id,
                                                                         CMDStateAbbreviation = cmdContact.CMDState.Abbriviation,
                                                                         CMDCountryAbbreviation = cmdContact.CMDState.CMDCountry.Abbreviation,
                                                                         ContactName = cmdContact.ContactName,
                                                                         CreatedDate = cmdContact.CreatedDate,
                                                                         Fax = cmdContact.Fax,
                                                                         Id = cmdContact.Id,
                                                                         IsDefault = cmdContact.IsDefault,
                                                                         Phone = cmdContact.Phone,
                                                                         PrivateEmail = cmdContact.PrivateEmail,
                                                                         PublicEmail = cmdContact.PublicEmail,
                                                                         UpdatedDate = cmdContact.UpdatedDate,
                                                                         WebsiteURL = cmdContact.WebsiteURL,
                                                                         ZipCode = cmdContact.ZipCode,
                                                                         CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                                                         CMDStateName = cmdContact.CMDState.Name
                                                                     },

                                                    // Get the list of speciality assigned to customer
                                                    CMDSpecialityList = from cmdSpeciality in context.CMDSpecialities
                                                                        where cmdSpeciality.IsActive
                                                                        join cmdSpecialityMap in context.CMDCustomerSpecialityMaps
                                                                        on cmdSpeciality.Id equals cmdSpecialityMap.CMDSpecialityID
                                                                        where cmdSpecialityMap.CMDCustomerID == recordCustomer.Id && cmdSpecialityMap.IsActive
                                                                        select new CMDDashboardSpeciality
                                                                        {
                                                                            CreatedDate = cmdSpeciality.CreatedDate,
                                                                            Id = cmdSpeciality.Id,
                                                                            IsActive = cmdSpeciality.IsActive,
                                                                            Name = cmdSpeciality.Name,
                                                                            UpdatedBy = cmdSpeciality.UpdatedBy,
                                                                            UpdatedDate = cmdSpeciality.UpdatedDate
                                                                        },

                                                    // Get the list of practices assigned to customer
                                                    CMDPracticeList = context.CMDCustomerPracticeMaps.Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == recordCustomer.Id &&
                                                                        cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDPractice.IsActive).Select(recordCMDCustomerPracticeMap => recordCMDCustomerPracticeMap.CMDPractice),

                                                    // Get the list of products assigned to customer
                                                    CMDProductList = context.CMDCustomers
                                                                     .Where(cmdCustomerRecord => cmdCustomerRecord.Id == recordCustomer.Id && cmdCustomerRecord.IsActive)
                                                                     .Join(context.CMDCustomerAccountMaps, recCustomer => recCustomer.Id,
                                                                     cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                                                                     (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                                                                     .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                                                         cmdCustomerAccountMap.CMDAccount.IsActive)
                                                                     .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                                                                     .SelectMany(transact => transact)
                                                                     .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                     .Join(context.CMDTransactionProductMaps, recTransaction => recTransaction.Id,
                                                                     cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                                     (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                                     .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                                                                     .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                                                                     .Where(cmdProduct => cmdProduct.IsActive).Distinct(),

                                                    Criteria = recordDuplicateCustomer.Criteria,
                                                    CreatedDate = recordCustomer.CreatedDate
                                                });

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardExistingCustomerList = cmdDashboardExistingCustomerList.Where(whereCondition);
                }

                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "CustomerId";
                cmdDashboardExistingCustomerList = cmdDashboardExistingCustomerList.OrderBy(orderByCondition).Skip(skip).Take(take);

                existingCustomerListResult = cmdDashboardExistingCustomerList.ToList();

                CMDLogger.LogAudit("Obtained the Duplicate Customers List by Customer ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return existingCustomerListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods        
    }
}
