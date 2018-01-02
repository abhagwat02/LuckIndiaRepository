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
    internal sealed class GetCMDTransactionListByPracticeIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;

            int practiceId = 0;

            List<CMDDashboardTransaction> customerPracticeListResult = null;

            try
            {
                if (!string.IsNullOrEmpty(queryParameters) && Convert.ToInt32(queryParameters) > 0)
                {
                    practiceId = Convert.ToInt32(queryParameters);
                }
                else
                {
                    return customerPracticeListResult as List<T>;
                }

                #region Select Query
                IQueryable<CMDDashboardTransaction> cmdDashboardPracticeTransactionList = null;
                var queyGetCMDTransactionListByPracticeId = ApplicationServices.LINQQueries.CMDTransaction.GetQueryTransactionListByCMDPracticeId(context, practiceId);

                cmdDashboardPracticeTransactionList = GetCMDDashboardPracticeTransactionList(context, queyGetCMDTransactionListByPracticeId);
                #endregion Select Query

                #region Where Condition
                //// Create where condition
                CreateWhereCondition(ref whereCondition, ref cmdDashboardPracticeTransactionList);
                #endregion Where Condition

                var countTransaction = cmdDashboardPracticeTransactionList.Count();

                take = take == 0 ? countTransaction : take;

                #region OrderBy Condition
                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";

                cmdDashboardPracticeTransactionList = cmdDashboardPracticeTransactionList.OrderBy(orderByCondition).Skip(skip).Take(take);

                #endregion OrderBy Condition

                customerPracticeListResult = cmdDashboardPracticeTransactionList.ToList();

                if (customerPracticeListResult != null)
                {
                    var totalTransactionAmount = customerPracticeListResult.Sum(t => t.TotalAmount);
                    customerPracticeListResult.All(m =>
                    {
                        m.TransactionCount = countTransaction;
                        m.TotalTransactionAmount = totalTransactionAmount;
                        return true;
                    });
                }

                CMDLogger.LogAudit("Obtained the TransactionList by PracticeID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerPracticeListResult as List<T>;
        }

        /// <summary>
        /// Creates a where condition by validating all the input values/parameters and return the final 
        /// </summary>
        /// <param name="whereCondition">string with pre defined conditions which will be validated</param>
        /// <param name="cmdDashboardPracticeTransactionList">IQueryable type list of practice transactions </param>
        private static void CreateWhereCondition(ref string whereCondition, ref IQueryable<CMDDashboardTransaction> cmdDashboardPracticeTransactionList)
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

                    cmdDashboardPracticeTransactionList = cmdDashboardPracticeTransactionList.Where(t => t.TxDate != null && t.TxDate.Value.Month == month && t.TxDate.Value.Year == year);
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

                    //// filter the transaction nullable date b/w start and end date.
                    cmdDashboardPracticeTransactionList = cmdDashboardPracticeTransactionList.Where(t => t.TxDate.HasValue && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) <= endDate.Date);
                }
                #endregion

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardPracticeTransactionList = cmdDashboardPracticeTransactionList.Where(whereCondition);
                }
            }
        }

        /// <summary>
        /// Returns the IQueryable list of transactions
        /// </summary>
        /// <param name="context">Database context object </param>
        /// <param name="queyGetCMDTransactionListByPracticeId">IQueryable list </param>
        /// <returns></returns>
        private static IQueryable<CMDDashboardTransaction> GetCMDDashboardPracticeTransactionList(CMDDatabaseContext context, IQueryable<CMDTransaction> queyGetCMDTransactionListByPracticeId)
        {
            return queyGetCMDTransactionListByPracticeId
                                .Select(transaction => new CMDDashboardTransaction()
                                {
                                    Id = transaction.Id,

                                    TxDate = transaction.TxDate,

                                    CMDTransactionTypeID = transaction.CMDTransactionTypeID,

                                    IsActive = transaction.IsActive,

                                    CurrencyType = transaction.CurrencyType,

                                    CMDTransactionTypeName = transaction.CMDTransactionType.TxType,

                                    BusinessUnitName = transaction.CMDAccount.CMDBusinessUnit.Name,

                                    TransactionSourceRecordId = context.CMDTransactionSourceTracks.Where(cmdTransactionSourceTrack => cmdTransactionSourceTrack.CMDTransactionID == transaction.Id &&
                                      cmdTransactionSourceTrack.IsActive).Select(cmdTransactionSourceTrack => cmdTransactionSourceTrack.SourceRecordID).FirstOrDefault(),

                                    NoOfItems = context.CMDTransactionProductMaps
                                    .Where(cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID == transaction.Id &&
                                    cmdTransactionProductMap.IsActive)
                                    .Select(transactionProductMap => transactionProductMap.CMDProductID).Distinct().Count(),

                                    TotalAmount = transaction.Amount,
                                });
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}