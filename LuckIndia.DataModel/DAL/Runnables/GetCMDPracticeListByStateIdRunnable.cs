using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDPracticeListByStateIdRunnable : BaseApiController, IRunnable
    {
        #region Public Methods
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;
            List<CMDPractice> customerPracticeListResult = null;

            int stateId = Convert.ToInt32(queryParameters);

            if (stateId <= 0)
            {
                return customerPracticeListResult as List<T>;
            }

            try
            {
                IQueryable<CMDPractice> practiceList = CreateSelectQuery(context, stateId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    practiceList = practiceList.Where(whereCondition);
                }

                var countCustomer = practiceList.Count();
                take = take == 0 ? countCustomer : take;

                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                practiceList = practiceList.OrderBy(orderByCondition).Skip(skip).Take(take);

                customerPracticeListResult = practiceList.ToList();
                CMDLogger.LogAudit("Obtained the Practice List by State ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerPracticeListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Create query to get the list of practice by selecte practice
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="stateId">Selected state id</param>
        /// <returns>query to fetch the list of practice</returns>
        private static IQueryable<CMDPractice> CreateSelectQuery(CMDDatabaseContext context, int stateId)
        {
            return context.CMDPractices
                                .Join(
                                context.CMDPracticeContactMaps,
                                recPractice => recPractice.Id,
                                cmdPracticeContactMap => cmdPracticeContactMap.CMDPracticeID,
                                (recPractice, cmdPracticeContactMap) => new { recPractice, cmdPracticeContactMap })
                                .Where(record => record.cmdPracticeContactMap.IsActive
                                && record.cmdPracticeContactMap.CMDContact.CMDStateID == stateId
                                && record.cmdPracticeContactMap.CMDContact.CMDState.IsActive
                                && record.cmdPracticeContactMap.CMDContact.IsActive)
                                .Select(record => record.cmdPracticeContactMap.CMDPractice).Distinct();
        }

        #endregion Private Methods
    }
}
