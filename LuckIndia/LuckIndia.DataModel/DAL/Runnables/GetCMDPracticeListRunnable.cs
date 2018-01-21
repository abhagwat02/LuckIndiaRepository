using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDPracticeListRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            ////check if search string contains ;amp; , then replace with &  
            whereCondition = whereCondition.ToggleAmpersand();

            context.Configuration.LazyLoadingEnabled = false;

            List<CMDDashboardPractice> practicelistResult = new List<CMDDashboardPractice>();

            //// Business unit id to filter list of practices
            int businessUnitIdFilter = Convert.ToInt32(queryParameters);

            IQueryable<CMDDashboardPractice> practiceList = null;

            try
            {
                //// Fetch all the practices belongs to selected business unit
                if (businessUnitIdFilter > 0)
                {
                    IQueryable<Models.CMDPractice> objCMDPractice = null;
                    objCMDPractice = context.CMDPracticeSourceTracks.Where(cmdPracticeSourceTrackRecord => cmdPracticeSourceTrackRecord.IsActive &&
                                                                            cmdPracticeSourceTrackRecord.CMDPractice.IsActive &&
                                                                            cmdPracticeSourceTrackRecord.BusinessUnitID == businessUnitIdFilter)
                                                                            .Select(cmdPracticeSourceTrackRecord => cmdPracticeSourceTrackRecord.CMDPractice);

                    practiceList = CreateSelectQuery(context, objCMDPractice).GroupBy(practiceRecord => practiceRecord.Id).Select(practiceRecord => practiceRecord.FirstOrDefault());
                }
                else
                {
                    practiceList = CreateSelectQuery(context, context.CMDPractices);
                }

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    #region FilterBySpecialityId
                    string specialitytId = string.Empty;

                    if (whereCondition.Contains("splt|"))
                    {
                        specialitytId = CommonUtility.GetFilterByStartEndParameters(ref whereCondition, "splt|", "|splt");
                    }

                    if (!string.IsNullOrEmpty(specialitytId))
                    {
                        int specialitytIdInt = Convert.ToInt32(specialitytId);
                        practiceList = practiceList.Where(m => m.CMDSpecialityList.Any(c => c.Id == specialitytIdInt));
                    }

                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        practiceList = practiceList.Where(whereCondition);
                    }
                    #endregion
                }

                var countPractice = practiceList.Count();

                if (countPractice > 0)
                {
                    //// If value of take passed as 0 then fetch all the practices 
                    if (take == 0)
                    {
                        take = countPractice;
                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        practiceList = practiceList.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {
                        practiceList = practiceList.OrderBy("Id").Skip(skip).Take(take);
                    }

                    practicelistResult = practiceList.ToList();
                }

                if (practicelistResult != null)
                {
                    practicelistResult.All(m => { m.PracticeCount = countPractice; return true; });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return practicelistResult as List<T>;
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
        /// <param name="objCMDPractice">CMDPractice</param>
        /// <returns>query to fetch the list of practices</returns>
        private static IQueryable<CMDDashboardPractice> CreateSelectQuery(CMDDatabaseContext context, IQueryable<CMDPractice> objCMDPractice)
        {
            return objCMDPractice.Select(cmdPractice => new CMDDashboardPractice()
            {
                Id = cmdPractice.Id,
                Phone = cmdPractice.Phone,
                PracticeName = cmdPractice.PracticeName,
                PrimaryOwnerID = cmdPractice.PrimaryOwnerID,
                IsActive = cmdPractice.IsActive,
                OriginalBusinessUnitID = cmdPractice.OriginalBusinessUnitID,

                //// Fetch the business unit name
                OriginalBusinessUnitName = context.CMDBusinessUnits.Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                                            cmdBusinessUnit.Id == cmdPractice.OriginalBusinessUnitID)
                                                            .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

                PrimaryOwnerFirstName = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.Id == cmdPractice.PrimaryOwnerID && cmdPractice.PrimaryOwnerID != null && cmdCustomer.IsActive)
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
                                         CreatedDate = cmdCustomerSpecialityMap.CMDSpeciality.CreatedDate,
                                         ParentSpecialityID = cmdCustomerSpecialityMap.CMDSpeciality.ParentSpecialityID,
                                         UpdatedBy = cmdCustomerSpecialityMap.CMDSpeciality.UpdatedBy,
                                         UpdatedDate = cmdCustomerSpecialityMap.CMDSpeciality.UpdatedDate
                                     }).Distinct<CMDDashboardSpeciality>(),

                CMDBusinessUnitCount = context.CMDAccounts.Where(cmdAccount => cmdAccount.CMDPracticeID == cmdPractice.Id && cmdAccount.IsActive && cmdAccount.CMDBusinessUnit.IsActive)
                                                                        .Select(recordAccount => recordAccount.CMDBusinessUnitId).Distinct().Count(),

                //// Find the number of products for practice
                CMDProductCount =
                                                                context.CMDPractices
                                                                    .Where(cmdPracticeRecord => cmdPracticeRecord.Id == cmdPractice.Id &&
                                                                    cmdPracticeRecord.IsActive)
                                                                    .SelectMany(cmdPracticeRecord => cmdPracticeRecord.CMDAccounts)
                                                                    .Where(cmdAccount => cmdAccount.IsActive)
                                                                    .SelectMany(j => j.CMDTransactions)
                                                                    .Where(cmdTransaction => cmdTransaction.IsActive)
                                                                    .Join(
                                                                    context.CMDTransactionProductMaps,
                                                                    r => r.Id,
                                                                    cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                                    (r, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                                    .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive &&
                                                                    cmdTransactionProductMap.CMDProduct.IsActive)
                                                                    .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct).Distinct().Count(),

                //// Find the number of physician for practice
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

                IsDuplicate = cmdPractice.IsDuplicate,

                //// if practice has duplicate records then set the value True else False.
                HasDuplicates = context.CMDPracticeDuplicateMap
                                                                .Where(recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.ExistingPracticeID == cmdPractice.Id && recordCMDPracticeDuplicateMap.IsActive && recordCMDPracticeDuplicateMap.ResolveAction == null)
                                                                .Any(),

                CreatedDate = cmdPractice.CreatedDate,
            });
        }

        #endregion Private Methods
    }
}