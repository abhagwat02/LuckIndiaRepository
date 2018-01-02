using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDPracticeListByCustomerIdRunnable : BaseApiController, IRunnable
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
            List<CMDDashboardCustomerPractice> customerPracticeListResult = null;

            int customerId = Convert.ToInt32(queryParameters);

            if (customerId <= 0)
            {
                return customerPracticeListResult as List<T>;
            }

            try
            {
                IQueryable<CMDDashboardCustomerPractice> cmdDashboardCustomerPracticeList = CreateSelectQuery(context, customerId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardCustomerPracticeList = cmdDashboardCustomerPracticeList.Where(whereCondition);
                }

                var countPractice = cmdDashboardCustomerPracticeList.Count();

                //// Fetch all the practice if parameter take is 0
                if (take == 0)
                {
                    take = countPractice;
                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdDashboardCustomerPracticeList = cmdDashboardCustomerPracticeList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {
                    cmdDashboardCustomerPracticeList = cmdDashboardCustomerPracticeList.OrderBy("Id").Skip(skip).Take(take);
                }

                customerPracticeListResult = cmdDashboardCustomerPracticeList.ToList();
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
        /// Create query to get the list of physicians by selecte practice
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="customerId">Selected customer id</param>
        /// <returns>query to fetch the list of practice</returns>
        private static IQueryable<CMDDashboardCustomerPractice> CreateSelectQuery(CMDDatabaseContext context, int customerId)
        {
            return context.CMDCustomerPracticeMaps
                                 .Where(recCMDCustomerPracticeMap => recCMDCustomerPracticeMap.IsActive &&
                                     recCMDCustomerPracticeMap.CMDCustomerID == customerId &&
                                     recCMDCustomerPracticeMap.CMDCustomer.IsActive)
                                .Select(recordCustomerPracticeMap => new CMDDashboardCustomerPractice()
                                {
                                    Id = recordCustomerPracticeMap.CMDPractice.Id,

                                    PracticeName = recordCustomerPracticeMap.CMDPractice.PracticeName,

                                    OriginalBusinessUnitID = recordCustomerPracticeMap.CMDPractice.OriginalBusinessUnitID,

                                    IsActive = recordCustomerPracticeMap.CMDPractice.IsActive,

                                    StateAbbreviation = context.CMDPracticeContactMaps.Where(cmdPracticeContactMap => cmdPracticeContactMap.IsActive
                                                          && cmdPracticeContactMap.CMDPracticeID == recordCustomerPracticeMap.CMDPractice.Id
                                                          && cmdPracticeContactMap.CMDContact.IsActive
                                                          && cmdPracticeContactMap.CMDContact.IsDefault)
                                                          .Select(cmdPracticeContactMap => cmdPracticeContactMap.CMDContact.CMDState.Abbriviation).FirstOrDefault(),

                                    CountryAbbreviation = context.CMDPracticeContactMaps.Where(cmdPracticeContactMap => cmdPracticeContactMap.IsActive
                                                            && cmdPracticeContactMap.CMDPracticeID == recordCustomerPracticeMap.CMDPractice.Id
                                                            && cmdPracticeContactMap.CMDContact.IsActive
                                                            && cmdPracticeContactMap.CMDContact.IsDefault)
                                                            .Select(cmdPracticeContactMap => cmdPracticeContactMap.CMDContact.CMDState.CMDCountry.Abbreviation).FirstOrDefault(),

                                    //// Is current practice duplicate
                                    IsDuplicate = recordCustomerPracticeMap.CMDPractice.IsDuplicate,
                                });
        }

        #endregion PrivateMethods
    }
}
