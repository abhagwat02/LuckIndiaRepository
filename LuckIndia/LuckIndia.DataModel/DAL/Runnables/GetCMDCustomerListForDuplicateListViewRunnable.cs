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
    internal sealed class GetCMDCustomerListForDuplicateListViewRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            ////check if search string contains ;amp; , then replace with &  
            whereCondition = whereCondition.ToggleAmpersand();

            int duplicateCustomerId = 0;

            if (!string.IsNullOrEmpty(queryParameters))
            {
                duplicateCustomerId = Convert.ToInt32(queryParameters);
            }

            context.Configuration.LazyLoadingEnabled = false;
            IQueryable<CMDDashboardCustomer> customerList = null;
            List<CMDDashboardCustomer> customerListResult = new List<CMDDashboardCustomer>();
            int countCustomer = 0;

            try
            {
                #region Data for Duplicate Customer List View
                customerList =

                                    from cmdCust in context.CMDCustomers
                                    select new CMDDashboardCustomer()
                                    {
                                        CustomerID = cmdCust.Id,
                                        FirstName = cmdCust.FirstName,
                                        MiddleName = cmdCust.MiddleName,
                                        LastName = cmdCust.LastName,
                                        IsActive = cmdCust.IsActive,
                                        IsDuplicate = cmdCust.IsDuplicate,
                                        FullName = cmdCust.FirstName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty) + " "
                                                   + (string.IsNullOrEmpty(cmdCust.MiddleName) ? string.Empty : cmdCust.MiddleName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty) + " ")
                                                   + cmdCust.LastName.ToLower().Trim().Replace(" ", string.Empty).Replace("\"", string.Empty),
                                        OriginalBusinessUnitID = cmdCust.OriginalBusinessUnitID,
                                        OriginalBusinessUnitName = context.CMDBusinessUnits
                                       .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive && cmdBusinessUnit.Id == cmdCust.OriginalBusinessUnitID)
                                       .Select(recordCMDBusinessUnit => recordCMDBusinessUnit.Name).FirstOrDefault(),

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

                    CMDLogger.LogAudit(string.Format("Obtained the Customer List from CMD DB for view Type Duplicate List View"), ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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