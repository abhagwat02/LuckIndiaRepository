using System;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Controllers;
using ALPHAEON.CMD.Common;
using ALPHAEON.CMD.TransientMessage;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    /// <summary>
    /// This class contains implementation of runnable GET ExecuteList method to check for entity is present in CMD or not. Enity can be CMDCustomer or CMDPractice.
    /// </summary>
    internal sealed class EntityExistCheckRunnable : BaseApiController, IRunnable
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

            List<EntityExist> entityExistList = new List<EntityExist>();

            try
            {
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    //// Split the comma "," seperated parameters
                    string[] parameters = queryParameters.Split(',');
                    int sourceRecordId = Convert.ToInt32(parameters[0]);
                    int businessUnitId = Convert.ToInt32(parameters[1]);
                    int entityId = Convert.ToInt32(parameters[2]);
                    string npi = string.Empty;

                    if (parameters.Count() == 4)
                    {
                        if (!string.IsNullOrEmpty(parameters[3]))
                        {
                            npi = parameters[3];
                        }
                    }

                    if (entityId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer)
                    {
                        entityExistList.Add(ValidateCustomer(context, sourceRecordId, businessUnitId, npi));
                    }
                    else if (entityId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDPractice)
                    {
                        entityExistList.Add(ValidatePractice(context, sourceRecordId, businessUnitId));
                    }
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }

            return entityExistList as List<T>;
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Find customer id for given NPI
        /// </summary>
        /// <param name="context">Current database context</param>
        /// <param name="npi">NPI</param>
        /// <returns>Get the customer id for passed NPI</returns>
        private int GetExistingEntityCMDIdForSameNPI(CMDDatabaseContext context, string npi)
        {
            return context.CMDCustomers.Where(cmdCustomer => cmdCustomer.IsActive && cmdCustomer.NPI.ToLower().Equals(npi.ToLower()))
                          .Select(cmdCustomer => cmdCustomer.Id).FirstOrDefault();
        }

        /// <summary>
        /// Check in Source record table : First if for same business unit that ID is there or not, if exist return that customer is already exist
        /// </summary>
        /// <param name="context">Current database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <returns>True: If customer is already exist, False: If customer is not exist</returns>
        private bool IsCustomerExistforSameBusinessUnit(CMDDatabaseContext context, int sourceRecordId, int businessUnitId)
        {
            return context.CMDCustomerSourceTracks
                                  .Any(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.IsActive &&
                                      cmdCustomerSourceTrackRecord.SourceRecordID == sourceRecordId &&
                                      cmdCustomerSourceTrackRecord.BusinessUnitID == businessUnitId);
        }

        /// <summary>
        /// Check which group given Business unit id belongs
        /// Get all business units belong to cmdSourceGroup Group
        /// If no id is there for same businessunit, check that Sourceid for other businessunits belong to same group
        /// Then we will return that only source record need to insert with same cmdCustomerid.
        /// </summary>
        /// <param name="context">Current database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <returns>Customer Id : If for same business unit that customer id  is already exist, NULL: If for same business unit that customer id is not exist</returns>
        private int? GetExistingCMDCustomerIdforSameGroup(CMDDatabaseContext context, int sourceRecordId, int businessUnitId)
        {
            int? existingCMDCustomerIdforSameGroup = null;

            if (!IsCustomerExistforSameBusinessUnit(context, sourceRecordId, businessUnitId))
            {
                existingCMDCustomerIdforSameGroup =
                        context.CMDCustomerSourceTracks
                        .Where(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.IsActive &&
                            cmdCustomerSourceTrackRecord.SourceRecordID == sourceRecordId &&
                            context.CMDSourceGroupCMDBusinessUnitMaps
                            .Where(cmdSourceGroupCMDBusinessUnitMap =>
                            cmdSourceGroupCMDBusinessUnitMap.CMDBusinessUnitID != businessUnitId &&
                            cmdSourceGroupCMDBusinessUnitMap.IsActive &&
                            cmdSourceGroupCMDBusinessUnitMap.CMDSourceGroupID ==
                            context.CMDSourceGroupCMDBusinessUnitMaps
                            .Where(cmdSourceGroupCMDBusinessUnitMapRecord => cmdSourceGroupCMDBusinessUnitMapRecord.CMDBusinessUnitID == businessUnitId &&
                                cmdSourceGroupCMDBusinessUnitMapRecord.IsActive &&
                                cmdSourceGroupCMDBusinessUnitMapRecord.CMDSourceGroup.IsActive)
                                .Select(cmdSourceGroupCMDBusinessUnitMapRecord => cmdSourceGroupCMDBusinessUnitMapRecord.CMDSourceGroup.Id)
                                .FirstOrDefault())
                        .Select(cmdSourceGroupCMDBusinessUnitMap => cmdSourceGroupCMDBusinessUnitMap)
                        .Any(cmdSourceGroupCMDBusinessUnitMap => cmdSourceGroupCMDBusinessUnitMap.CMDBusinessUnitID == cmdCustomerSourceTrackRecord.BusinessUnitID))
                        .Select(cmdCustomerSourceTrackRecord => cmdCustomerSourceTrackRecord.CMDCustomerID).FirstOrDefault();
            }

            return existingCMDCustomerIdforSameGroup;
        }

        /// <summary>
        /// Check in Source record table : If practice exists for business unit id and source record id exists or not
        /// </summary>
        /// <param name="context">Current database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <returns>True: If practice is already exist, False: If practice is not exist</returns>
        private bool IsPracticeExistforSameBusinessUnit(CMDDatabaseContext context, int sourceRecordId, int businessUnitId)
        {
            return context.CMDPracticeSourceTracks.Any(cmdPracticeSourceTrackRecord => cmdPracticeSourceTrackRecord.IsActive &&
                                                        cmdPracticeSourceTrackRecord.SourceRecordID == sourceRecordId &&
                                                        cmdPracticeSourceTrackRecord.BusinessUnitID == businessUnitId);
        }

        /// <summary>
        /// Check which group for given business unit id belongs
        /// Get all business units belong to cmdSourceGroup Group
        /// If no id is there for same businessunit, check that Sourceid for other businessunits belong to same group
        /// Then we will return that only source record need to insert with same cmdPracticeid.
        /// </summary>
        /// <param name="context">Current database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <returns>Practice Id : If for same business unit that id practice is already exist, NULL: If for same business unit that id practice is not exist</returns>
        private int? GetExistingCMDPracticeIdForSameGroup(CMDDatabaseContext context, int sourceRecordId, int businessUnitId)
        {
            int? existingCMDPracticeIdforSameGroup = null;

            if (!IsPracticeExistforSameBusinessUnit(context, sourceRecordId, businessUnitId))
            {
                existingCMDPracticeIdforSameGroup = context.CMDPracticeSourceTracks.Where(cmdPracticeSourceTrackRecord => cmdPracticeSourceTrackRecord.IsActive &&
                                                    cmdPracticeSourceTrackRecord.SourceRecordID == sourceRecordId &&
                                                        context.CMDSourceGroupCMDBusinessUnitMaps.Where(cmdSourceGroupCMDBusinessUnitMap =>
                                                                cmdSourceGroupCMDBusinessUnitMap.CMDBusinessUnitID != businessUnitId && cmdSourceGroupCMDBusinessUnitMap.IsActive &&
                                                                cmdSourceGroupCMDBusinessUnitMap.CMDSourceGroupID == context.CMDSourceGroupCMDBusinessUnitMaps
                                                                                .Where(cmdSourceGroupCMDBusinessUnitMapRecord => cmdSourceGroupCMDBusinessUnitMapRecord.CMDBusinessUnitID == businessUnitId &&
                                                                                cmdSourceGroupCMDBusinessUnitMapRecord.IsActive && cmdSourceGroupCMDBusinessUnitMapRecord.CMDSourceGroup.IsActive)
                                                                                .Select(cmdSourceGroupCMDBusinessUnitMapRecord => cmdSourceGroupCMDBusinessUnitMapRecord.CMDSourceGroup.Id)
                                                        .FirstOrDefault())
                                                    .Select(cmdSourceGroupCMDBusinessUnitMap => cmdSourceGroupCMDBusinessUnitMap).Any(cmdSourceGroupCMDBusinessUnitMap =>
                                                    cmdSourceGroupCMDBusinessUnitMap.CMDBusinessUnitID == cmdPracticeSourceTrackRecord.BusinessUnitID))
                                                    .Select(cmdPracticeSourceTrackRecord => cmdPracticeSourceTrackRecord.CMDPracticeID).FirstOrDefault();
            }

            return existingCMDPracticeIdforSameGroup;
        }

        /// <summary>
        /// This method check whether customer exists for business unit id and source record id
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <param name="npi">NPI to check the customer</param>        
        /// <returns></returns>
        private EntityExist ValidateCustomer(CMDDatabaseContext context, int sourceRecordId, int businessUnitId, string npi)
        {
            EntityExist cmdCustomerExist = new EntityExist();
            int existingEntityCMDIdforSameNPI = 0;

            if (!string.IsNullOrEmpty(npi))
            {
                //// Find customer id for given NPI
                existingEntityCMDIdforSameNPI = GetExistingEntityCMDIdForSameNPI(context, npi);

                if (existingEntityCMDIdforSameNPI > 0)
                {
                    cmdCustomerExist.ExistingEntityCMDIdforAutoMerge = existingEntityCMDIdforSameNPI;
                    cmdCustomerExist.AutoMergeCriteria = (int)ALPHAEON.CMD.Common.Enums.AutoMergeCriteria.SameNPI;
                    cmdCustomerExist.IsEntityExistforSameBusinessUnit = false;
                }
            }

            // If Customer is not merged using same NPI, check for Same SourceGroup
            if (existingEntityCMDIdforSameNPI == 0)
            {
                cmdCustomerExist.IsEntityExistforSameBusinessUnit = IsCustomerExistforSameBusinessUnit(context, sourceRecordId, businessUnitId);
                cmdCustomerExist.ExistingEntityCMDIdforAutoMerge = GetExistingCMDCustomerIdforSameGroup(context, sourceRecordId, businessUnitId);
                cmdCustomerExist.AutoMergeCriteria = (int)ALPHAEON.CMD.Common.Enums.AutoMergeCriteria.SameSourceGroup;
            }

            return cmdCustomerExist;
        }

        /// <summary>
        /// This method check whether practice exists for business unit id and source record id
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="sourceRecordId">Current source record id</param>
        /// <param name="businessUnitId">Current business unit id</param>
        /// <returns></returns>
        private EntityExist ValidatePractice(CMDDatabaseContext context, int sourceRecordId, int businessUnitId)
        {
            EntityExist cmdPracticeExist = new EntityExist();
            cmdPracticeExist.IsEntityExistforSameBusinessUnit = IsPracticeExistforSameBusinessUnit(context, sourceRecordId, businessUnitId);
            cmdPracticeExist.ExistingEntityCMDIdforAutoMerge = GetExistingCMDPracticeIdForSameGroup(context, sourceRecordId, businessUnitId);
            cmdPracticeExist.AutoMergeCriteria = (int)ALPHAEON.CMD.Common.Enums.AutoMergeCriteria.SameSourceGroup;
            return cmdPracticeExist;
        }

        #endregion Private Methods
    }
}