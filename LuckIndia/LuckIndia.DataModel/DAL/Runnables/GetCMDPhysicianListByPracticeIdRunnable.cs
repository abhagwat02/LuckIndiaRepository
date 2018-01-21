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
    internal sealed class GetCMDPhysicianListByPracticeIdRunnable : BaseApiController, IRunnable
    {
        #region Public Methods
        void IRunnable.Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;
            List<CMDDashboardPracticePhysician> physicianListResult = null;

            int practiceId = Convert.ToInt32(queryParameters);

            if (practiceId <= 0)
            {
                return physicianListResult as List<T>;
            }

            try
            {
                IQueryable<CMDDashboardPracticePhysician> physicianList = CreateSelectQuery(context, practiceId);

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    physicianList = physicianList.Where(whereCondition);
                }

                var countCustomer = physicianList.Count();
                take = take == 0 ? countCustomer : take;

                orderByCondition = !string.IsNullOrEmpty(orderByCondition) ? orderByCondition : "ProductsCount";
                physicianList = physicianList.OrderBy(orderByCondition).Skip(skip).Take(take);

                physicianListResult = physicianList.ToList();
                CMDLogger.LogAudit("Obtained the Physician List by Practice ID from CMD", ALPHAEON.CMD.Common.Constants.General.ApplicationName);
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return physicianListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Create query to get the list of physicians by selecte practice
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="practiceId">Selected practice id</param>
        /// <returns>query to fetch the list of physicians</returns>
        private static IQueryable<CMDDashboardPracticePhysician> CreateSelectQuery(CMDDatabaseContext context, int practiceId)
        {
            return from cmdPractice in context.CMDPractices
                   where cmdPractice.Id == practiceId && cmdPractice.IsActive
                   join cmdCustomerPracticeMap in context.CMDCustomerPracticeMaps
                   on cmdPractice.Id equals cmdCustomerPracticeMap.CMDPracticeID
                   where cmdCustomerPracticeMap.IsActive && cmdCustomerPracticeMap.CMDCustomer.CMDCustomerType ==
                                              (from cmdCustomer in context.CMDCustomers
                                               where cmdCustomer.CMDCustomerType.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDCustomerTypeNamePhysician &&
                                                                                                       cmdCustomer.IsActive && !cmdCustomer.IsDuplicate
                                               select cmdCustomer.CMDCustomerType).FirstOrDefault()
                   select new CMDDashboardPracticePhysician()
                   {
                       Id = cmdCustomerPracticeMap.CMDCustomerID,

                       IsActive = cmdCustomerPracticeMap.CMDCustomer.IsActive,

                       PhysicianFirstName = cmdCustomerPracticeMap.CMDCustomer.FirstName,

                       PhysicianMiddleName = cmdCustomerPracticeMap.CMDCustomer.MiddleName,

                       PhysicianLastName = cmdCustomerPracticeMap.CMDCustomer.LastName,

                       //// Get the list of speciality for current practice
                       CMDSpecialityList = (from cmdCustomerSpecialityMap in context.CMDCustomerSpecialityMaps
                                            where cmdCustomerPracticeMap.CMDCustomer.Id == cmdCustomerSpecialityMap.CMDCustomerID &&
                                            cmdCustomerSpecialityMap.IsActive &&
                                            cmdCustomerSpecialityMap.CMDSpeciality.IsActive &&
                                            cmdCustomerSpecialityMap.CMDCustomer.IsActive
                                            select new CMDDashboardSpeciality
                                            {
                                                Id = cmdCustomerSpecialityMap.CMDSpeciality.Id,
                                                IsActive = cmdCustomerSpecialityMap.CMDSpeciality.IsActive,
                                                Name = cmdCustomerSpecialityMap.CMDSpeciality.Name,
                                                ParentSpecialityID = cmdCustomerSpecialityMap.CMDSpeciality.ParentSpecialityID
                                            }).Distinct(),

                       //// Get the praduct count for current practice
                       ProductsCount = context.CMDCustomers
                                           .Where(cmdCustomer => cmdCustomer.Id == cmdCustomerPracticeMap.CMDCustomer.Id && cmdCustomer.IsActive)
                                           .Join(
                                           context.CMDCustomerAccountMaps,
                                           recCustomer => recCustomer.Id,
                                           cmdCustomerAccountMap => cmdCustomerAccountMap.CMDCustomerID,
                                           (recCustomer, cmdCustomerAccountMap) => cmdCustomerAccountMap)
                                           .Where(cmdCustomerAccountMap => cmdCustomerAccountMap.IsActive &&
                                               cmdCustomerAccountMap.CMDAccount.IsActive)
                                           .Select(cmdCustomerAccountMap => cmdCustomerAccountMap.CMDAccount.CMDTransactions)
                                           .SelectMany(transact => transact)
                                           .Where(transact => transact.IsActive)
                                           .Join(
                                           context.CMDTransactionProductMaps,
                                           recTransaction => recTransaction.Id,
                                           cmdTransactionProductMap => cmdTransactionProductMap.CMDTransactionID,
                                           (recTransaction, cmdTransactionProductMap) => cmdTransactionProductMap)
                                           .Where(cmdTransactionProductMap => cmdTransactionProductMap.IsActive)
                                           .Select(cmdTransactionProductMap => cmdTransactionProductMap.CMDProduct)
                                           .Where(cmdProduct => cmdProduct.IsActive)
                                           .Select(product => product.Id).Distinct().Count(),

                       Email = cmdCustomerPracticeMap.CMDCustomer.Email,

                       Phone = cmdCustomerPracticeMap.CMDCustomer.Phone,
                   };
        }

        #endregion Private Methods
    }
}