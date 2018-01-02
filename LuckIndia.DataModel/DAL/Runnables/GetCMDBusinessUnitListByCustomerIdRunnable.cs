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
    internal sealed class GetCMDBusinessUnitListByCustomerIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            context.Configuration.LazyLoadingEnabled = false;
            List<CMDDashboardCustomerBusinessUnit> customerBusinessUnitListResult = null;

            int customerId = Convert.ToInt32(queryParameters);

            if (customerId <= 0)
            {
                return customerBusinessUnitListResult as List<T>;
            }

            try
            {
                //// Get the Product Lines for the selected Customer
                //// Also get the transaction date for the product line


                IQueryable<CMDDashboardCustomerBusinessUnit> cmdDashboardCustomerBusinessUnitList =
                context.CMDCustomers
                        .Where(cmdCustomer => cmdCustomer.Id == customerId && cmdCustomer.IsActive)
                        .Join(
                        context.CMDCustomerAccountMaps,
                        recCustomer => recCustomer.Id,
                        cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                        (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                        .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                            cmdCustomerAccountMap.CMDAccount.IsActive)
                        .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDBusinessUnit)
                        .Select(cmdBusinessUnit => new CMDDashboardCustomerBusinessUnit
                        {
                            CustomerSupportEmail = cmdBusinessUnit.CMDContact.PublicEmail,
                            CustomerSupportNumber = cmdBusinessUnit.CMDContact.Phone,
                            #region Getting first transaction date for customer
                            FirstTransactionDate = context.CMDCustomers
                                           .Where(cmdCustomer => cmdCustomer.Id == customerId && cmdCustomer.IsActive)
                                           .Join(
                                           context.CMDCustomerAccountMaps,
                                           recCustomer => recCustomer.Id,
                                           cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                                           (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                                           .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                               cmdCustomerAccountMap.CMDAccount.IsActive)
                                               .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                                           .SelectMany(transact => transact)
                                           .Where(cmdTransaction => cmdTransaction.IsActive).ToList().OrderBy(transact => transact.TxDate).FirstOrDefault().TxDate,
                            #endregion
                            Id = cmdBusinessUnit.Id,
                            IsActive = cmdBusinessUnit.IsActive,
                            Name = cmdBusinessUnit.Name,
                            WebURL = cmdBusinessUnit.CMDContact.WebsiteURL,
                            ImageURL = cmdBusinessUnit.ImageURL,
                            CMDContact = cmdBusinessUnit.CMDContact,
                            CMDContactID = cmdBusinessUnit.CMDContactID,
                        }).Distinct();

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardCustomerBusinessUnitList = cmdDashboardCustomerBusinessUnitList.Where(whereCondition);
                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdDashboardCustomerBusinessUnitList = cmdDashboardCustomerBusinessUnitList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {
                    cmdDashboardCustomerBusinessUnitList = cmdDashboardCustomerBusinessUnitList.OrderBy("Id").Skip(skip).Take(take);
                }

                customerBusinessUnitListResult = cmdDashboardCustomerBusinessUnitList.ToList();
                CMDLogger.LogAudit("Obtained the BusinessUnit List by Customer ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerBusinessUnitListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}