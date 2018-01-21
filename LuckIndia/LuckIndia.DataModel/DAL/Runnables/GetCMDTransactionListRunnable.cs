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
    internal sealed class GetCMDTransactionListRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            context.Configuration.LazyLoadingEnabled = false;

            List<CMDDashboardProductTransaction> transactionListResult = null;

            try
            {
                #region Select Query                

                IQueryable<CMDDashboardProductTransaction> transactionList = CreateSelectQuery(context);

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

                        transactionList = transactionList.Where(t => t.TxDate != null && t.TxDate.Value.Month == month && t.TxDate.Value.Year == year);
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
                        transactionList = transactionList.Where(t => t.TxDate.HasValue && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) >= startDate.Date && System.Data.Entity.DbFunctions.TruncateTime(t.TxDate) <= endDate.Date);
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        transactionList = transactionList.Where(whereCondition);
                    }
                }
                #endregion Where Condition
                var transactionsCount = transactionList.Count();

                #region OrderBy Condition
                if (transactionsCount > 0)
                {
                    take = take == 0 ? transactionsCount : take;

                    orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                    transactionList = transactionList.OrderBy(orderByCondition).Skip(skip).Take(take);

                    transactionListResult = transactionList.ToList();
                }

                #endregion OrderBy Condition

                if (transactionListResult != null && transactionListResult.Count() > 0)
                {
                    var totalTransactionAmount = transactionListResult.Sum(t => t.TotalAmount);
                    transactionListResult.All(m =>
                    {
                        m.TransactionCount = transactionsCount; m.TotalTransactionAmount = totalTransactionAmount;
                        return true;
                    });
                }

                CMDLogger.LogAudit("Obtained the TransactionList from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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
        #endregion

        #region Private Methods
        private static IQueryable<CMDDashboardProductTransaction> CreateSelectQuery(CMDDatabaseContext context)
        {
            return ApplicationServices.LINQQueries.CMDTransaction.GetQueryCMDTransactionsList(context)
                                    .Select(cmdTransaction => new CMDDashboardProductTransaction()
                                    {
                                        Id = cmdTransaction.Id,

                                        TxDate = cmdTransaction.TxDate,

                                        CMDTransactionTypeID = cmdTransaction.CMDTransactionTypeID,

                                        CMDAccountID = cmdTransaction.CMDAccountID,

                                        IsActive = cmdTransaction.IsActive,

                                        CurrencyType = cmdTransaction.CurrencyType,

                                        CMDTransactionTypeName = cmdTransaction.CMDTransactionType.TxType,

                                        BusinessUnitName = cmdTransaction.CMDAccount.CMDBusinessUnit.Name,

                                        BusinessUnitID = cmdTransaction.CMDAccount.CMDBusinessUnitId,

                                        TransactionSourceRecordId = context.CMDTransactionSourceTracks.Where(cmdTransactionSourceTrack => cmdTransactionSourceTrack.CMDTransactionID == cmdTransaction.Id &&
                                              cmdTransactionSourceTrack.IsActive).Select(cmdTransactionSourceTrack => cmdTransactionSourceTrack.SourceRecordID).FirstOrDefault(),

                                        NoOfItems = context.CMDTransactionProductMaps
                                        .Where(cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID == cmdTransaction.Id &&
                                              cmdTransactionProductMap.IsActive)
                                      .Select(transactionProductMap => transactionProductMap.CMDProductID).Distinct().Count(),

                                        TotalAmount = cmdTransaction.Amount,
                                    });
        }
        #endregion Private Methods
    }
}