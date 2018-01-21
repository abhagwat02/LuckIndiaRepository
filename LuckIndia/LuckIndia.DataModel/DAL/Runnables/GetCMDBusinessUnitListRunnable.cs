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
    sealed class GetCMDBusinessUnitListRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            List<CMDDashboardBusinessUnit> businessUnitListResult = null;
            context.Configuration.LazyLoadingEnabled = false;
            try
            {
                IQueryable<CMDDashboardBusinessUnit> businessUnitList =

                    (from cmdBusinessUnit in context.CMDBusinessUnits

                     select new CMDDashboardBusinessUnit()
                     {
                         Id = cmdBusinessUnit.Id,

                         ContactPerson = cmdBusinessUnit.ContactPerson,

                         Description = cmdBusinessUnit.Description,

                         CMDContact = cmdBusinessUnit.CMDContact,

                         CMDContactID = cmdBusinessUnit.CMDContactID,

                         Name = cmdBusinessUnit.Name,

                         WebURL = cmdBusinessUnit.CMDContact.WebsiteURL,

                         CustomerSupportNumber = cmdBusinessUnit.CMDContact.Phone,

                         CustomerSupportEmail = cmdBusinessUnit.CMDContact.PublicEmail,

                         UpdatedDate = cmdBusinessUnit.UpdatedDate,

                         IsActive = cmdBusinessUnit.IsActive,

                         CreatedDate = cmdBusinessUnit.CreatedDate,

                         UpdatedBy = cmdBusinessUnit.UpdatedBy,

                         BusinessUnitType = cmdBusinessUnit.BusinessUnitType,

                         CMDBusinessUnitProductListCount =
                                                    (context.CMDBusinessUnits)
                                                    .Where(businessUnit => businessUnit.Id == (cmdBusinessUnit.Id) && businessUnit.IsActive == true)
                                                    .SelectMany(recordBusinessUnit => recordBusinessUnit.CMDProducts).Where(cmdProductRecord => cmdProductRecord.IsActive == true).Distinct().Count(),

                     });


                if (!string.IsNullOrEmpty(whereCondition))
                {
                    businessUnitList = businessUnitList.Where(whereCondition);

                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    businessUnitList = businessUnitList.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {

                    businessUnitList = businessUnitList.OrderBy("Id").Skip(skip).Take(take);
                }

                businessUnitListResult = businessUnitList.ToList();
                CMDLogger.LogAudit("Obtained the BusinessUnit List from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }
            return businessUnitListResult as List<T>;
        }





        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}