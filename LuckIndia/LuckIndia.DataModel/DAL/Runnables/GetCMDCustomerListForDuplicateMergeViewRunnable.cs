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
    internal sealed class GetCMDCustomerListForDuplicateMergeViewRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            ////check if search string contains ;amp; , then replace with &  
            whereCondition = whereCondition.ToggleAmpersand();

            context.Configuration.LazyLoadingEnabled = false;
            IQueryable<CMDDashboardCustomer> customerList = null;
            List<CMDDashboardCustomer> customerListResult = new List<CMDDashboardCustomer>();
            int countCustomer = 0;

            try
            {
                #region Data for Existing Customer on Merge with Existing Customer View

                customerList =

                    from cmdCust in context.CMDCustomers
                    select new CMDDashboardCustomer()
                    {
                        CustomerID = cmdCust.Id,
                        FirstName = cmdCust.FirstName,
                        MiddleName = cmdCust.MiddleName,
                        LastName = cmdCust.LastName,
                        Email = cmdCust.Email,
                        Phone = cmdCust.Phone,
                        DOB = cmdCust.DOB,
                        IsActive = cmdCust.IsActive,
                        FullName = cmdCust.FirstName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty) + " "
                                   + (string.IsNullOrEmpty(cmdCust.MiddleName) ? string.Empty : cmdCust.MiddleName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty) + " ")
                                   + cmdCust.LastName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty),
                        CustomerTypeID = cmdCust.CMDCustomerTypeID,
                        CustomerTypeName = cmdCust.CMDCustomerType.Name,
                        OriginalBusinessUnitID = cmdCust.OriginalBusinessUnitID,
                        OriginalBusinessUnitName = context.CMDBusinessUnits
                        .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == cmdCust.OriginalBusinessUnitID)
                        .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),
                        CMDCustomerEmailList = context.CMDCustomerEmails
                        .Where(cmdCustomerEmail => cmdCustomerEmail.IsActive && cmdCustomerEmail.CMDCustomerID == cmdCust.Id)
                        .Select(cmdCustEmailList => cmdCustEmailList).ToList(),

                        CMDContactList =
                                         from cmdContact in context.CMDContacts
                                         where cmdContact.IsActive
                                         join cmdCustomerContactMap in context.CMDCustomerContactMaps
                                         on cmdContact.Id equals cmdCustomerContactMap.CMDContactID
                                         where cmdCustomerContactMap.CMDCustomerID == cmdCust.Id && cmdCustomerContactMap.IsActive
                                         select new CMDDashboardContact
                                         {
                                             Address1 = cmdContact.Address1,
                                             Address2 = cmdContact.Address2,
                                             City = cmdContact.City,
                                             Cell = cmdContact.Cell,
                                             CMDStateID = cmdContact.CMDState.Id,
                                             CMDStateAbbreviation = cmdContact.CMDState.Abbriviation,
                                             CMDCountryAbbreviation = cmdContact.CMDState.CMDCountry.Abbreviation,
                                             ContactName = cmdContact.ContactName,
                                             CreatedDate = cmdContact.CreatedDate,
                                             Fax = cmdContact.Fax,
                                             Id = cmdContact.Id,
                                             IsDefault = cmdContact.IsDefault,
                                             Phone = cmdContact.Phone,
                                             PrivateEmail = cmdContact.PrivateEmail,
                                             PublicEmail = cmdContact.PublicEmail,
                                             UpdatedDate = cmdContact.UpdatedDate,
                                             WebsiteURL = cmdContact.WebsiteURL,
                                             ZipCode = cmdContact.ZipCode,
                                             CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                             CMDStateName = cmdContact.CMDState.Name
                                         },

                        CMDSpecialityList =
                                          from cmdSpeciality in context.CMDSpecialities
                                          where cmdSpeciality.IsActive
                                          join cmdSpecialityMap in context.CMDCustomerSpecialityMaps
                                          on cmdSpeciality.Id equals cmdSpecialityMap.CMDSpecialityID
                                          where cmdSpecialityMap.CMDCustomerID == cmdCust.Id && cmdSpecialityMap.IsActive
                                          select new CMDDashboardSpeciality
                                          {
                                              CreatedDate = cmdSpeciality.CreatedDate,
                                              Id = cmdSpeciality.Id,
                                              IsActive = cmdSpeciality.IsActive,
                                              Name = cmdSpeciality.Name,
                                              UpdatedBy = cmdSpeciality.UpdatedBy,
                                              UpdatedDate = cmdSpeciality.UpdatedDate
                                          },

                        DuplicateMatchPercent = context.CMDCustomerDuplicateMap.FirstOrDefault(recordCMDCustomerDuplicateMap => recordCMDCustomerDuplicateMap.DuplicateCustomerID == cmdCust.Id && recordCMDCustomerDuplicateMap.IsActive && recordCMDCustomerDuplicateMap.ResolveAction == null).DuplicateMatchPercent,

                        // If the selected customer is having any duplicate record then set this flag to true
                        HasDuplicates = context.CMDCustomerDuplicateMap
                                      .Where(recordCMDCustomerDuplicateMap => recordCMDCustomerDuplicateMap.ExistingCustomerID == cmdCust.Id && recordCMDCustomerDuplicateMap.IsActive && recordCMDCustomerDuplicateMap.ResolveAction == null)
                                      .Any(),

                        IsDuplicate = cmdCust.IsDuplicate,

                        ////If the selected customer is duplicate customer then, give count of Existing Customers for which the selected customer is duplicate

                        ExistingCustomersCountforSelectedDuplicate =
                        context.CMDCustomerDuplicateMap
                        .Where(cmdCustomerDuplicateMap => cmdCustomerDuplicateMap.IsActive && cmdCustomerDuplicateMap.ResolveAction == null
                        && cmdCustomerDuplicateMap.DuplicateCustomerID == cmdCust.Id)
                        .Join(
                        context.CMDCustomers,
                        recordCMDCustomerDuplicateMap => recordCMDCustomerDuplicateMap.ExistingCustomerID,
                        recordCMDCustomer => recordCMDCustomer.Id,
                        (recordCMDCustomerDuplicateMap, recordCMDCustomer) => recordCMDCustomer)
                        .Where(recordCMDCust => recordCMDCust.IsActive).Count(),

                        CMDPracticeList =
                       context.CMDCustomerPracticeMaps
                       .Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == cmdCust.Id &&
                           cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDPractice.IsActive)
                           .Select(recordCMDCustomerPracticeMap => recordCMDCustomerPracticeMap.CMDPractice),

                        CMDProductList =
                        context.CMDCustomers
                            .Where(cmdCustomerRecord => cmdCustomerRecord.Id == cmdCust.Id && cmdCustomerRecord.IsActive)
                            .Join(
                            context.CMDCustomerAccountMaps,
                            recCustomer => recCustomer.Id,
                            cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                            (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                            .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                cmdCustomerAccountMap.CMDAccount.IsActive)
                            .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                            .SelectMany(transact => transact)
                            .Where(cmdTransaction => cmdTransaction.IsActive)
                            .Join(
                            context.CMDTransactionProductMaps,
                            recTransaction => recTransaction.Id,
                            cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                            (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                            .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                            .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                            .Where(cmdProduct => cmdProduct.IsActive).Distinct(),

                        CreatedDate = cmdCust.CreatedDate,
                    };

                #endregion

                if (customerList != null)
                {
                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        #region Search

                        // Find search string
                        string searchString = string.Empty;

                        if (whereCondition.Contains(ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchStartFilter))
                        {
                            int posA = whereCondition.IndexOf(ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchStartFilter);
                            int posB = whereCondition.LastIndexOf(ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchEndFilter);

                            int adjustedPosA = posA + ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchStartFilter.Length;

                            searchString = whereCondition.Substring(adjustedPosA, posB - adjustedPosA);
                            whereCondition = whereCondition.Replace(ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchStartFilter + searchString + ALPHAEON.CMD.Common.Constants.Dashboard.CustomerSearchEndFilter, string.Empty);
                        }

                        // Find the customer by full name...
                        // If customer can not found by full name then find by first name, middle name and last name.
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            string searchCondition = whereCondition + " AND FullName.Contains(\"" + searchString.Trim().ToggleAmpersand().Replace("\"", string.Empty) + "\")";

                            // Find customer by fullname
                            if (customerList.Where(searchCondition).ToList().Count() > 0)
                            {
                                customerList = customerList.Where(searchCondition);
                            }
                            else
                            {
                                // Variable to hold the first, middle and last name
                                string[] nameArray = searchString.Split(' ');
                                string searchByNameCondition = string.Empty;

                                // Create 'OR' condition for first name, middle name and last name
                                if (nameArray.Count() > 0)
                                {
                                    foreach (string name in nameArray)
                                    {
                                        if (!string.IsNullOrEmpty(name))
                                        {
                                            if (!string.IsNullOrEmpty(searchByNameCondition))
                                            {
                                                searchByNameCondition += " OR FullName.Contains(\"" + name.ToggleAmpersand().Trim().Replace("\"", string.Empty) + "\")";
                                            }
                                            else
                                            {
                                                searchByNameCondition = "FullName.Contains(\"" + name.ToggleAmpersand().Trim().Replace("\"", string.Empty) + "\")";
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(searchByNameCondition))
                                {
                                    searchCondition = whereCondition + " AND (" + searchByNameCondition + ")";
                                }
                                else
                                {
                                    searchCondition = whereCondition;
                                }
                            }

                            // Find customer
                            customerList = customerList.Where(searchCondition);
                        }
                        else
                        {
                            customerList = customerList.Where(whereCondition);
                        }

                        #endregion
                    }

                    countCustomer = customerList.Count();
                    if (countCustomer > 0)
                    {
                        if (!string.IsNullOrEmpty(orderByCondition))
                        {
                            if (orderByCondition.Contains("FirstName"))
                            {
                                string[] orderByArray = orderByCondition.Trim().Split(' ');

                                if (orderByArray.Count() > 1)
                                {
                                    string order = orderByArray[1].ToLower().Trim();

                                    if (order.Equals("asc"))
                                    {
                                        customerList = customerList.OrderBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.LastName).Skip(skip).Take(take);
                                    }
                                    else
                                    {
                                        customerList = customerList.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName).ThenByDescending(x => x.LastName).Skip(skip).Take(take);
                                    }
                                }
                            }
                            else
                            {
                                customerList = customerList.OrderBy(orderByCondition).Skip(skip).Take(take);
                            }
                        }
                        else
                        {
                            customerList = customerList.OrderBy("CustomerID").Skip(skip).Take(take);
                        }

                        customerListResult = customerList.ToList();
                    }

                    if (customerListResult != null && customerListResult.Count > 0)
                    {
                        customerListResult.All(m => { m.CustomersCount = countCustomer; return true; });
                    }

                    CMDLogger.LogAudit(string.Format("Obtained the Customer List from CMD DB for view Type Duplicate Merge View"), ALPHAEON.CMD.Common.Constants.General.ApplicationName);
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}