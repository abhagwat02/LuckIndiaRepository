using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDPracticeDetailsRunnable : BaseApiController, IRunnable
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

            List<CMDDashboardPractice> practiceListResult = new List<CMDDashboardPractice>();

            int duplicatePracticeId = Convert.ToInt32(queryParameters);

            IQueryable<CMDDashboardPractice> practiceDetails = null;
            try
            {
                practiceDetails = CreateSelectQuery(context, duplicatePracticeId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        practiceDetails = practiceDetails.Where(whereCondition);
                    }
                }

                var countPractice = practiceDetails.Count();

                if (countPractice > 0)
                {
                    //// If value of take passed as 0 then fetch all the practices 
                    if (take == 0)
                    {
                        take = countPractice;
                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        practiceDetails = practiceDetails.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {
                        practiceDetails = practiceDetails.OrderBy("Id").Skip(skip).Take(take);
                    }

                    practiceListResult = practiceDetails.ToList();
                }

                if (practiceListResult != null)
                {
                    practiceListResult.All(m => { m.PracticeCount = countPractice; return true; });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return practiceListResult as List<T>;
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
        /// <param name="duplicatePracticeId">Duplicate practice id to fetch the practice details</param>
        /// <returns>query to fetch the list of practices</returns>
        private static IQueryable<CMDDashboardPractice> CreateSelectQuery(CMDDatabaseContext context, int duplicatePracticeId)
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

                       //// Fetch the list of speciality for practice
                       CMDSpecialityList = (from cmdCustomer in
                                               (from practice in context.CMDPractices
                                                where practice.IsActive
                                                join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                                  on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                                where cmdCustomerPracticeMap.CMDPracticeID != null && practice.Id == cmdPractice.Id &&
                                                  cmdCustomerPracticeMap.IsActive &&
                                                  cmdCustomerPracticeMap.CMDCustomer.IsActive
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

                       //// Fetch the list of business unit ids
                       CMDBusinessUnitIdsList = context.CMDAccounts
                                               .Where(cmdAccount => cmdAccount.CMDPracticeID == cmdPractice.Id && cmdAccount.IsActive && cmdAccount.CMDBusinessUnit.IsActive)
                                               .Select(recordAccount => recordAccount.CMDBusinessUnitId).Distinct(),

                       //// Fetch physician count of practice
                       PhysicianCount = (from practice in context.CMDPractices
                                         where practice.IsActive
                                         join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                         on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                         where practice.Id == cmdPractice.Id && cmdCustomerPracticeMap.IsActive &&
                                         cmdCustomerPracticeMap.CMDCustomer.CMDCustomerType ==
                                         (from cmdCustomer in context.CMDCustomers
                                           where cmdCustomer.CMDCustomerType.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDCustomerTypeNamePhysician &&
                                               cmdCustomer.IsActive && !cmdCustomer.IsDuplicate
                                           select cmdCustomer.CMDCustomerType).FirstOrDefault()
                                         select cmdCustomerPracticeMap.CMDCustomer).Distinct().Count(),

                       //// Fetch duplicate practice count of practice
                       DuplicatePracticesPhysicianCount = duplicatePracticeId == 0 ? 0 : (from practice in context.CMDPractices
                                                                                          where practice.IsActive
                                                                                          join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                                                                                          on practice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                                                                                          where practice.Id == duplicatePracticeId && cmdCustomerPracticeMap.IsActive &&
                                                                                          cmdCustomerPracticeMap.CMDCustomer.CMDCustomerType ==
                                                                                          (from cmdCustomer in context.CMDCustomers
                                                                                            where cmdCustomer.CMDCustomerType.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDCustomerTypeNamePhysician &&
                                                                                           cmdCustomer.IsActive
                                                                                            select cmdCustomer.CMDCustomerType).FirstOrDefault()
                                                                                          select cmdCustomerPracticeMap.CMDCustomer).Distinct().Count(),

                       //// Fetch contact list of practice
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

                       //// Fetch contact count of practice
                       CMDContactCount = (from cmdContact in context.CMDContacts
                                          where cmdContact.IsActive
                                          join cmdPracticeContactMap in context.CMDPracticeContactMaps
                                          on cmdContact.Id equals cmdPracticeContactMap.CMDContactID
                                          where cmdPracticeContactMap.CMDPracticeID == cmdPractice.Id && cmdPracticeContactMap.IsActive
                                          select cmdContact).Count(),

                       IsDuplicate = cmdPractice.IsDuplicate,

                       ////If the selected practice is duplicate practice then, give count of Existing Practices for which the selected practice is duplicate
                       DuplicateMatchPercent = context.CMDPracticeDuplicateMap.FirstOrDefault(recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.DuplicatePracticeID == cmdPractice.Id && recordCMDPracticeDuplicateMap.IsActive && recordCMDPracticeDuplicateMap.ResolveAction == null).DuplicateMatchPercent,

                       CMDDashboardSourceRecordList = context.CMDPracticeSourceTracks
                                                   .Where(cmdPracticeSourceTrack => cmdPracticeSourceTrack.IsActive &&
                                                       cmdPracticeSourceTrack.CMDPracticeID == cmdPractice.Id).Select(cmdPracticeSourceTrack => new CMDDashboardSourceRecord
                                                       {
                                                           CMDBusinessUnitName = context.CMDBusinessUnits.Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                                       cmdBusinessUnit.Id == cmdPracticeSourceTrack.BusinessUnitID).FirstOrDefault().Name,
                                                           SourceRecordID = cmdPracticeSourceTrack.SourceRecordID,
                                                           CreatedDate = cmdPracticeSourceTrack.CreatedDate
                                                       }).Distinct(),

                       CMDSourceRecordCount = context.CMDPracticeSourceTracks
                                               .Where(cmdPracticeSourceTrack => cmdPracticeSourceTrack.IsActive &&
                                                   cmdPracticeSourceTrack.CMDPracticeID == cmdPractice.Id).Select(cmdPracticeSourceTrack => new CMDDashboardSourceRecord
                                                   {
                                                       CMDBusinessUnitName = context.CMDBusinessUnits.Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                                       cmdBusinessUnit.Id == cmdPracticeSourceTrack.BusinessUnitID).FirstOrDefault().Name,
                                                       SourceRecordID = cmdPracticeSourceTrack.SourceRecordID,
                                                       CreatedDate = cmdPracticeSourceTrack.CreatedDate
                                                   }).Distinct().Count(),

                       //// List of emails of practice
                       CMDPracticeEmailList = context.CMDPracticeEmails.Where(cmdPracticeEmail => cmdPracticeEmail.CMDPracticeID == cmdPractice.Id &&
                                               cmdPracticeEmail.IsActive).ToList(),

                       //// List of phones of practice
                       CMDPracticePhoneList = context.CMDPracticePhones.Where(cmdPracticePhone => cmdPracticePhone.CMDPracticeID == cmdPractice.Id &&
                                               cmdPracticePhone.IsActive).ToList(),

                       CreatedDate = cmdPractice.CreatedDate,
                   };
        }
        #endregion
    }
}