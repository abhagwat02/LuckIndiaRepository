using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDTransactionDetailsByTransactionIdRunnable : IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            int transactionId = 0;

            List<CMDDashboardProductTransaction> transactionListResult = new List<CMDDashboardProductTransaction>();
            if (!string.IsNullOrEmpty(queryParameters))
            {
                transactionId = Convert.ToInt32(queryParameters);
            }

            if (transactionId <= 0)
            {
                return transactionListResult as List<T>;
            }

            context.Configuration.LazyLoadingEnabled = false;

            IQueryable<CMDDashboardProductTransaction> cmdDashboardBusinessUnitTransactionList = null;

            #region Select Query

            cmdDashboardBusinessUnitTransactionList = CreateSelectQuery(context, transactionId);

            #endregion Select Query

            ////Create where condition
            CreateWhereContidion(ref whereCondition, ref cmdDashboardBusinessUnitTransactionList);

            //// In case of export transaction
            var countTransaction = cmdDashboardBusinessUnitTransactionList.Count();

            take = take == 0 ? countTransaction : take;

            #region OrderBy Condition
            orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";

            cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.OrderBy(orderByCondition).Skip(skip).Take(take);

            #endregion OrderBy Condition

            transactionListResult = cmdDashboardBusinessUnitTransactionList.ToList();

            if (transactionListResult != null)
            {
                transactionListResult.All(m =>
                {
                    m.TransactionCount = countTransaction;
                    return true;
                });
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
        /// <param name="transactionId">Transction id to get the details</param>
        /// <returns>Query to get transctions</returns>
        private static IQueryable<CMDDashboardProductTransaction> CreateSelectQuery(CMDDatabaseContext context, int transactionId)
        {
            return ApplicationServices.LINQQueries.CMDTransaction.GetQueryTransactionDetailsByCMDTransactionId(context, transactionId)
                            .Select(transactionProductMap => new CMDDashboardProductTransaction()
                            {
                                Id = transactionProductMap.CMDTransaction.Id,

                                TxDate = transactionProductMap.CMDTransaction.TxDate,

                                IsActive = transactionProductMap.CMDTransaction.IsActive,

                                CMDTransactionType = transactionProductMap.CMDTransaction.CMDTransactionType,

                                CMDTransactionTypeName = transactionProductMap.CMDTransaction.CMDTransactionType.TxType,

                                //// Get the contact list for the selected transactions
                                TransactionContactList = context.CMDTransactionContactMaps
                                                            .Where(cmdTransactionContactMap =>
                                                                cmdTransactionContactMap.IsActive &&
                                                                cmdTransactionContactMap.CMDTransactionID == transactionProductMap.CMDTransaction.Id &&
                                                                cmdTransactionContactMap.CMDContact.IsActive &&
                                                                cmdTransactionContactMap.CMDTransaction.IsActive)
                                                             .Select(cmdTransactionContactMap => new CMDDashboardTransactionContact()
                                                             {
                                                                 CMDContact = cmdTransactionContactMap.CMDContact,
                                                                 ContactTypeID = cmdTransactionContactMap.ContactType,
                                                                 CMDStateAbbreviation = cmdTransactionContactMap.CMDContact.CMDState.Abbriviation,
                                                                 CMDCountryAbbreviation = cmdTransactionContactMap.CMDContact.CMDState.CMDCountry.Abbreviation,
                                                             }),

                                BusinessUnitName = transactionProductMap.CMDTransaction.CMDAccount.CMDBusinessUnit.Name,

                                BusinessUnitID = (int?)transactionProductMap.CMDProduct.CMDBusinessUnitID,

                                ProductName = transactionProductMap.CMDProduct.Name,

                                ProductID = transactionProductMap.CMDProduct.Id,

                                Amount = (double?)transactionProductMap.Amount ?? 0,

                                UnitPrice = (double?)transactionProductMap.UnitPrice ?? 0,

                                TotalTransactionAmount = (double?)transactionProductMap.CMDTransaction.Amount ?? 0,

                                TransactionCurrencyType = transactionProductMap.CMDTransaction.CurrencyType,

                                Quantity = transactionProductMap.Quantity,

                                ////Name of the Entity who did the transaction
                                TransactionDoneBy = context.CMDAccounts
                                .Where(cmdAccount => cmdAccount.IsActive &&
                                cmdAccount.Id == transactionProductMap.CMDTransaction.CMDAccountID)
                                .Select(cmdAccount => cmdAccount.CMDPractice)
                                .Where(cmdPractice => cmdPractice.IsActive)
                                .Select(cmdPractice => cmdPractice.PracticeName).FirstOrDefault() ??
                                context.CMDAccounts
                                .Where(cmdAccount => cmdAccount.IsActive &&
                                cmdAccount.Id == transactionProductMap.CMDTransaction.CMDAccountID &&
                                cmdAccount.CMDPracticeID == null)
                                .Join(context.CMDCustomerAccountMaps, recordAccount => recordAccount.Id, recordCustomerAccountMap => recordCustomerAccountMap.CMDAccountID, (recordAccount, recordCustomerAccountMap) => recordCustomerAccountMap)
                                .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive && cmdCustomerAccountMap.CMDCustomer.IsActive)
                                .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomer)
                                .Select(cmdCustomer => new { TransactionDoneBy = String.Concat(cmdCustomer.FirstName + " ", cmdCustomer.MiddleName + " ", cmdCustomer.LastName) }).FirstOrDefault().TransactionDoneBy.ToString(),

                                ////Type of the Entity who did the transaction (CMDCustomer , CMDPractice)
                                TransactionAssociatedWith = context.CMDAccounts
                                .Where(cmdAccount => cmdAccount.IsActive &&
                                cmdAccount.Id == transactionProductMap.CMDTransaction.CMDAccountID &&
                                cmdAccount.CMDPracticeID == null)
                                .Any() ?
                                ALPHAEON.CMD.Common.Constants.LinqQueries.TransactionAssociatedwithCMDCustomer : ALPHAEON.CMD.Common.Constants.LinqQueries.TransactionAssociatedwithCMDPractice,

                                ////Information as List of SourceRecords of Entity (CMDCustomer/CMDPractice) involved in Transaction 
                                CMDDashboardSourceRecordList =
                                context.CMDAccounts
                                .Where(cmdAccount => cmdAccount.IsActive &&
                                cmdAccount.Id == transactionProductMap.CMDTransaction.CMDAccountID &&
                                cmdAccount.CMDPracticeID == null)
                                .Join(context.CMDCustomerAccountMaps, recordAccount => recordAccount.Id, recordCustomerAccountMap => recordCustomerAccountMap.CMDAccountID, (recordAccount, recordCustomerAccountMap) => recordCustomerAccountMap)
                                .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive && cmdCustomerAccountMap.CMDCustomer.IsActive)
                                .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomer)
                                .Join(context.CMDCustomerSourceTracks, cmdCustomer => cmdCustomer.Id, cmdCustomerSourceTrack => cmdCustomerSourceTrack.CMDCustomerID, (cmdCustomer, cmdCustomerSourceTrack) => cmdCustomerSourceTrack)
                                .Where(cmdCustomerSourceTrack => cmdCustomerSourceTrack.IsActive).Select(cmdCustomerSourceTrack => new CMDDashboardSourceRecord
                                {
                                    CMDBusinessUnitName = context.CMDBusinessUnits.Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                    cmdBusinessUnit.Id == cmdCustomerSourceTrack.BusinessUnitID).FirstOrDefault().Name,

                                    SourceRecordID = cmdCustomerSourceTrack.SourceRecordID,

                                    CreatedDate = cmdCustomerSourceTrack.CreatedDate,

                                    ////CMD ID of Customer Record
                                    CMDRecordID = cmdCustomerSourceTrack.CMDCustomerID,

                                    AlphaeonId = cmdCustomerSourceTrack.AlphaeonId
                                })
                                .Concat(context.CMDAccounts
                                    .Where(cmdAccount => cmdAccount.IsActive &&
                                    cmdAccount.Id == transactionProductMap.CMDTransaction.CMDAccountID &&
                                    cmdAccount.CMDPracticeID != null)
                                    .Select(cmdAccount => cmdAccount.CMDPractice)
                                    .Where(cmdPractice => cmdPractice.IsActive)
                                    .Join(context.CMDPracticeSourceTracks, cmdPractice => cmdPractice.Id, cmdPracticeSourceTrack => cmdPracticeSourceTrack.CMDPracticeID, (cmdPractice, cmdPracticeSourceTrack) => cmdPracticeSourceTrack)
                                    .Where(cmdPracticeSourceTrack => cmdPracticeSourceTrack.IsActive).Select(cmdPracticeSourceTrack => new CMDDashboardSourceRecord
                                    {
                                        CMDBusinessUnitName = context.CMDBusinessUnits.Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                        cmdBusinessUnit.Id == cmdPracticeSourceTrack.BusinessUnitID).FirstOrDefault().Name,

                                        SourceRecordID = cmdPracticeSourceTrack.SourceRecordID,

                                        CreatedDate = cmdPracticeSourceTrack.CreatedDate,

                                        ////CMD ID of Practice Record
                                        CMDRecordID = cmdPracticeSourceTrack.CMDPracticeID,

                                        AlphaeonId = ""

                                    })).Distinct(),
                            });
        }

        private static void CreateWhereContidion(ref string whereCondition, ref IQueryable<CMDDashboardProductTransaction> cmdDashboardBusinessUnitTransactionList)
        {
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

                    cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.Where(t => t.TxDate >= startDate && t.TxDate <= endDate);
                }
                #endregion

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardBusinessUnitTransactionList = cmdDashboardBusinessUnitTransactionList.Where(whereCondition);
                }
            }
            #endregion Where Condition
        }

        #endregion Private Methods
    }
}
