using System;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDProductListByPracticeIdRunnable : BaseApiController, IRunnable
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
            string[] ids = queryParameters.Split(',');

            List<CMDDashboardProductAndContact> customerProductContactListResult = null;
            int practiceId = Convert.ToInt32(ids[0]);
            int businessUnitId = Convert.ToInt32(ids[1]);

            try
            {
                IQueryable<CMDDashboardProductAndContact> cmdDashboardCustomerProductAndContactList = CreateSelectQuery(context, practiceId, businessUnitId);

                customerProductContactListResult = cmdDashboardCustomerProductAndContactList.ToList();
                CMDLogger.LogAudit("Obtained the Product List by Practice ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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
        /// Create a query to fetch products by provided practice id and product line
        /// </summary>
        /// <param name="context">Datacontext</param>
        /// <param name="practiceId">CMD practice id</param>
        /// <param name="businessUnitId">Product line id</param>
        /// <returns>Query to fetch products </returns>
        private static IQueryable<CMDDashboardProductAndContact> CreateSelectQuery(CMDDatabaseContext context, int practiceId, int businessUnitId)
        {
            return from cmdPractice in context.CMDPractices.Where(practice => practice.Id == practiceId && practice.IsActive)
                   select new CMDDashboardProductAndContact()
                   {
                       cmdDashboardProductList = context.CMDAccounts
                                                 .Where(cmdAccount => cmdAccount.CMDPracticeID == practiceId && cmdAccount.IsActive)
                                                 .Select(account => account.CMDTransactions)
                                                 .SelectMany(transact => transact)
                                                 .Where(cmdTransaction => cmdTransaction.IsActive)
                                                 .Join(context.CMDTransactionProductMaps, recTransaction => recTransaction.Id,
                                                 cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                                 (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                                                 .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                                                 .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                                                 .Where(cmdProduct => cmdProduct.CMDBusinessUnitID == businessUnitId
                                                  && cmdProduct.IsActive)
                                                 .Select(product => new CMDDashboardProduct()
                                                 {
                                                     Id = product.Id,

                                                     Name = product.Name,

                                                     CMDProductTypeID = product.CMDProductTypeID,

                                                     CMDBusinessUnitID = product.CMDBusinessUnitID,

                                                     SKU = product.SKU,

                                                     IsActive = product.IsActive,

                                                     CreatedDate = product.CreatedDate,

                                                     UpdatedDate = product.UpdatedDate,

                                                     UpdatedBy = product.UpdatedBy,

                                                     CMDBusinessUnit = product.CMDBusinessUnit,

                                                     CMDProductType = product.CMDProductType
                                                 }).Distinct(),

                       cmdDashboardProductContactsList = context.CMDAccounts
                                                    .Where(i => i.CMDPracticeID == practiceId && i.CMDBusinessUnitId == businessUnitId
                                                     && i.IsActive)
                                                    .Select(account => account)
                                                    .Join(context.CMDAccountContactMaps, recAccount => recAccount.Id,
                                                    cmdAccountContactMap => cmdAccountContactMap.CMDAccountID,
                                                    (recAccount, cmdAccountContactMap) => cmdAccountContactMap.CMDContact)
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
