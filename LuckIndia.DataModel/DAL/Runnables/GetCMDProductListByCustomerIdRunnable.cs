using System;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDProductListByCustomerIdRunnable : BaseApiController, IRunnable
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

            string[] arrayOfIds = queryParameters.Split(',');
            List<CMDDashboardProductAndContact> customerProductContactListResult = null;

            int customerId = Convert.ToInt32(arrayOfIds[0]);
            int businessUnitId = Convert.ToInt32(arrayOfIds[1]);

            try
            {
                IQueryable<CMDDashboardProductAndContact> cmdDashboardCustomerProductAndContactList = CreateSelectQuery(context, customerId, businessUnitId);

                customerProductContactListResult = cmdDashboardCustomerProductAndContactList.ToList();
                CMDLogger.LogAudit("Obtained the Product List by Customer ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerProductContactListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Create a query to fetch products by provided customer id
        /// </summary>
        /// <param name="context">Datacontext</param>
        /// <param name="customerId">CMD customer id</param>
        /// <param name="businessUnitId">Product line id</param>
        /// <returns>Query to fetch products </returns>
        private static IQueryable<CMDDashboardProductAndContact> CreateSelectQuery(CMDDatabaseContext context, int customerId, int businessUnitId)
        {
            return from cmdCustomer in context.CMDCustomers
                             .Where(cust => cust.Id == customerId && cust.IsActive)
                   select new CMDDashboardProductAndContact()
                   {
                       cmdDashboardProductList =
                           context.CMDCustomers
                           .Where(cmdCustomerRecord => cmdCustomerRecord.Id == customerId && cmdCustomerRecord.IsActive)
                           .Join(context.CMDCustomerAccountMaps, recCustomer => recCustomer.Id,
                           cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                           (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                           .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                               cmdCustomerAccountMap.CMDAccount.IsActive)
                           .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                           .SelectMany(transact => transact)
                           .Where(cmdTransaction => cmdTransaction.IsActive)
                           .Join(context.CMDTransactionProductMaps, recTransaction => recTransaction.Id,
                           cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                           (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                           .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                           .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                           .Where(cmdProduct => cmdProduct.CMDBusinessUnitID == businessUnitId && cmdProduct.IsActive)
                           .Select(product => new CMDDashboardProduct()
                           {
                               Id = product.Id,

                               Name = product.Name,

                               CMDProductTypeID = product.CMDProductTypeID,

                               CMDBusinessUnitID = product.CMDBusinessUnitID,

                               SKU = product.SKU,

                               IsActive = product.IsActive,

                               CMDBusinessUnit = product.CMDBusinessUnit,

                               CMDProductType = product.CMDProductType,
                           }).Distinct(),

                       cmdDashboardProductContactsList =
                       context.CMDCustomers
                        .Where(cmdCustomerRecord => cmdCustomerRecord.Id == customerId && cmdCustomerRecord.IsActive)
                        .Join(context.CMDCustomerAccountMaps, recCustomer => recCustomer.Id,
                        cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                        (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                        .Where(customerAccountMap => customerAccountMap.CMDBusinessUnitID == businessUnitId && customerAccountMap.IsActive)
                        .Select(customerAccountMap => customerAccountMap.CMDAccount)
                        .Where(recAccount => recAccount.IsActive)
                        .Select(account => account)
                        .Join(context.CMDAccountContactMaps, recAccount => recAccount.Id,
                        cmdAccountContactMap => cmdAccountContactMap.CMDAccountID,
                        (recAccount, cmdAccountContactMap) => cmdAccountContactMap)
                        .Where(cmdAccountContactMap => cmdAccountContactMap.IsActive)
                        .Select(cmdAccountContactMap => cmdAccountContactMap.CMDContact)
                        .Where(recContact => recContact.IsActive && context.CMDContactSourceTracks
                            .Where(cmdContactSourceTrack => cmdContactSourceTrack.IsActive &&
                                cmdContactSourceTrack.CMDContactID == recContact.Id &&
                                cmdContactSourceTrack.ContactTypeID != ((int)ALPHAEON.CMD.Common.Enums.CMDContactType.ShippingAddress)).Any())
                        .Distinct()
                   };
        }
        #endregion Private Methods
    }
}
