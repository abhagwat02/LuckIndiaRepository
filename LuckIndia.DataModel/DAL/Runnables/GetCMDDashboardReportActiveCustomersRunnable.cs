using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDDashboardReportActiveCustomersRunnable : IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            context.Configuration.LazyLoadingEnabled = false;

            #region FilterByYears
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MinValue;

            string filterByYearString = string.Empty;
            if (whereCondition.Contains("period|"))
            {
                filterByYearString = CommonUtility.GetFilterByStartEndParameters(ref whereCondition, "period|", "|period");
            }

            if (!string.IsNullOrEmpty(filterByYearString))
            {
                string[] filterByYearArray = filterByYearString.Split(',');

                if (!string.IsNullOrEmpty(filterByYearArray[0]))
                {
                    startDate = Convert.ToDateTime(filterByYearArray[0]);
                }

                if (!string.IsNullOrEmpty(filterByYearArray[1]))
                {
                    endDate = Convert.ToDateTime(filterByYearArray[1]);
                }

                ////cmdDashboardCustomerTransactionList = cmdDashboardCustomerTransactionList.Where(t => t.TxDate >= startDate && t.TxDate <= endDate);
            }

            #endregion

            IQueryable<CMDDashboardActiveCustomersReport> activeCustomerReport =
                from cmdCust in context.CMDCustomers
                select new CMDDashboardActiveCustomersReport()
                {
                    CustomerID = cmdCust.Id,
                    IsActive = cmdCust.IsActive,
                    PhysicianName = cmdCust.FirstName + " " + cmdCust.MiddleName + " " + cmdCust.LastName,

                    businessUnitTransactionDateList =
                    context.CMDBusinessUnits.Where(cmdBusinessUnitRecord => cmdBusinessUnitRecord.IsActive)
                    .Select(cmdBusinessUnit => new CMDDashboardBusinessUnitandLastTransactionDate()
                    {
                        businessUnitID = cmdBusinessUnit.Id,
                        businessUnitName = cmdBusinessUnit.Name,

                        TxDate = context.CMDTransactions
                        .Where(cmdTransactionRecord => cmdTransactionRecord.CMDAccountID ==
                        context.CMDCustomerAccountMaps
                   .Where(cmdCustomerAccountMapRecord =>
                   cmdCustomerAccountMapRecord.IsActive
                   && cmdCustomerAccountMapRecord.CMDAccount.IsActive
                   && cmdCustomerAccountMapRecord.CMDCustomerID == cmdCust.Id
                   && (cmdCustomerAccountMapRecord.CMDBusinessUnitID == cmdBusinessUnit.Id
                   || cmdCustomerAccountMapRecord.CMDAccount.CMDBusinessUnitId == cmdBusinessUnit.Id)).Select(cmdCustomerAccountMapRecord => cmdCustomerAccountMapRecord.CMDAccountID).FirstOrDefault())
                   .Where(recordCmdTransaction =>
                       recordCmdTransaction.TxDate >= startDate
                       && recordCmdTransaction.TxDate <= endDate)
                   .OrderByDescending(cmdTransactionRec => cmdTransactionRec.TxDate)
                   .Select(cmdTransactionRec => cmdTransactionRec.TxDate)
                   .FirstOrDefault()
                    })
                };

            if (!string.IsNullOrEmpty(whereCondition))
            {
                activeCustomerReport = activeCustomerReport.Where(whereCondition);
            }

            var countCustomer = activeCustomerReport.Count();
            if (!string.IsNullOrEmpty(orderByCondition))
            {
                activeCustomerReport = activeCustomerReport.OrderBy(orderByCondition).Skip(skip).Take(take);
            }
            else
            {
                activeCustomerReport = activeCustomerReport.OrderBy("CustomerID").Skip(skip).Take(take);
            }

            var activeCustomerReportListResult = activeCustomerReport.ToList();
            if (activeCustomerReportListResult != null)
            {
                activeCustomerReportListResult.All(m => { m.CustomersCount = countCustomer; return true; });
            }

            return activeCustomerReportListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}