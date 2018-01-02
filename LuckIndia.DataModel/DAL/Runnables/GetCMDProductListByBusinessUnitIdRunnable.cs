using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class GetCMDProductListByBusinessUnitIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string Id = null) where T : class
        {
            context.Configuration.LazyLoadingEnabled = false;

            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            int businessUnitID = Convert.ToInt32(Id);

            List<CMDDashboardProduct> customerProductContactListResult = null;

            try
            {
                IQueryable<CMDDashboardProduct> cmdDashboardBusinessUnitProductList =
                                                     (context.CMDBusinessUnits
                                                    .Where(businessUnit => businessUnit.Id == (businessUnitID) && businessUnit.IsActive == true)
                                                    .SelectMany(recordBusinessUnit => recordBusinessUnit.CMDProducts)
                                                    .Select(cmdProduct => new CMDDashboardProduct()
                                                    {
                                                        CMDBusinessUnitID = cmdProduct.CMDBusinessUnitID,
                                                        CMDBusinessUnit = cmdProduct.CMDBusinessUnit,
                                                        CMDProductType = cmdProduct.CMDProductType,
                                                        CMDProductTypeID = cmdProduct.CMDProductTypeID,
                                                        CreatedDate = cmdProduct.CreatedDate,
                                                        Id = cmdProduct.Id,
                                                        IsActive = cmdProduct.IsActive,
                                                        Name = cmdProduct.Name,
                                                        SKU = cmdProduct.SKU,
                                                        UpdatedBy = cmdProduct.UpdatedBy,
                                                        UpdatedDate = cmdProduct.UpdatedDate

                                                    }));

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardBusinessUnitProductList = cmdDashboardBusinessUnitProductList.Where(whereCondition);

                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdDashboardBusinessUnitProductList = cmdDashboardBusinessUnitProductList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {

                    cmdDashboardBusinessUnitProductList = cmdDashboardBusinessUnitProductList.OrderBy("Id").Skip(skip).Take(take);
                }

                customerProductContactListResult = cmdDashboardBusinessUnitProductList.ToList();
                CMDLogger.LogAudit("Obtained the Product List by BusinessUnit ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
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
    }
}
