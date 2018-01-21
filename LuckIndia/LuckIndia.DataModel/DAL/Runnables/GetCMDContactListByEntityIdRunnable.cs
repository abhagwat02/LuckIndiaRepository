
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
    sealed class GetCMDContactListByEntityIdRunnable : BaseApiController, IRunnable
    {
        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns a list of CMDContacts for the entity whose Id is passed. Entity type can be CMDCustomer or CMDPractice.
        /// </summary>
        /// <typeparam name="T">Type of object returned by RPC, here CMDContact</typeparam>
        /// <param name="context">Obj. of CMDDatabaseContext</param>
        /// <param name="whereCondition">where condition string</param>
        /// <param name="orderByCondition">order by condition string</param>
        /// <param name="skip">No. of records to be skipped</param>
        /// <param name="take">No. of records to be taken</param>
        /// <param name="queryParameters">Query Parameters, comma separated IDs entityId, entityTypeId</param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            context.Configuration.LazyLoadingEnabled = false;
            List<CMDDashboardContact> cmdContactListResult = null;
            try
            {
                if (!string.IsNullOrEmpty(queryParameters))
                {
                    string[] parameters = queryParameters.Split(',');
                    int entityId = Convert.ToInt32(parameters[0]);
                    int entityTypeId = Convert.ToInt32(parameters[1]);
                    int usage = Convert.ToInt32(parameters[2]);
                    int businessUnitId = 0;

                    if (usage == (int)ALPHAEON.CMD.Common.Enums.Usage.CMDApi)
                    {
                        businessUnitId = Convert.ToInt32(parameters[3]);
                    }
                    IQueryable<CMDDashboardContact> contactList = null;

                    if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer)
                    {
                        if (businessUnitId > 0)
                        {

                            #region Customer CMDContact List

                            contactList = (from cmdContact in context.CMDContacts
                                           where cmdContact.IsActive == true
                                           join cmdCustomerContactMap in context.CMDCustomerContactMaps
                                           on cmdContact.Id equals cmdCustomerContactMap.CMDContactID
                                           where cmdCustomerContactMap.CMDCustomerID == entityId && cmdCustomerContactMap.IsActive == true && cmdCustomerContactMap.CMDBusinessUnitID == businessUnitId
                                           select new CMDDashboardContact
                                           {
                                               Address1 = cmdContact.Address1,
                                               Address2 = cmdContact.Address2,
                                               City = cmdContact.City,
                                               Cell = cmdContact.Cell,
                                               CMDStateID = cmdContact.CMDState.Id,
                                               CMDStateAbbreviation = cmdContact.CMDState.Abbriviation,
                                               ContactName = cmdContact.ContactName,
                                               CreatedDate = cmdContact.CreatedDate,
                                               Fax = cmdContact.Fax,
                                               Id = cmdContact.Id,
                                               IsDefault = cmdContact.IsDefault,
                                               Phone = cmdContact.Phone,
                                               PrivateEmail = cmdContact.PrivateEmail,
                                               PublicEmail = cmdContact.PublicEmail,
                                               UpdatedDate = cmdContact.UpdatedDate,
                                               WebsiteURL = cmdContact.WebsiteURL,
                                               ZipCode = cmdContact.ZipCode,
                                               CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                               CMDStateName = cmdContact.CMDState.Name,
                                               IsActive = cmdContact.IsActive,
                                           });

                            #endregion

                        }
                        else
                        {
                            #region Customer CMDContact List

                            contactList = (from cmdContact in context.CMDContacts
                                           where cmdContact.IsActive == true
                                           join cmdCustomerContactMap in context.CMDCustomerContactMaps
                                           on cmdContact.Id equals cmdCustomerContactMap.CMDContactID
                                           where cmdCustomerContactMap.CMDCustomerID == entityId && cmdCustomerContactMap.IsActive == true
                                           select new CMDDashboardContact
                                           {
                                               Address1 = cmdContact.Address1,
                                               Address2 = cmdContact.Address2,
                                               City = cmdContact.City,
                                               Cell = cmdContact.Cell,
                                               CMDStateID = cmdContact.CMDState.Id,
                                               CMDStateAbbreviation = cmdContact.CMDState.Abbriviation,
                                               CMDCountryAbbreviation = cmdContact.CMDState.CMDCountry.Abbreviation,
                                               ContactName = cmdContact.ContactName,
                                               CreatedDate = cmdContact.CreatedDate,
                                               Fax = cmdContact.Fax,
                                               Id = cmdContact.Id,
                                               IsDefault = cmdContact.IsDefault,
                                               Phone = cmdContact.Phone,
                                               PrivateEmail = cmdContact.PrivateEmail,
                                               PublicEmail = cmdContact.PublicEmail,
                                               UpdatedDate = cmdContact.UpdatedDate,
                                               WebsiteURL = cmdContact.WebsiteURL,
                                               ZipCode = cmdContact.ZipCode,
                                               CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                               CMDStateName = cmdContact.CMDState.Name,
                                               IsActive = cmdContact.IsActive,
                                           });

                            #endregion
                        }
                    }

                    if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDPractice)
                    {

                        #region Practice CMDContact List

                        contactList = (from cmdContact in context.CMDContacts
                                       where cmdContact.IsActive == true
                                       join cmdPracticeContactMap in context.CMDPracticeContactMaps
                                       on cmdContact.Id equals cmdPracticeContactMap.CMDContactID
                                       where cmdPracticeContactMap.CMDPracticeID == entityId && cmdPracticeContactMap.IsActive == true
                                       select new CMDDashboardContact
                                       {
                                           Address1 = cmdContact.Address1,
                                           Address2 = cmdContact.Address2,
                                           City = cmdContact.City,
                                           Cell = cmdContact.Cell,
                                           CMDStateID = cmdContact.CMDState.Id,
                                           CMDState = cmdContact.CMDState.Name,
                                           Country = cmdContact.CMDState.CMDCountry.Name,
                                           CMDStateAbbreviation = cmdContact.CMDState.Abbriviation,
                                           CMDCountryAbbreviation = cmdContact.CMDState.CMDCountry.Abbreviation,
                                           ContactName = cmdContact.ContactName,
                                           CreatedDate = cmdContact.CreatedDate,
                                           Fax = cmdContact.Fax,
                                           Id = cmdContact.Id,
                                           IsDefault = cmdContact.IsDefault,
                                           Phone = cmdContact.Phone,
                                           PrivateEmail = cmdContact.PrivateEmail,
                                           PublicEmail = cmdContact.PublicEmail,
                                           UpdatedDate = cmdContact.UpdatedDate,
                                           WebsiteURL = cmdContact.WebsiteURL,
                                           ZipCode = cmdContact.ZipCode,
                                           CMDCountryName = cmdContact.CMDState.CMDCountry.Name,
                                           CMDStateName = cmdContact.CMDState.Name,
                                           IsActive = cmdContact.IsActive,
                                       });

                        #endregion

                    }

                    if (!string.IsNullOrEmpty(whereCondition))
                    {
                        contactList = contactList.Where(whereCondition);

                    }

                    if (!string.IsNullOrEmpty(orderByCondition))
                    {
                        contactList = contactList.OrderBy(orderByCondition).Skip(skip).Take(take);
                    }
                    else
                    {

                        contactList = contactList.OrderBy("Id").Skip(skip).Take(take);
                    }

                    cmdContactListResult = contactList.ToList();

                    if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer)
                    {
                        CMDLogger.LogAudit(string.Format("Obtained the CMD Contact List by {0} ID from CMD", ALPHAEON.CMD.Common.Enums.Entity.CMDCustomer), ALPHAEON.CMD.Common.Constants.General.ApplicationName);
                    }
                    if (entityTypeId == (int)ALPHAEON.CMD.Common.Enums.Entity.CMDPractice)
                    {
                        CMDLogger.LogAudit(string.Format("Obtained the CMD Contact List by {0} ID from CMD", ALPHAEON.CMD.Common.Enums.Entity.CMDPractice), ALPHAEON.CMD.Common.Constants.General.ApplicationName);
                    }
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
            }
            return cmdContactListResult as List<T>;
        }


        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }
    }
}



