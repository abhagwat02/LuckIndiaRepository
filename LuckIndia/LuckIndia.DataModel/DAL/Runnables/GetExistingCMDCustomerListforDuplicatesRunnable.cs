using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetExistingCMDCustomerListforDuplicatesRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            context.Configuration.LazyLoadingEnabled = false;

            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            List<CMDDashboardCustomer> existingCustomerListResult = null;

            try
            {
                IQueryable<CMDDashboardCustomer> cmdDashboardExistingCustomerList = CreateSelectQuery(context);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardExistingCustomerList = cmdDashboardExistingCustomerList.Where(whereCondition);
                }

                existingCustomerListResult = cmdDashboardExistingCustomerList.ToList();

                if (existingCustomerListResult != null)
                {
                    existingCustomerListResult =
                           cmdDashboardExistingCustomerList.ToList()
                           .GroupBy(existingCustomer => existingCustomer.CustomerID)
                           .Select(recordCustomer => recordCustomer.First()).ToList();

                    var countCustomer = existingCustomerListResult.Count();

                    #region Order By Condition
                    orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "CustomerID";
                    cmdDashboardExistingCustomerList = cmdDashboardExistingCustomerList.OrderBy(orderByCondition).Skip(skip).Take(take);
                    #endregion

                    existingCustomerListResult =
                           cmdDashboardExistingCustomerList.ToList()
                           .GroupBy(existingCustomer => existingCustomer.CustomerID)
                           .Select(recordCustomer => recordCustomer.First()).ToList();

                    existingCustomerListResult.All(m => { m.CustomersCount = countCustomer; return true; });
                }

                CMDLogger.LogAudit("Obtained the Customers List having Duplicate Customers record in CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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

        private static IQueryable<CMDDashboardCustomer> CreateSelectQuery(CMDDatabaseContext context)
        {
            return context.CMDCustomerDuplicateMap
                                        .Where(cmdCustomerDuplicateMap => cmdCustomerDuplicateMap.IsActive && cmdCustomerDuplicateMap.ResolveAction == null)
                                        .Join(
                                        context.CMDCustomers,
                                        recordCustomerDuplicateMap => recordCustomerDuplicateMap.ExistingCustomerID,
                                        recordCMDCustomer => recordCMDCustomer.Id,
                                        (recordCustomerDuplicateMap, recordCustomer) => new CMDDashboardCustomer()
                                        {
                                            CustomerID = recordCustomer.Id,
                                            FirstName = recordCustomer.FirstName,
                                            MiddleName = recordCustomer.MiddleName,
                                            LastName = recordCustomer.LastName,
                                            Email = recordCustomer.Email,
                                            DOB = recordCustomer.DOB,
                                            IsActive = recordCustomer.IsActive,
                                            CustomerTypeID = recordCustomer.CMDCustomerTypeID,
                                            CustomerTypeName = recordCustomer.CMDCustomerType.Name,
                                            OriginalBusinessUnitID = recordCustomer.OriginalBusinessUnitID,
                                            OriginalBusinessUnitName = context.CMDBusinessUnits
                                            .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == recordCustomer.OriginalBusinessUnitID)
                                            .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),
                                            CMDContactList =
                                                                from cmdContact in context.CMDContacts
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
                                                                    Fax = cmdContact.Fax,
                                                                    Id = cmdContact.Id,
                                                                    IsDefault = cmdContact.IsDefault,
                                                                    Phone = cmdContact.Phone,
                                                                    PrivateEmail = cmdContact.PrivateEmail,
                                                                    PublicEmail = cmdContact.PublicEmail,
                                                                    WebsiteURL = cmdContact.WebsiteURL,
                                                                    ZipCode = cmdContact.ZipCode,
                                                                    CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                                                    CMDStateName = cmdContact.CMDState.Name
                                                                },

                                            CMDSpecialityList =
                                                                from cmdSpeciality in context.CMDSpecialities
                                                                where cmdSpeciality.IsActive
                                                                join cmdSpecialityMap in context.CMDCustomerSpecialityMaps
                                                                    on cmdSpeciality.Id equals cmdSpecialityMap.CMDSpecialityID
                                                                where cmdSpecialityMap.CMDCustomerID == recordCustomer.Id && cmdSpecialityMap.IsActive
                                                                select new CMDDashboardSpeciality
                                                                {
                                                                    Id = cmdSpeciality.Id,
                                                                    IsActive = cmdSpeciality.IsActive,
                                                                    Name = cmdSpeciality.Name
                                                                },
                                            PracticesCount = context.CMDCustomerPracticeMaps.Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == recordCustomer.Id && cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDPractice.IsActive).Count(),
                                            ProductsCount = context.CMDCustomers
                                                                .Where(cmdCustomerRecord => cmdCustomerRecord.Id == recordCustomer.Id && cmdCustomerRecord.IsActive)
                                                                    .Join(
                                                                    context.CMDCustomerAccountMaps,
                                                                    recCustomer => recCustomer.Id,
                                                                    cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                                                                    (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                                                                .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                                                    cmdCustomerAccountMap.CMDAccount.IsActive)
                                                                .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                                                                .SelectMany(transact => transact)
                                                                .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                .Join(
                                                                context.CMDTransactionProductMaps,
                                                                recTransaction => recTransaction.Id,
                                                                cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                                (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                                .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                                                                .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                                                                .Where(cmdProduct => cmdProduct.IsActive).Distinct().Count(),

                                            DuplicatesCount = context.CMDCustomers
                                                                .Where(cmdCust => cmdCust.Id == recordCustomer.Id && cmdCust.IsActive)
                                                                .Join(
                                                                context.CMDCustomerDuplicateMap,
                                                                recCustomer => recCustomer.Id,
                                                                cmdCustomerDuplicateMap => cmdCustomerDuplicateMap.ExistingCustomerID,
                                                                (recCustomer, cmdCustomerDuplicateMap) => cmdCustomerDuplicateMap)
                                                                .Where(recCMDCustomerDuplicateMap => recCMDCustomerDuplicateMap.IsActive && recCMDCustomerDuplicateMap.ResolveAction == null)
                                                                .Select(recCustomerDuplicateMap => recCustomerDuplicateMap)
                                                                .Join(
                                                                context.CMDCustomers,
                                                                recordDuplicateCustomer => recordDuplicateCustomer.DuplicateCustomerID,
                                                                recordCMDCustomer => recordCMDCustomer.Id,
                                                                (recordDuplicateCustomer, recordCMDCustomer) => recordCMDCustomer).Where(customerRecord => customerRecord.IsActive).Count(),

                                            CMDPracticeList = context.CMDCustomerPracticeMaps
                                                            .Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == recordCustomer.Id &&
                                                                cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDPractice.IsActive)
                                                                .Select(recordCMDCustomerPracticeMap => recordCMDCustomerPracticeMap.CMDPractice),
                                            CMDProductList = context.CMDCustomers
.Where(cmdCustomerRecord => cmdCustomerRecord.Id == recordCustomer.Id && cmdCustomerRecord.IsActive)
                                                                .Join(
                                                                context.CMDCustomerAccountMaps,
                                                                recCustomer => recCustomer.Id,
                                                                cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                                                                (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                                                                .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                                                    cmdCustomerAccountMap.CMDAccount.IsActive)
                                                                .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                                                                .SelectMany(transact => transact)
                                                                .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                .Join(
                                                                context.CMDTransactionProductMaps,
                                                                recTransaction => recTransaction.Id,
                                                                cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                                (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                                .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                                                                .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                                                                .Where(cmdProduct => cmdProduct.IsActive).Distinct(),

                                            CreatedDate = recordCustomer.CreatedDate,

                                            Criteria = recordCustomerDuplicateMap.Criteria,
                                        });
        }
    }
}
