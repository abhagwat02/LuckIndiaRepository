using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
   internal sealed class GetCMDPracticeDuplicateListRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            //// Check if search string contains ;amp; , then replace with &  
            whereCondition = whereCondition.ToggleAmpersand();

            context.Configuration.LazyLoadingEnabled = false;
            List<CMDDashboardPractice> practiceDuplicateListResult = new List<CMDDashboardPractice>();

            IQueryable<CMDDashboardPractice> practiceDuplicateList = null;
            try
            {
                practiceDuplicateList = CreateSelectQuery(context);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    practiceDuplicateList = practiceDuplicateList.Where(whereCondition);
                }

                var countPractice = practiceDuplicateList.Count();

                if (countPractice > 0)
                {
                    //// If value of take passed as 0 then fetch all the practices 
                    if (take == 0)
                    {
                        take = countPractice;
                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        practiceDuplicateList = practiceDuplicateList.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {
                        practiceDuplicateList = practiceDuplicateList.OrderBy("Id").Skip(skip).Take(take);
                    }

                    practiceDuplicateListResult = practiceDuplicateList.ToList();
                }

                if (practiceDuplicateListResult != null)
                {
                    practiceDuplicateListResult.All(m => { m.PracticeCount = countPractice; return true; });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return practiceDuplicateListResult as List<T>;
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
                       PracticeName = cmdPractice.PracticeName,
                       IsActive = cmdPractice.IsActive,
                       OriginalBusinessUnitID = cmdPractice.OriginalBusinessUnitID,

                       OriginalBusinessUnitName = context.CMDBusinessUnits
                                                      .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == cmdPractice.OriginalBusinessUnitID)
                                                      .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

                       //// Fetch the business unit list
                       CMDBusinessUnitIdsList = context.CMDAccounts
                                                      .Where(cmdAccount => cmdAccount.CMDPracticeID == cmdPractice.Id && cmdAccount.IsActive && cmdAccount.CMDBusinessUnit.IsActive)
                                                      .Select(recordAccount => recordAccount.CMDBusinessUnitId).Distinct(),

                       IsDuplicate = cmdPractice.IsDuplicate,

                       ////If the selected practice is duplicate practice then, give count of Existing Practices for which the selected practice is duplicate
                       ExistingPracticesCountforSelectedDuplicate = context.CMDPracticeDuplicateMap
                                                                      .Where(cmdPracticeDuplicateMap => cmdPracticeDuplicateMap.IsActive && cmdPracticeDuplicateMap.ResolveAction == null
                                                                       && cmdPracticeDuplicateMap.DuplicatePracticeID == cmdPractice.Id)
                                                                       .Join(context.CMDPractices, recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.ExistingPracticeID, recordCMDPractice => recordCMDPractice.Id,
                                                                       (recordCMDPracticeDuplicateMap, recordCMDPractice) => recordCMDPractice)
                                                                       .Where(recordCMDPractice => recordCMDPractice.IsActive).Count(),

                       CreatedDate = cmdPractice.CreatedDate,
                   };
        }

        #endregion Private Methods
    }
}