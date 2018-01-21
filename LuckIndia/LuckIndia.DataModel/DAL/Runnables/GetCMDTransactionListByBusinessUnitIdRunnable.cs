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
    internal sealed class GetCMDTransactionListByBusinessUnitIdRunnable : BaseApiController, IRunnable
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

            int businessUnitId = 0;

            List<CMDDashboardProductTransaction> transactionListResult = null;

            IQueryable<CMDDashboardProductTransaction> cmdDashboardBusinessUnitTransactionList = null;
            try
            {
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    businessUnitId = Convert.ToInt32(queryParameters);
                }

                if (businessUnitId <= 0)
                {
                    return transactionListResult as List<T>;
                }

                #region Select Query

                cmdDashboardBusinessUnitTransactionList = CreateSelectQuery(context, businessUnitId);

                #endregion Select Query

                #region Where Condition

                CreateWhereCondition(ref whereCondition, ref cmdDashboardBusinessUnitTransactionList);

                #endregion Where Condition

                // All the records are fetched for export functionality
                var countTransaction = cmdDashboardBusinessUnitTransactionList.Count();
                take = take == 0 ? countTransaction : take;

                #region OrederBy Condition

                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.OrderBy(orderByCondition).Skip(skip).Take(take);

                #endregion OrederBy Condition

                transactionListResult = cmdDashboardBusinessUnitTransactionList.ToList();

                if (transactionListResult != null)
                {
                    var totalTransactionAmount = transactionListResult.Sum(t => t.TotalAmount);
                    transactionListResult.All(m =>
                    {
                        m.TransactionCount = countTransaction;
                        m.TotalTransactionAmount = totalTransactionAmount;
                        return true;
                    });
                }

                CMDLogger.LogAudit("Obtained the TransactionList by BusinessUnit ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return transactionListResult as List<T>;
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
        /// <param name="businessUnitId">Business unit id to get the details</param>
        /// <returns>Query to get transctions</returns>
        private static IQueryable<CMDDashboardProductTransaction> CreateSelectQuery(CMDDatabaseContext context, int businessUnitId)
        {
            return ApplicationServices.LINQQueries.CMDTransaction.GetQueryTransactionListByCMDBusinessUnitId(context, businessUnitId)
                .Select(cmdTransaction => new CMDDashboardProductTransaction()
                {
                    Id = cmdTransaction.Id,
                    TxDate = cmdTransaction.TxDate,
                    CMDTransactionTypeID = cmdTransaction.CMDTransactionTypeID,
                    IsActive = cmdTransaction.IsActive,
                    CMDTransactionTypeName = cmdTransaction.CMDTransactionType.TxType,
                    CurrencyType = cmdTransaction.CurrencyType,
                    BusinessUnitName = cmdTransaction.CMDAccount.CMDBusinessUnit.Name,

                    TransactionSourceRecordId = context.CMDTransactionSourceTracks.Where(cmdTransactionSourceTrack => cmdTransactionSourceTrack.CMDTransactionID == cmdTransaction.Id &&
                        cmdTransactionSourceTrack.IsActive).Select(cmdTransactionSourceTrack => cmdTransactionSourceTrack.SourceRecordID).FirstOrDefault(),

                    NoOfItems = context.CMDTransactionProductMaps
                     .Where(cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID == cmdTransaction.Id &&
                           cmdTransactionProductMap.IsActive)
                    .Select(transactionProductMap => transactionProductMap.CMDProductID).Distinct().Count(),

                    TotalAmount = cmdTransaction.Amount,
                });
        }

        private static void CreateWhereCondition(ref string whereCondition, ref IQueryable<CMDDashboardProductTransaction> cmdDashboardBusinessUnitTransactionList)
        {
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

                    cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.Where(t => t.TxDate != null && t.TxDate.Value.Month == month && t.TxDate.Value.Year == year);
                }

                #endregion

                #region FilterByYears
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
                    cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.Where(t => t.TxDate.HasValue && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) <= endDate.Date);
                }
                #endregion

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.Where(whereCondition);
                }
            }
        }

        #endregion Private Methods
    }
}
