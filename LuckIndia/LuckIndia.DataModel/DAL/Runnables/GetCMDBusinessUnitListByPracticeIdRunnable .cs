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
    internal sealed class GetCMDBusinessUnitListByPracticeIdRunnable : BaseApiController, IRunnable
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

            int practiceId = Convert.ToInt32(queryParameters);

            List<CMDDashboardCustomerBusinessUnit> customerPracticeListResult = null;

            try
            {
                IQueryable<CMDDashboardCustomerBusinessUnit> cmdDashboardPracticeProductList = CreateSelectQuery(context, practiceId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardPracticeProductList = cmdDashboardPracticeProductList.Where(whereCondition);
                }

                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                cmdDashboardPracticeProductList = cmdDashboardPracticeProductList.OrderBy(orderByCondition).Skip(skip).Take(take);

                customerPracticeListResult = cmdDashboardPracticeProductList.ToList();
                CMDLogger.LogAudit("Obtained the BusinessUnit List by Practice ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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
        /// This method create the select query to get the list of business units
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="practiceId">Cmdpractice id</param>
        /// <returns></returns>
        private static IQueryable<CMDDashboardCustomerBusinessUnit> CreateSelectQuery(CMDDatabaseContext context, int practiceId)
        {
            return context.CMDAccounts.Where(cmdAccount => cmdAccount.CMDPracticeID == practiceId &&
                                      cmdAccount.IsActive &&
                                      cmdAccount.CMDPractice.IsActive)
                                      .Select(cmdAcc => cmdAcc.CMDBusinessUnit)
                                      .Select(cmdBusinessUnit => new CMDDashboardCustomerBusinessUnit
                                      {
                                          CustomerSupportEmail = cmdBusinessUnit.CMDContact.PublicEmail,
                                          CustomerSupportNumber = cmdBusinessUnit.CMDContact.Phone,
                                          FirstTransactionDate = context.CMDAccounts.Where(cmdAccount => cmdAccount.CMDPracticeID == practiceId &&
                                                                                    cmdAccount.IsActive &&
                                                                                    cmdAccount.CMDPractice.IsActive)
                                        .Select(cmdAcc => cmdAcc.CMDTransactions)
                                        .SelectMany(transact => transact)
                                        .Where(cmdTransaction => cmdTransaction.IsActive).ToList().OrderBy(transact => transact.TxDate).FirstOrDefault().TxDate,
                                          Id = cmdBusinessUnit.Id,
                                          IsActive = cmdBusinessUnit.IsActive,
                                          Name = cmdBusinessUnit.Name,
                                          WebURL = cmdBusinessUnit.CMDContact.WebsiteURL,
                                          ImageURL = cmdBusinessUnit.ImageURL,
                                          CMDContact = cmdBusinessUnit.CMDContact,
                                          CMDContactID = cmdBusinessUnit.CMDContactID,
                                      }).Distinct();
        }

        #endregion Private Methods
    }
}
