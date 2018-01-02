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
    internal sealed class GetCMDSourceRecordListByPracticeIdRunnable : BaseApiController, IRunnable
    {
        #region Public Methods

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns a list of CMDDashboardSourceRecord for the practice whose Id is passed. 
        /// </summary>
        /// <typeparam name="T">Type of object returned by RPC, here CMDDashboardSourceRecord</typeparam>
        /// <param name="context">Obj. of CMDDatabaseContext</param>
        /// <param name="whereCondition">where condition string</param>
        /// <param name="orderByCondition">order by condition string</param>
        /// <param name="skip">No. of records to be skipped</param>
        /// <param name="take">No. of records to be taken</param>
        /// <param name="queryParameters">PracticeId</param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            context.Configuration.LazyLoadingEnabled = false;
            List<CMDDashboardSourceRecord> cmdPracticedSourceRecordListResult = null;
            try
            {
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    int practiceId = Convert.ToInt32(queryParameters);

                    if (practiceId <= 0)
                    {
                        return cmdPracticedSourceRecordListResult as List<T>;
                    }

                    IQueryable<CMDDashboardSourceRecord> cmdDashboardSourceRecordList = CreateSelectQuery(context, practiceId);

                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        cmdDashboardSourceRecordList = cmdDashboardSourceRecordList.Where(whereCondition);
                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        cmdDashboardSourceRecordList = cmdDashboardSourceRecordList.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {
                        cmdDashboardSourceRecordList = cmdDashboardSourceRecordList.OrderBy("CreatedDate").Skip(skip).Take(take);
                    }

                    cmdPracticedSourceRecordListResult = cmdDashboardSourceRecordList.ToList();

                    CMDLogger.LogAudit(string.Format("Obtained the CMD Source Track Record List by {0} ID from CMD", ALPHAEON.CMD.Common.Enums.Entity.CMDPractice), ALPHAEON.CMD.Common.Constants.General.ApplicationName);
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return cmdPracticedSourceRecordListResult as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// This method return a query to select customer source track 
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="practiceId">Practice id to get source track</param>
        /// <returns>Return a query to get practice source track</returns>
        private static IQueryable<CMDDashboardSourceRecord> CreateSelectQuery(CMDDatabaseContext context, int practiceId)
        {
            return context.CMDPracticeSourceTracks.Where(cmdPracticeSourceTrack => cmdPracticeSourceTrack.IsActive &&
                                                                cmdPracticeSourceTrack.CMDPracticeID == practiceId)
                                                                .Select(cmdPracticeSourceTrack => new CMDDashboardSourceRecord
                                                                {
                                                                    CMDBusinessUnitName = context.CMDBusinessUnits
                                                                    .Where(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                                                                        cmdBusinessUnit.Id == cmdPracticeSourceTrack.BusinessUnitID).FirstOrDefault().Name,
                                                                    SourceRecordID = cmdPracticeSourceTrack.SourceRecordID,
                                                                    CreatedDate = cmdPracticeSourceTrack.CreatedDate
                                                                }).Distinct();
        }

        #endregion Private Methods
    }
}
