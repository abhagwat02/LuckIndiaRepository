using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDProductListByCMDCustomerIdRunnable : BaseApiController, IRunnable
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

            List<CMDDashboardProduct> customerProductContactListResult = null;

            try
            {
                if (string.IsNullOrEmpty(queryParameters))
                {
                    return customerProductContactListResult as List<T>;
                }

                IQueryable<CMDDashboardProduct> cmdDashboardCustomerProductList = CreateSelectQuery(context, queryParameters);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardCustomerProductList = cmdDashboardCustomerProductList.Where(whereCondition);
                }

                #region Order By Condition
                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "Id";
                cmdDashboardCustomerProductList = cmdDashboardCustomerProductList.OrderBy(orderByCondition).Skip(skip).Take(take);
                #endregion

                customerProductContactListResult = cmdDashboardCustomerProductList.ToList();
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
        /// Create select query to get the list of products
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="queryParameters">Parameters to filter the list</param>
        /// <returns>Query to get products</returns>
        private static IQueryable<CMDDashboardProduct> CreateSelectQuery(CMDDatabaseContext context, string queryParameters)
        {
            return ApplicationServices.LINQQueries.CMDProduct.GetProductsByCustomerId(context, Convert.ToInt32(queryParameters))
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

                                         CMDProductTypeName = product.CMDProductType.TypeName,

                                         CMDBusinessUnitName = product.CMDBusinessUnit.Name,
                                     }).Distinct();
        }

        #endregion Private Methods
    }
}
