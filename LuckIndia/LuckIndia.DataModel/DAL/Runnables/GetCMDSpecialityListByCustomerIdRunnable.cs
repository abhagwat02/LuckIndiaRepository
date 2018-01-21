using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    internal sealed class GetCMDSpecialityListByCustomerIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;

            int customerId = Convert.ToInt32(queryParameters);
            
            List<CMDSpeciality> customerSpecialityListResult = null;
            try
            {
                IQueryable<CMDSpeciality> cmdDashboardCustomerSpecialityList =
                    context.CMDCustomerSpecialityMaps
                     .Where(recCMDCustomerSpecialityMap => recCMDCustomerSpecialityMap.IsActive == true &&
                     recCMDCustomerSpecialityMap.CMDCustomerID == customerId &&
                     recCMDCustomerSpecialityMap.CMDCustomer.IsActive == true)
                     .Select(recordCustomerSpecialityMap => recordCustomerSpecialityMap.CMDSpeciality);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdDashboardCustomerSpecialityList = cmdDashboardCustomerSpecialityList.Where(whereCondition);
                }

                var countSpeciality = cmdDashboardCustomerSpecialityList.Count();

                if (take == 0)
                {
                    take = countSpeciality;
                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdDashboardCustomerSpecialityList = cmdDashboardCustomerSpecialityList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {
                    cmdDashboardCustomerSpecialityList = cmdDashboardCustomerSpecialityList.OrderBy("Id").Skip(skip).Take(take);
                }

                customerSpecialityListResult = cmdDashboardCustomerSpecialityList.ToList();
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return customerSpecialityListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}
