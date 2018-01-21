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
    /// <summary>
    /// This class contains implementation of runnable GET ExecuteList method to retrive number of customers excluding customers having 
    /// CMDCustomerSourceTrack entries for passed list of business units, .
    /// </summary>
    sealed class CMDCustomersExcludingCustomersOfPassedBusinessUnitIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method retrives the customers excluding customers having CMDCustomerSourceTrack entries for passed list of business units.
        /// </summary>
        /// <typeparam name="T">Type of List</typeparam>
        /// <param name="context">CMD DB Context</param>
        /// <param name="whereCondition">Where Condition need to apply on result</param>
        /// <param name="orderByCondition">OrderBy Condition need to apply on result</param>
        /// <param name="skip">Records to be skipped from result</param>
        /// <param name="take">Records to be taken from result</param>
        /// <param name="queryParameters">Comma separated list of Business Unit Ids, which need to be consider as exclude condition</param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            context.Configuration.LazyLoadingEnabled = false;

            List<CMDCustomer> cmdCustomerListResult = new List<CMDCustomer>();
            IQueryable<CMDCustomer> cmdCustomers = null;
            try
            {
                //If query parameter does not containg anything then return else execute
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    string[] parameters = queryParameters.Split(',');

                    List<int> businessUnitIdsList = parameters.Select(Int32.Parse).ToList();

                    //Check -  Customer List not equal to customer list which has sourcerecord entry for businessunitid passed for exclude in queryparameters

                    var customerIdListToBeExcluded = (context.CMDCustomerSourceTracks
                        .Where(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.IsActive == true &&
                        (businessUnitIdsList.Any(businessUnitId => businessUnitId == cmdCustomerSourceTrackRecord.BusinessUnitID)))
                        .Select(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.CMDCustomerID).Distinct()
                        );

                    cmdCustomers = context.CMDCustomers
                        .Where(cmdCustomerRecord => ! customerIdListToBeExcluded.Contains(cmdCustomerRecord.Id))                        
                        .Select(cmdCustomerRecord => cmdCustomerRecord).Distinct();
                    

                    //cmdCustomers =
                    //     context.CMDCustomerSourceTracks
                    //     .Where(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.IsActive == true &&
                    //     (businessUnitIdsList.Any(businessUnitId => businessUnitId != cmdCustomerSourceTrackRecord.BusinessUnitID)))
                    //     .Select(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.CMDCustomer).Distinct();
                }
                else
                {
                    cmdCustomers = context.CMDCustomers;
                }

                if (!string.IsNullOrEmpty(whereCondition))
                {
                    cmdCustomers = cmdCustomers.Where(whereCondition);

                }

                if (!string.IsNullOrEmpty(orderByCondition))
                {
                    cmdCustomers = cmdCustomers.OrderBy(orderByCondition).Skip(skip).Take(take);
                }
                else
                {

                    cmdCustomers = cmdCustomers.OrderBy("Id").Skip(skip).Take(take);
                }

                cmdCustomerListResult = cmdCustomers.ToList();
                CMDLogger.LogAudit(string.Format("Obtained the CMDCustomers excluding those having CMDCustomerSourceTrack record entried for Business Units Ids {0} from CMD", queryParameters)
                , ALPHAEON.CMD.Common.Constants.General.ApplicationName);




            }

            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return cmdCustomerListResult as List<T>;
        }


        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}