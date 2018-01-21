using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetExistingCMDPracticeListforDuplicatesRunnable : BaseApiController, IRunnable
    {
        #region Public Methods
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            context.Configuration.LazyLoadingEnabled = false;

            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            List<CMDDashboardPractice> existingPracticeListResult = null;

            try
            {
                IQueryable<CMDDashboardPractice> cmdDashboardExistingPracticeList = CreateSelectQuery(context);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardExistingPracticeList = cmdDashboardExistingPracticeList.Where(whereCondition);
                }

                //// Order By Condition
                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                cmdDashboardExistingPracticeList = cmdDashboardExistingPracticeList.OrderBy(orderByCondition).Skip(skip).Take(take);

                existingPracticeListResult = cmdDashboardExistingPracticeList.ToList();

                if (existingPracticeListResult != null)
                {
                    existingPracticeListResult =
                           cmdDashboardExistingPracticeList.ToList()
                           .GroupBy(existingPractice => existingPractice.Id)
                           .Select(recordPractice => recordPractice.First()).ToList();
                }

                CMDLogger.LogAudit("Obtained the Duplicate Practices List by Practice ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return existingPracticeListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods        
        /// <summary>
        /// Get the list of practices by duplicate customers
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>list of practices</returns>
        private static IQueryable<CMDDashboardPractice> CreateSelectQuery(CMDDatabaseContext context)
        {
            return context.CMDPracticeDuplicateMap
                                            .Where(cmdPracticeDuplicateMap => cmdPracticeDuplicateMap.IsActive && cmdPracticeDuplicateMap.ResolveAction == null)
                                            .Join(
                                            context.CMDPractices,
                                            recordPracticeDuplicateMap => recordPracticeDuplicateMap.ExistingPracticeID,
                                            recordCMDPractice => recordCMDPractice.Id,
                                            (recordPracticeDuplicateMap, recordCMDPractice) => recordCMDPractice)
                                            .Select(recordPractice => new CMDDashboardPractice()
                                            {
                                                Id = recordPractice.Id,
                                                PracticeName = recordPractice.PracticeName,
                                                Phone = recordPractice.Phone,
                                                Email = recordPractice.Email,
                                                PrimaryOwnerID = recordPractice.PrimaryOwnerID,
                                                PrimaryOwnerFirstName = (from cmdCustomer in context.CMDCustomers
                                                                         where cmdCustomer.Id == recordPractice.PrimaryOwnerID && recordPractice.PrimaryOwnerID != null && cmdCustomer.IsActive
                                                                         select cmdCustomer.FirstName).FirstOrDefault(),
                                                PrimaryOwnerLastName = (from cmdCustomer in context.CMDCustomers
                                                                        where cmdCustomer.Id == recordPractice.PrimaryOwnerID && recordPractice.PrimaryOwnerID != null && cmdCustomer.IsActive
                                                                        select cmdCustomer.LastName).FirstOrDefault(),
                                                IsActive = recordPractice.IsActive,

                                                //// List of speciality for practice
                                                CMDSpecialityList = (from cmdCustomer in
                                                                         (from practice in context.CMDPractices
                                                                          where practice.IsActive
                                                                          join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                                                          on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                                                          where cmdCustomerPracticeMap.CMDPracticeID != null && practice.Id == recordPractice.Id && cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDCustomer.IsActive
                                                                          select cmdCustomerPracticeMap.CMDCustomer)
                                                                     join cmdCustomerSpecialityMap in context.CMDCustomerSpecialityMaps
                                                                     on cmdCustomer.Id equals cmdCustomerSpecialityMap.CMDCustomerID
                                                                     where cmdCustomerSpecialityMap.CMDSpeciality.IsActive &&
                                                                     cmdCustomerSpecialityMap.IsActive
                                                                     select new CMDDashboardSpeciality
                                                                     {
                                                                         Id = cmdCustomerSpecialityMap.CMDSpeciality.Id,
                                                                         IsActive = cmdCustomerSpecialityMap.CMDSpeciality.IsActive,
                                                                         Name = cmdCustomerSpecialityMap.CMDSpeciality.Name,
                                                                         ParentSpecialityID = cmdCustomerSpecialityMap.CMDSpeciality.ParentSpecialityID
                                                                     }).Distinct<CMDDashboardSpeciality>(),

                                                CMDBusinessUnitIdsList = context.CMDAccounts
                                                                        .Where(cmdAccount => cmdAccount.CMDPracticeID == recordPractice.Id && cmdAccount.IsActive && cmdAccount.CMDBusinessUnit.IsActive)
                                                                        .Select(recordAccount => recordAccount.CMDBusinessUnitId).Distinct(),

                                                //// List of products for practice
                                                CMDPracticeProductList = context.CMDPractices.Where(cmdPractice => cmdPractice.Id == recordPractice.Id && cmdPractice.IsActive)
                                                                   .SelectMany(cmdPractice => cmdPractice.CMDAccounts)
                                                                   .Where(cmdAccount => cmdAccount.IsActive)
                                                                   .SelectMany(cmdAccount => cmdAccount.CMDTransactions)
                                                                   .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                   .Join(
                                                                   context.CMDTransactionProductMaps,
                                                                   r => r.Id,
                                                                   cmdTransactionProductMap =>
                                                                       cmdTransactionProductMap.CMDTransactionID,
                                                                       (r, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                                   .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive &&
                                                                   cmdTransactionProductMap.CMDProduct.IsActive)
                                                                   .Select(cmdTransactionProductMap => new CMDDashboardProduct
                                                                   {
                                                                       Id = cmdTransactionProductMap.CMDProduct.Id,
                                                                       CMDBusinessUnitID = cmdTransactionProductMap.CMDProduct.CMDBusinessUnitID,
                                                                       SKU = cmdTransactionProductMap.CMDProduct.SKU,
                                                                       CMDProductTypeID = cmdTransactionProductMap.CMDProduct.CMDProductTypeID,
                                                                       Name = cmdTransactionProductMap.CMDProduct.Name,
                                                                       IsActive = cmdTransactionProductMap.CMDProduct.IsActive,
                                                                       CMDBusinessUnit = cmdTransactionProductMap.CMDProduct.CMDBusinessUnit,
                                                                       CMDProductType = cmdTransactionProductMap.CMDProduct.CMDProductType,
                                                                       CMDContactList =
                                                                      context.CMDAccounts
                                                                      .Where(account => account.Id == cmdTransactionProductMap.CMDTransaction.CMDAccountID)
                                                                      .SelectMany(acc => acc.CMDAccountContactMaps)
                                                                      .Join(
                                                                      context.CMDContacts,
                                                                      cmdAccountTransactionMap => cmdAccountTransactionMap.CMDContactID,
                                                                      recContact => recContact.Id,
                                                                      (cmdAccountTransactionMap, recContact) => recContact)
                                                                   }),

                                                //// List of transaction for practice
                                                CMDPracticeTransactionList = context.CMDAccounts
                                                                            .Where(cmdAccount => cmdAccount.CMDPracticeID == recordPractice.Id &&
                                                                                cmdAccount.IsActive)
                                                                            .SelectMany(account => account.CMDTransactions)
                                                                            .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                            .Select(transaction => new CMDDashboardTransaction()
                                                                            {
                                                                                Id = transaction.Id,

                                                                                CMDUserID = transaction.CMDUserID,

                                                                                TxDate = transaction.TxDate,

                                                                                CMDTransactionTypeID = transaction.CMDTransactionTypeID,

                                                                                Note = transaction.Note,

                                                                                TxCompletedDate = transaction.TxCompletedDate,

                                                                                CMDAccountID = transaction.CMDAccountID,

                                                                                City = transaction.City,

                                                                                CMDStateID = transaction.CMDStateID,

                                                                                IsActive = transaction.IsActive,

                                                                                CMDAccount = transaction.CMDAccount,

                                                                                CMDState = transaction.CMDState,

                                                                                CMDTransactionType = transaction.CMDTransactionType,

                                                                                CMDUser = transaction.CMDUser,

                                                                                TransactionContactList = context.CMDTransactionContactMaps
                                                                            .Where(cmdTransactionContactMap =>
                                                                                cmdTransactionContactMap.IsActive &&
                                                                                cmdTransactionContactMap.CMDTransactionID == transaction.Id &&
                                                                                cmdTransactionContactMap.CMDContact.IsActive)
                                                                            .Select(cmdTransactionContactMap => new CMDDashboardTransactionContact()
                                                                            {
                                                                                CMDContact = cmdTransactionContactMap.CMDContact,
                                                                                ContactTypeID = cmdTransactionContactMap.ContactType
                                                                            }),
                                                                            }),

                                                //// List of physician for practice
                                                CMDPhysicianList = from practice in context.CMDPractices
                                                                   where practice.IsActive
                                                                   join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                                                   on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                                                   where practice.Id == recordPractice.Id && cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDCustomer.CMDCustomerType ==
                                                       (from cmdCustomer in context.CMDCustomers
                                                        where cmdCustomer.CMDCustomerType.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDCustomerTypeNamePhysician && cmdCustomer.IsActive
                                                        select cmdCustomer.CMDCustomerType).FirstOrDefault()
                                                                   select new CMDDashboardCustomer
                                                                   {
                                                                       CustomerID = cmdCustomerPracticeMap.CMDCustomer.Id,
                                                                       CustomerTypeID = cmdCustomerPracticeMap.CMDCustomer.CMDCustomerTypeID,
                                                                       DOB = cmdCustomerPracticeMap.CMDCustomer.DOB,
                                                                       Email = cmdCustomerPracticeMap.CMDCustomer.Email,
                                                                       FirstName = cmdCustomerPracticeMap.CMDCustomer.FirstName,
                                                                       LastName = cmdCustomerPracticeMap.CMDCustomer.LastName,
                                                                       IsActive = cmdCustomerPracticeMap.CMDCustomer.IsActive,
                                                                       CustomerTypeName = cmdCustomerPracticeMap.CMDCustomer.CMDCustomerType.Name
                                                                   },

                                                //// List of contact for practice
                                                CMDContactList = from cmdContact in context.CMDContacts
                                                                 where cmdContact.IsActive
                                                                 join cmdPracticeContactMap in context.CMDPracticeContactMaps
                                                                    on cmdContact.Id equals cmdPracticeContactMap.CMDContactID
                                                                 where cmdPracticeContactMap.CMDPracticeID == recordPractice.Id && cmdPracticeContactMap.IsActive
                                                                && cmdPracticeContactMap.CMDPractice.IsActive
                                                                 select new CMDDashboardContact
                                                                 {
                                                                     Address1 = cmdContact.Address1,
                                                                     Address2 = cmdContact.Address2,
                                                                     City = cmdContact.City,
                                                                     Cell = cmdContact.Cell,
                                                                     CMDStateID = cmdContact.CMDState.Id,
                                                                     CMDState = cmdContact.CMDState.Name,
                                                                     Country = cmdContact.CMDState.CMDCountry.Name,
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
                                            });
        }

        #endregion Private Methods
    }
}
