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
    internal sealed class GetCMDTransactionListByCustomerIdRunnable : BaseApiController, IRunnable
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

            int customerId = 0;

            List<CMDDashboardTransaction> customerTransactionListResult = null;
            try
            {
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    customerId = Convert.ToInt32(queryParameters);
                }

                #region Select Query

                IQueryable<CMDDashboardTransaction> cmdDashboardCustomerTransactionList = CreateSelectQuery(context, customerId);

                #endregion Select Query

                #region Where Condition

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    #region FilterByMonths
                    string filterByMonthString = string.Empty;
                    if (whereCondition.Contains("months|"))
                    {
                        filterByMonthString = CommonUtility.GetFilterByStartEndParameters(ref whereCondition, "months|", "|months");
                    }

                    if (!string.IsNullOrEmpty(filterByMonthString))
                    {
                        string[] filterByMonthArray = filterByMonthString.Split(',');

                        int month = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(filterByMonthArray[0]))
                        {
                            month = Convert.ToInt32(filterByMonthArray[0]);
                        }

                        int year = DateTime.Now.Year;
                        if (!string.IsNullOrEmpty(filterByMonthArray[1]))
                        {
                            year = Convert.ToInt32(filterByMonthArray[1]);
                        }

                        cmdDashboardCustomerTransactionList = cmdDashboardCustomerTransactionList.Where(t => t.TxDate != null && t.TxDate.Value.Month == month && t.TxDate.Value.Year == year);
                    }

                    #endregion

                    #region FilterByperiod
                    string filterByYearString = string.Empty;
                    if (whereCondition.Contains("period|"))
                    {
                        filterByYearString = CommonUtility.GetFilterByStartEndParameters(ref whereCondition, "period|", "|period");
                    }

                    if (!string.IsNullOrEmpty(filterByYearString))
                    {
                        string[] filterByYearArray = filterByYearString.Split(',');

                        var startDate = DateTime.MinValue;
                        if (!string.IsNullOrEmpty(filterByYearArray[0]))
                        {
                            startDate = Convert.ToDateTime(filterByYearArray[0]);
                        }

                        var endDate = DateTime.MinValue;
                        if (!string.IsNullOrEmpty(filterByYearArray[1]))
                        {
                            endDate = Convert.ToDateTime(filterByYearArray[1]);
                        }

                        // filter the transaction nullable date b/w start and end date.
                        cmdDashboardCustomerTransactionList = cmdDashboardCustomerTransactionList.Where(t => t.TxDate.HasValue && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) <= endDate.Date);
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        cmdDashboardCustomerTransactionList = cmdDashboardCustomerTransactionList.Where(whereCondition);
                    }
                }
                #endregion Where Condition

                var countTransaction = cmdDashboardCustomerTransactionList.Count();

                take = take == 0 ? countTransaction : take;

                #region OrderBy Condition
                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                cmdDashboardCustomerTransactionList = cmdDashboardCustomerTransactionList.OrderBy(orderByCondition).Skip(skip).Take(take);
                #endregion OrderBy Condition

                customerTransactionListResult = cmdDashboardCustomerTransactionList.ToList();

                if (customerTransactionListResult != null)
                {
                    var totalTransactionAmount = customerTransactionListResult.Sum(t => t.TotalAmount);
                    customerTransactionListResult.All(m =>
                    {
                        m.TransactionCount = countTransaction; m.TotalTransactionAmount = totalTransactionAmount;
                        return true;
                    });
                }

                CMDLogger.LogAudit("Obtained the TransactionList by Customer ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerTransactionListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Create select query to get the list of transctions
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="customerId">Customer id to get the details</param>
        /// <returns>Query to get transctions</returns>
        private static IQueryable<CMDDashboardTransaction> CreateSelectQuery(CMDDatabaseContext context, int customerId)
        {
            return ApplicationServices.LINQQueries.CMDTransaction.GetQueryTransactionListByCMDCustomerId(context, customerId)
                .Select(transaction => new CMDDashboardTransaction()
                {
                    Id = transaction.Id,

                    TxDate = transaction.TxDate,

                    CMDTransactionTypeID = transaction.CMDTransactionTypeID,

                    IsActive = transaction.IsActive,

                    CurrencyType = transaction.CurrencyType,

                    CMDTransactionTypeName = transaction.CMDTransactionType.TxType,

                    BusinessUnitName = transaction.CMDAccount.CMDBusinessUnit.Name,

                    TransactionSourceRecordId = context.CMDTransactionSourceTracks
                    .Where(cmdTransactionSourceTrack => cmdTransactionSourceTrack.CMDTransactionID == transaction.Id &&
                    cmdTransactionSourceTrack.IsActive)
                    .Select(cmdTransactionSourceTrack => cmdTransactionSourceTrack.SourceRecordID).FirstOrDefault(),

                    NoOfItems = context.CMDTransactionProductMaps
                                       .Where(cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID == transaction.Id &&
                                             cmdTransactionProductMap.IsActive)
                                     .Select(transactionProductMap => transactionProductMap.CMDProductID).Distinct().Count(),

                    TotalAmount = transaction.Amount,
                });
        }
        #endregion Private Methods
    }
}
