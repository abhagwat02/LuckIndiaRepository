using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetExistingCMDPracticeListByDuplicatePracticeIdRunnable : BaseApiController, IRunnable
    {
        #region Public methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string id = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            List<CMDDashboardPractice> existingPracticeListResult = null;
            context.Configuration.LazyLoadingEnabled = false;
            try
            {
                int practiceId = Convert.ToInt32(id);
                IQueryable<CMDDashboardPractice> cmdDashboardExistingPracticeList = CreateSelectQuery(context, practiceId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardExistingPracticeList = cmdDashboardExistingPracticeList.Where(whereCondition);
                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdDashboardExistingPracticeList = cmdDashboardExistingPracticeList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {
                    cmdDashboardExistingPracticeList = cmdDashboardExistingPracticeList.OrderBy("Id").Skip(skip).Take(take);
                }

                existingPracticeListResult = cmdDashboardExistingPracticeList.ToList();

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

        #endregion Public methods

        #region Private Methods

        /// <summary>
        /// This create a query to fetch the list of existing practice for provided practice
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="practiceId">CMDPractice id</param>
        /// <returns>query to fetch the list of existing practice for provided practice</returns>
        private static IQueryable<CMDDashboardPractice> CreateSelectQuery(CMDDatabaseContext context, int practiceId)
        {
            return context.CMDPractices.Where(cmdPractice => cmdPractice.Id == practiceId && cmdPractice.IsActive && cmdPractice.IsDuplicate)
                            .Join(context.CMDPracticeDuplicateMap,
                            recPractice => recPractice.Id,
                            cmdPracticeDuplicateMap => cmdPracticeDuplicateMap.DuplicatePracticeID,
                            (recPractice, cmdPracticeDuplicateMap) => cmdPracticeDuplicateMap)
                            .Where(recCMDPracticeDuplicateMap => recCMDPracticeDuplicateMap.IsActive && recCMDPracticeDuplicateMap.ResolveAction == null)
                            .Select(recPracticeDuplicateMap => recPracticeDuplicateMap)
                            .Join(context.CMDPractices,
                            recordDuplicatePractice => recordDuplicatePractice.ExistingPracticeID,
                            recordPractice => recordPractice.Id,
                            (recordDuplicatePractice, recordPractice) => new CMDDashboardPractice()
                            {
                                Id = recordPractice.Id,
                                PracticeName = recordPractice.PracticeName,
                                Phone = recordPractice.Phone,
                                Email = recordPractice.Email,
                                PrimaryOwnerID = recordPractice.PrimaryOwnerID,
                                IsActive = recordPractice.IsActive,
                                OriginalBusinessUnitID = recordPractice.OriginalBusinessUnitID,

                                //// Name of business unit
                                OriginalBusinessUnitName = context.CMDBusinessUnits
                                                         .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == recordPractice.OriginalBusinessUnitID)
                                                         .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

                                PrimaryOwnerFirstName = (from cmdCustomer in context.CMDCustomers
                                                         where cmdCustomer.Id == recordPractice.PrimaryOwnerID && recordPractice.PrimaryOwnerID != null && cmdCustomer.IsActive
                                                         select cmdCustomer.FirstName).FirstOrDefault(),

                                PrimaryOwnerLastName = (from cmdCustomer in context.CMDCustomers
                                                        where cmdCustomer.Id == recordPractice.PrimaryOwnerID && recordPractice.PrimaryOwnerID != null && cmdCustomer.IsActive
                                                        select cmdCustomer.LastName).FirstOrDefault(),

                                //// List of speciality for practices
                                CMDSpecialityList = (from cmdCustomer in
                                                         (from practice in context.CMDPractices
                                                          join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                                          on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                                          where cmdCustomerPracticeMap.CMDPracticeID != null && practice.Id == recordPractice.Id && cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDCustomer.IsActive
                                                          select cmdCustomerPracticeMap.CMDCustomer)
                                                     join cmdCustomerSpecialityMap in context.CMDCustomerSpecialityMaps
                                                     on cmdCustomer.Id equals cmdCustomerSpecialityMap.CMDCustomerID
                                                     where cmdCustomerSpecialityMap.CMDSpeciality.IsActive
                                                     select new CMDDashboardSpeciality
                                                     {
                                                         Id = cmdCustomerSpecialityMap.CMDSpeciality.Id,
                                                         IsActive = cmdCustomerSpecialityMap.CMDSpeciality.IsActive,
                                                         Name = cmdCustomerSpecialityMap.CMDSpeciality.Name,
                                                         ParentSpecialityID = cmdCustomerSpecialityMap.CMDSpeciality.ParentSpecialityID
                                                     }).Distinct<CMDDashboardSpeciality>(),

                                //// List of contacts for practice
                                CMDContactList = from cmdContact in context.CMDContacts
                                                 where cmdContact.IsActive
                                                 join cmdPracticeContactMap in context.CMDPracticeContactMaps
                                                 on cmdContact.Id equals cmdPracticeContactMap.CMDContactID
                                                 where cmdPracticeContactMap.CMDPracticeID == recordPractice.Id && cmdPracticeContactMap.IsActive
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

                                //// Is current practice duplicate
                                IsDuplicate = recordPractice.IsDuplicate,

                                CreatedDate = recordPractice.CreatedDate,

                                //// List of contacts for practice
                                Criteria = recordDuplicatePractice.Criteria,

                                //// List of emails for practices
                                CMDPracticeEmailList = context.CMDPracticeEmails.Where(cmdPracticeEmail => cmdPracticeEmail.CMDPracticeID == recordPractice.Id &&
                                                        cmdPracticeEmail.IsActive).ToList(),

                                //// List of phones for practices
                                CMDPracticePhoneList = context.CMDPracticePhones.Where(cmdPracticePhone => cmdPracticePhone.CMDPracticeID == recordPractice.Id &&
                                                       cmdPracticePhone.IsActive).ToList(),
                            });
        }

        #endregion Private Methods
    }
}
