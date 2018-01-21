using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class GetCMDProductByProductIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {

            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;

            List<CMDDashboardProduct> productListResult = null;

            int productID = Convert.ToInt32(queryParameters);

            try
            {
                IQueryable<CMDDashboardProduct> cmdDashboardProductList =
                         (context.CMDProducts
                         .Where(cmdProduct => cmdProduct.Id == productID && cmdProduct.IsActive == true)
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

                             CMDProductType = product.CMDProductType,

                             CMDProductTypeName = product.CMDProductType.TypeName,

                             CMDBusinessUnitName = product.CMDBusinessUnit.Name

                         })
                 );

                productListResult = cmdDashboardProductList.ToList();
                CMDLogger.LogAudit("Obtained the Product List by Product ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }
            return productListResult as List<T>;
        }


        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}
