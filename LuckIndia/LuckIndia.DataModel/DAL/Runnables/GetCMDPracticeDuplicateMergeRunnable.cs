using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{    
    internal sealed class GetCMDPracticeDuplicateMergeRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            //// Check if search string contains ;amp; , then replace with &  and ;tmp;" then  replace with "#" 
            whereCondition = whereCondition.ToggleAmpersand();

            context.Configuration.LazyLoadingEnabled = false;

            List<CMDDashboardPractice> practiceDuplicateMergeDetailsResult = new List<CMDDashboardPractice>();

            IQueryable<CMDDashboardPractice> practiceDuplicateMergeDetails = null;
            try
            {
                practiceDuplicateMergeDetails = CreateSelectQuery(context);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        practiceDuplicateMergeDetails = practiceDuplicateMergeDetails.Where(whereCondition);
                    }
                }

                var countPractice = practiceDuplicateMergeDetails.Count();

                if (countPractice > 0)
                {
                    //// If value of take passed as 0 then fetch all the practices 
                    if (take == 0)
                    {
                        take = countPractice;
                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        practiceDuplicateMergeDetails = practiceDuplicateMergeDetails.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {
                        practiceDuplicateMergeDetails = practiceDuplicateMergeDetails.OrderBy("Id").Skip(skip).Take(take);
                    }

                    practiceDuplicateMergeDetailsResult = practiceDuplicateMergeDetails.ToList();
                }

                if (practiceDuplicateMergeDetailsResult != null)
                {
                    practiceDuplicateMergeDetailsResult.All(m => { m.PracticeCount = countPractice; return true; });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return practiceDuplicateMergeDetailsResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// This method create a query to fetch the list of practices
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>query to fetch the list of practices</returns>
        private static IQueryable<CMDDashboardPractice> CreateSelectQuery(CMDDatabaseContext context)
        {
            return from cmdPractice in context.CMDPractices
                   select new CMDDashboardPractice()
                   {
                       Id = cmdPractice.Id,
                       Phone = cmdPractice.Phone,
                       Email = cmdPractice.Email,
                       PracticeName = cmdPractice.PracticeName,
                       PrimaryOwnerID = cmdPractice.PrimaryOwnerID,
                       IsActive = cmdPractice.IsActive,
                       OriginalBusinessUnitID = cmdPractice.OriginalBusinessUnitID,

                       //// Fetch the business unit name
                       OriginalBusinessUnitName = context.CMDBusinessUnits
                                               .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == cmdPractice.OriginalBusinessUnitID)
                                               .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

                       PrimaryOwnerFirstName = context.CMDCustomers
                                               .Where(cmdCustomer => cmdCustomer.Id == cmdPractice.PrimaryOwnerID && cmdPractice.PrimaryOwnerID != null && cmdCustomer.IsActive)
                                               .Select(cmdCustomer => cmdCustomer.FirstName + " " + cmdCustomer.LastName).FirstOrDefault(),

                       //// Fetch the product and contact list for practice
                       CMDPracticeProductAndContactList = context.CMDPractices
                                                          .Where(cmdPracticeRecord => cmdPracticeRecord.Id == cmdPractice.Id &&
                                                           cmdPracticeRecord.IsActive)
                                                           .Select(recordCmdPractice => new CMDDashboardProductAndContact()
                                                           {
                                                               //// Product list
                                                               cmdDashboardProductList =
                                                               context.CMDPractices
                                                              .Where(cmdPracticeRecord => cmdPracticeRecord.Id == cmdPractice.Id &&
                                                               cmdPracticeRecord.IsActive)
                                                              .SelectMany(cmdPracticeRecord => cmdPracticeRecord.CMDAccounts)
                                                              .Where(cmdAccount => cmdAccount.IsActive)
                                                              .SelectMany(j => j.CMDTransactions)
                                                              .Where(cmdTransaction => cmdTransaction.IsActive)
                                                              .Join(context.CMDTransactionProductMaps, r => r.Id,
                                                              cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                              (r, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                              .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive &&
                                                              cmdTransactionProductMap.CMDProduct.IsActive)
                                                              .Select(cmdTransactionProductMap => new CMDDashboardProduct()
                                                              {
                                                                  Id = cmdTransactionProductMap.CMDProduct.Id,
                                                                  CMDBusinessUnitID = cmdTransactionProductMap.CMDProduct.CMDBusinessUnitID,
                                                                  SKU = cmdTransactionProductMap.CMDProduct.SKU,
                                                                  CMDProductTypeID = cmdTransactionProductMap.CMDProduct.CMDProductTypeID,
                                                                  Name = cmdTransactionProductMap.CMDProduct.Name,
                                                                  IsActive = cmdTransactionProductMap.CMDProduct.IsActive,
                                                                  CMDBusinessUnit = cmdTransactionProductMap.CMDProduct.CMDBusinessUnit,
                                                                  CMDProductType = cmdTransactionProductMap.CMDProduct.CMDProductType,
                                                              }).Distinct(),

                                                               //// Contact List
                                                               cmdDashboardProductContactsList =
                                                                context.CMDPractices
                                                                .Where(cmdPracticeRecord => cmdPracticeRecord.Id == cmdPractice.Id &&
                                                                 cmdPracticeRecord.IsActive)
                                                                .SelectMany(cmdPracticeRecord => cmdPracticeRecord.CMDAccounts)
                                                                .Where(cmdAccount => cmdAccount.IsActive)
                                                                .SelectMany(acc => acc.CMDAccountContactMaps)
                                                                .Where(cmdAccountContactMap => cmdAccountContactMap.IsActive &&
                                                                       cmdAccountContactMap.CMDContact.IsActive)
                                                                .Join(context.CMDContacts, cmdAccountTransactionMap => cmdAccountTransactionMap.CMDContactID,
                                                                   recContact => recContact.Id, (cmdAccountTransactionMap, recContact) => recContact).Distinct()
                                                           }),

                       CMDContactList = from cmdContact in context.CMDContacts
                                        where cmdContact.IsActive
                                        join cmdPracticeContactMap in context.CMDPracticeContactMaps
                                        on cmdContact.Id equals cmdPracticeContactMap.CMDContactID
                                        where cmdPracticeContactMap.CMDPracticeID == cmdPractice.Id && cmdPracticeContactMap.IsActive
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

                       IsDuplicate = cmdPractice.IsDuplicate,

                       ////If the selected practice is duplicate practice then, find the matching percent(%) with existing one
                       DuplicateMatchPercent = context.CMDPracticeDuplicateMap.FirstOrDefault(recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.DuplicatePracticeID == cmdPractice.Id && recordCMDPracticeDuplicateMap.IsActive && recordCMDPracticeDuplicateMap.ResolveAction == null).DuplicateMatchPercent,

                       ////If the selected practice is duplicate practice then, give count of Existing Practices for which the selected practice is duplicate
                       ExistingPracticesCountforSelectedDuplicate = context.CMDPracticeDuplicateMap
                                                                   .Where(cmdPracticeDuplicateMap => cmdPracticeDuplicateMap.IsActive && cmdPracticeDuplicateMap.ResolveAction == null
                                                                   && cmdPracticeDuplicateMap.DuplicatePracticeID == cmdPractice.Id)
                                                                   .Join(context.CMDPractices, recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.ExistingPracticeID, recordCMDPractice => recordCMDPractice.Id,
                                                                   (recordCMDPracticeDuplicateMap, recordCMDPractice) => recordCMDPractice)
                                                                   .Where(recordCMDPractice => recordCMDPractice.IsActive).Count(),

                       CreatedDate = cmdPractice.CreatedDate,

                       //// List of emails of practice
                       CMDPracticeEmailList = context.CMDPracticeEmails.Where(cmdPracticeEmail => cmdPracticeEmail.CMDPracticeID == cmdPractice.Id &&
                                               cmdPracticeEmail.IsActive).ToList(),

                       //// List of phones of practice
                       CMDPracticePhoneList = context.CMDPracticePhones.Where(cmdPracticePhone => cmdPracticePhone.CMDPracticeID == cmdPractice.Id &&
                                               cmdPracticePhone.IsActive).ToList(),

                       //// if practice has duplicate records then set the value True else False.
                       HasDuplicates = context.CMDPracticeDuplicateMap
                                       .Where(recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.ExistingPracticeID == cmdPractice.Id && recordCMDPracticeDuplicateMap.IsActive && recordCMDPracticeDuplicateMap.ResolveAction == null)
                                       .Any(),
                   };
        }

        #endregion Private Methods
    }
}