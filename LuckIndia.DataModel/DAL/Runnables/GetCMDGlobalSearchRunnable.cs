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
    internal sealed class GetCMDGlobalSearchRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns list of CMDDashboardGlobalSearch objects, as a result of search on Practice and Customers
        /// </summary>
        /// <typeparam name="T">Type of list. CMDDashboardGlobalSearch</typeparam>
        /// <param name="context">CMD DB Context</param>
        /// <param name="whereCondition">Where condition need to apply on result</param>
        /// <param name="orderByCondition">Order by condition need to apply on result</param>
        /// <param name="skip">Number of records need to skip from result</param>
        /// <param name="take">Number of records need to take from result</param>
        /// <param name="queryParameters"></param>
        /// <returns>List of GLobal serach result</returns>
        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            var cmdGlobalSearchList = new List<CMDDashboardGlobalSearchResult>();

            const string CMDEntityName = "CMDEntityName";

            int customerSearchResultCount = 0;
            int practiceSearchResultCount = 0;
            int alternateSkip = 0;
            int alternateTake = 0;

            try
            {
                //// Split query parametes and find entity type(CMDCustomer, CMDPractice ) and serahc condition
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    int entityTypeId = 0;
                    string searchString = string.Empty;
                    if (queryParameters.Contains('|'))
                    {
                        string[] parameters = queryParameters.Split('|');
                        entityTypeId = Convert.ToInt32(parameters[0]);
                        searchString = parameters[1];

                        if (parameters.Length > 2)
                        {
                            alternateSkip = Convert.ToInt32(parameters[2]);
                            alternateTake = Convert.ToInt32(parameters[3]);
                        }
                    }
                    else
                    {
                        searchString = queryParameters;
                    }

                    //// Replcae special characters like "&", "#" to ";amp;", ";tmp;" and vise-versa 
                    whereCondition = whereCondition.ToggleAmpersand();

                    IQueryable<CMDDashboardGlobalSearch> customerSearchQuery = null;
                    IQueryable<CMDDashboardGlobalSearch> practiceSearchQuery = null;
                    var searchStringList = GetSearchTerms(searchString);

                    //// Find query to search customer/practice
                    switch (entityTypeId)
                    {
                        case (int)ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer:
                            customerSearchQuery = GetCustomerSearchQuery(context, searchStringList, whereCondition);
                            break;

                        case (int)ALPHAEON.CMD.Common.Enums.Entity.CMDPractice:
                            practiceSearchQuery = GetPracticeSearchQuery(context, searchStringList, whereCondition);
                            break;

                        default:
                            customerSearchQuery = GetCustomerSearchQuery(context, searchStringList, whereCondition);
                            practiceSearchQuery = GetPracticeSearchQuery(context, searchStringList, whereCondition);
                            break;
                    }

                    customerSearchResultCount = customerSearchQuery != null ? customerSearchQuery.Count() : 0;
                    practiceSearchResultCount = practiceSearchQuery != null ? practiceSearchQuery.Count() : 0;

                    #region OrderBy Condition

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer || entityTypeId == 0)
                        {
                            if (orderByCondition.IndexOf(CMDEntityName, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                if (customerSearchQuery != null)
                                {
                                    customerSearchQuery = GetCustomerListWithOrderByCondition(orderByCondition, skip, take, customerSearchQuery);
                                }
                            }
                            else
                            {
                                if (customerSearchQuery != null)
                                {
                                    customerSearchQuery = customerSearchQuery.OrderBy(orderByCondition).Skip(skip).Take(take);
                                }
                            }

                            if (entityTypeId == 0)
                            {
                                if (practiceSearchQuery != null)
                                {
                                    practiceSearchQuery = practiceSearchQuery.OrderBy(orderByCondition).Skip(skip).Take(take);
                                }
                            }
                            else
                            {
                                if (practiceSearchQuery != null)
                                {
                                    practiceSearchQuery = practiceSearchQuery.OrderBy(CMDEntityName).Skip(alternateSkip).Take(alternateTake);
                                }
                            }
                        }

                        if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDPractice)
                        {
                            if (practiceSearchQuery != null)
                            {
                                practiceSearchQuery = practiceSearchQuery.OrderBy(orderByCondition).Skip(skip).Take(take);
                            }

                            if (customerSearchQuery != null)
                            {
                                customerSearchQuery = customerSearchQuery.OrderBy(CMDEntityName).Skip(alternateSkip).Take(alternateTake);
                            }
                        }
                    }
                    else
                    {
                        if (customerSearchQuery != null)
                        {
                            customerSearchQuery = GetCustomerListWithOrderByCondition(orderByCondition, skip, take, customerSearchQuery);
                        }

                        if (practiceSearchQuery != null)
                        {
                            practiceSearchQuery = practiceSearchQuery.OrderBy(CMDEntityName).Skip(skip).Take(take);
                        }
                    }
                    #endregion OrderBy Condition

                    //// Add seach result, counts to final list
                    cmdGlobalSearchList.Add(
                        new CMDDashboardGlobalSearchResult()
                        {
                            SearchResultCustomerList = customerSearchQuery != null ? customerSearchQuery.ToList() : null,
                            SearchResultPracticeList = practiceSearchQuery != null ? practiceSearchQuery.ToList() : null,
                            CustomerSearchResultCount = customerSearchResultCount,
                            PracticeSearchResultCount = practiceSearchResultCount,
                            SearchResultCount = customerSearchResultCount + practiceSearchResultCount
                        });

                    CMDLogger.LogAudit("Obtained the Global Search result from CMD DB", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return cmdGlobalSearchList as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Method search the practices on given search string and where condition
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="searchStringList">List of string need to search in practice list</param>
        /// <param name="whereCondition">Filter the searched practice list according to specific conditions</param>
        /// <returns>A query to search practice</returns>
        private static IQueryable<CMDDashboardGlobalSearch> GetPracticeSearchQuery(CMDDatabaseContext context, IEnumerable<string> searchStringList, string whereCondition)
        {
            IQueryable<CMDDashboardGlobalSearch> practiceSearchQuery;

            var practiceIdAllTermsSearchQuery = GetPracticeIdSearchQuery(context, searchStringList);

            practiceSearchQuery = context.CMDPractices.Where(cmdPractice => practiceIdAllTermsSearchQuery.Contains(cmdPractice.Id))
                    .Select(cmdPractice => new CMDDashboardGlobalSearch()
                    {
                        CMDRecordID = cmdPractice.Id,
                        CMDEntityName = cmdPractice.PracticeName,
                        CMDRecordType = cmdPractice.IsDuplicate ? ALPHAEON.CMD.Common.Enums.RecordType.Duplicate.ToString() : ALPHAEON.CMD.Common.Enums.RecordType.Existing.ToString(),
                        HasDuplicates = context.CMDPracticeDuplicateMap
                           .Where(recordCMDPracticeDuplicateMap => recordCMDPracticeDuplicateMap.ExistingPracticeID == cmdPractice.Id && recordCMDPracticeDuplicateMap.IsActive && recordCMDPracticeDuplicateMap.ResolveAction == null)
                           .Any(),
                    });

            if (practiceSearchQuery != null)
            {
                practiceSearchQuery = practiceSearchQuery.Distinct();
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    practiceSearchQuery = practiceSearchQuery.Where(whereCondition);
                }
            }

            return practiceSearchQuery;
        }

        /// <summary>
        /// Method search the customers on given search string and where condition
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="searchStringList">List of string need to search in customers list</param>
        /// <param name="whereCondition">Filter the searched customer list according to specific conditions</param>
        /// <returns>A query to search customer</returns>
        private static IQueryable<CMDDashboardGlobalSearch> GetCustomerSearchQuery(CMDDatabaseContext context, IEnumerable<string> searchStringList, string whereCondition)
        {
            IQueryable<CMDDashboardGlobalSearch> customerSearchQuery;
            var customerIdAllTermsSearchQuery = GetCustomerIdSearchQuery(context, searchStringList);
            customerSearchQuery = context.CMDCustomers.Where(cmdCustomer => customerIdAllTermsSearchQuery.Contains(cmdCustomer.Id))
                      .Select(cmdCustomer => new CMDDashboardGlobalSearch()
                      {
                          CMDRecordID = cmdCustomer.Id,
                          CMDEntityName = cmdCustomer.FirstName.Trim() + " " + cmdCustomer.MiddleName.Trim() + " " + cmdCustomer.LastName.Trim(),
                          CMDRecordType = cmdCustomer.IsDuplicate ? ALPHAEON.CMD.Common.Enums.RecordType.Duplicate.ToString() : ALPHAEON.CMD.Common.Enums.RecordType.Existing.ToString(),
                          ShoutScoreTotal = context.CMDShoutScores.Where(cmdShoutScore => cmdShoutScore.CMDCustomerID == cmdCustomer.Id && cmdShoutScore.IsActive).Select(cmdShoutScoreRecord => cmdShoutScoreRecord.Total).FirstOrDefault(),
                          HasDuplicates = context.CMDCustomerDuplicateMap
                                    .Where(recordCMDCustomerDuplicateMap => recordCMDCustomerDuplicateMap.ExistingCustomerID == cmdCustomer.Id && recordCMDCustomerDuplicateMap.IsActive && recordCMDCustomerDuplicateMap.ResolveAction == null)
                                    .Any(),
                      });
            if (customerSearchQuery != null)
            {
                customerSearchQuery = customerSearchQuery.Distinct();
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    customerSearchQuery = customerSearchQuery.Where(whereCondition);
                }
            }

            return customerSearchQuery;
        }

        /// <summary>
        /// Method will remove the ";amp;", ";tmp;" and replace with "&", "#"
        /// </summary>
        /// <param name="searchQueryString">Search string contaning the ";amp;", ";tmp;" </param>
        /// <returns>Return a string to search after replacing the ";amp;", ";tmp;"</returns>
        private static IEnumerable<string> GetSearchTerms(string searchQueryString)
        {
            ////check if search string contains ";amp;", ";tmp;" then replace with "&", "#"
            searchQueryString = searchQueryString.ToggleAmpersand();

            ////Remove SpecialCharacters excluding spaces
            searchQueryString = CommonUtility.RemoveSpecialCharacters(searchQueryString);
            return searchQueryString.Split(' ').Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Method find a list of practice id to filter the searched practice list
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="searchTerms">List of string to search practice</param>
        /// <returns></returns>
        private static IQueryable<int> GetPracticeIdSearchQuery(CMDDatabaseContext context, IEnumerable<string> searchTerms)
        {
            IQueryable<int> practiceIdAllTermsSearchQuery = null;

            foreach (var searchTerm in searchTerms)
            {
                int sourceRecordId;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    bool isNumericSearchString = CommonUtility.IsNumeric(searchTerm, out sourceRecordId);

                    var practiceIdSearchQuery = context.CMDPractices.Where(cmdPractice => cmdPractice.IsActive &&
                                (cmdPractice.PracticeName.Contains(searchTerm) || cmdPractice.Email.Contains(searchTerm))).Select(cmdPractice => cmdPractice.Id);

                    ////Search String matched PublicEmail, Private Email, Address1, ZipCode  of Practice Details
                    practiceIdSearchQuery = practiceIdSearchQuery.Concat(context.CMDPracticeContactMaps
                         .Where(cmdPracticeContactMap => cmdPracticeContactMap.IsActive &&
                                cmdPracticeContactMap.CMDContact.IsActive &&
                                cmdPracticeContactMap.CMDPractice.IsActive && (
                                cmdPracticeContactMap.CMDContact.PublicEmail.Contains(searchTerm) ||
                                cmdPracticeContactMap.CMDContact.PrivateEmail.Contains(searchTerm) ||
                                cmdPracticeContactMap.CMDContact.Address1.Contains(searchTerm) ||
                                cmdPracticeContactMap.CMDContact.ZipCode.Contains(searchTerm))).Select(cmdPracticeContactMap => cmdPracticeContactMap.CMDPracticeID));

                    ////Search String matched with Sourcerecord id of Practice 
                    if (isNumericSearchString)
                    {
                        practiceIdSearchQuery = practiceIdSearchQuery.Concat(context.CMDPracticeSourceTracks
                                .Where(cmdPracticeSourceTrack => cmdPracticeSourceTrack.IsActive &&
                                    cmdPracticeSourceTrack.CMDPractice.IsActive &&
                                    cmdPracticeSourceTrack.SourceRecordID == sourceRecordId)
                                    .Select(cmdPracticeSourceTrack => cmdPracticeSourceTrack.CMDPracticeID));
                    }

                    ////Search String matched Email of Practice Details
                    practiceIdSearchQuery = practiceIdSearchQuery.Concat(context.CMDPracticeEmails
                         .Where(cmdPracticeEmails => cmdPracticeEmails.IsActive &&
                                cmdPracticeEmails.CMDPractice.IsActive &&
                                cmdPracticeEmails.Email.Contains(searchTerm))
                                .Select(cmdPracticeEmails => cmdPracticeEmails.CMDPracticeID));

                    if (practiceIdAllTermsSearchQuery == null)
                    {
                        practiceIdAllTermsSearchQuery = practiceIdSearchQuery;
                    }
                    else
                    {
                        practiceIdAllTermsSearchQuery = practiceIdAllTermsSearchQuery.Concat(practiceIdSearchQuery);
                    }
                }
            }

            return practiceIdAllTermsSearchQuery;
        }

        /// <summary>
        /// Method find a list of customer id to filter the searched practice list
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="searchTerms">List of string to search customers</param>
        /// <returns></returns>
        private static IQueryable<int> GetCustomerIdSearchQuery(CMDDatabaseContext context, IEnumerable<string> searchTerms)
        {
            IQueryable<int> customerIdAllTermsSearchQuery = null;
            foreach (var searchTerm in searchTerms)
            {
                int sourceRecordId;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    bool isNumericSearchString = CommonUtility.IsNumeric(searchTerm, out sourceRecordId);

                    ////Search String matched with FN, LN ,Email of Customer obj
                    var customerIdSearchQuery = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.IsActive &&
                            (cmdCustomer.FirstName.Contains(searchTerm) ||
                            cmdCustomer.LastName.Contains(searchTerm) ||
                            cmdCustomer.Email.Contains(searchTerm))).Select(cmdCustomer => cmdCustomer.Id);

                    ////Search String matched PublicEmail, Private Email, Address1, ZipCode of Customer Details
                    customerIdSearchQuery = customerIdSearchQuery.Concat(context.CMDCustomerContactMaps
                         .Where(cmdCustomerContactMap => cmdCustomerContactMap.IsActive &&
                                cmdCustomerContactMap.CMDContact.IsActive &&
                                cmdCustomerContactMap.CMDCustomer.IsActive && (
                                cmdCustomerContactMap.CMDContact.PublicEmail.Contains(searchTerm) ||
                                cmdCustomerContactMap.CMDContact.PrivateEmail.Contains(searchTerm) ||
                                cmdCustomerContactMap.CMDContact.Address1.Contains(searchTerm) ||
                                cmdCustomerContactMap.CMDContact.ZipCode.Contains(searchTerm))).Select(cmdCustomerContactMap => cmdCustomerContactMap.CMDCustomerID));

                    ////Search String matched with Sourcerecord id of Customer 
                    if (isNumericSearchString)
                    {
                        customerIdSearchQuery = customerIdSearchQuery.Concat(context.CMDCustomerSourceTracks
                                .Where(cmdCustomerSourceTrack => cmdCustomerSourceTrack.IsActive &&
                                    cmdCustomerSourceTrack.CMDCustomer.IsActive &&
                                    cmdCustomerSourceTrack.SourceRecordID == sourceRecordId).Select(cmdCustomerSourceTrack => cmdCustomerSourceTrack.CMDCustomerID));
                    }

                    ////Search String matched Email of Customer Details
                    customerIdSearchQuery = customerIdSearchQuery.Concat(context.CMDCustomerEmails
                         .Where(cmdCustomerEmails => cmdCustomerEmails.IsActive &&
                                cmdCustomerEmails.CMDCustomer.IsActive &&
                                cmdCustomerEmails.Email.Contains(searchTerm)).Select(cmdCustomerEmails => cmdCustomerEmails.CMDCustomerID));

                    if (customerIdAllTermsSearchQuery == null)
                    {
                        //// If search string contain only one string and no space OR if space and multiple string present then considering result of first 
                        customerIdAllTermsSearchQuery = customerIdSearchQuery;
                    }
                    else
                    {
                        //// If search string contains space and multiple string present then considering result  
                        customerIdAllTermsSearchQuery = customerIdAllTermsSearchQuery.Concat(customerIdSearchQuery);
                    }
                }
            }

            return customerIdAllTermsSearchQuery;
        }

        /// <summary>
        /// This method returns list of customer search result ordered by condition passed as param
        /// </summary>
        /// <param name="orderByCondition">string order by condition</param>
        /// <param name="skip">Number of records to be skipped from the list of result</param>
        /// <param name="take">Number of records to be taken from the list of result</param>
        /// <param name="searchResultCustomerList">Search result list where order by conditon need to ve aplied</param>
        /// <returns>list of customer search result ordered by condition passed as param</returns>
        private static IQueryable<CMDDashboardGlobalSearch> GetCustomerListWithOrderByCondition(string orderByCondition, int skip, int take, IQueryable<CMDDashboardGlobalSearch> searchResultCustomerList)
        {
            if (string.IsNullOrEmpty(orderByCondition))
            {
                orderByCondition = "CMDEntityName asc";
            }

            string[] orderByArray = orderByCondition.Trim().Split(' ');

            if (orderByArray.Count() > 1)
            {
                string order = orderByArray[1].ToLower().Trim();
                if (order.Equals("asc"))
                {
                    searchResultCustomerList = searchResultCustomerList.OrderByDescending(cmdSearchResult => cmdSearchResult.ShoutScoreTotal).ThenBy(cmdSearchResult => cmdSearchResult.CMDEntityName)
                        .Skip(skip).Take(take);
                }

                if (order.Equals("desc"))
                {
                    searchResultCustomerList = searchResultCustomerList.OrderByDescending(cmdSearchResult => cmdSearchResult.ShoutScoreTotal).ThenByDescending(cmdSearchResult => cmdSearchResult.CMDEntityName)
                        .Skip(skip).Take(take);
                }
            }

            return searchResultCustomerList;
        }

        #endregion Private Methods
    }
}