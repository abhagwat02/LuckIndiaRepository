namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Exceptions;
    using Alphaeon.Services.EnterpriseAPI.Controllers;
    using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
    using Alphaeon.Services.EnterpriseAPI.Models;
    using ALPHAEON.CMD.TransientMessage;

    internal sealed class CMDCustomerAutoMergeRunnable : BaseApiController, IRunnable
    {
        #region Support Methods

        public void Execute(CMDDatabaseContext context)
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            context.Configuration.LazyLoadingEnabled = false;

            //// Find the current status(Running, Completed, Aborted, Stopped) of RPC
            var status = context.CMDAPIStatus.Where(apiStatus => apiStatus.APIName == RemoteProcedureCallValue.RPC_CMDCustomerAutoMerge.ToString()).FirstOrDefault();
            try
            {
                List<CMDCustomerForAutoMerge> customerListResult = null;

                IQueryable<CMDCustomerForAutoMerge> cmdCustomerList = null;

                string cmdBusinessunit = string.Empty;

                //// Find the CMD business unit by name
                var businessUnit = context.CMDBusinessUnits.FirstOrDefault(cmdBusinessUnit => cmdBusinessUnit.IsActive &&
                    cmdBusinessUnit.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDBusinessUnitNameCMD);

                if (businessUnit != null)
                {
                    cmdBusinessunit = businessUnit.ToString();
                }

                //// Check RPC status that its running or not , if not than proceed to auto merge
                if (status.APIStatus != (int)ALPHAEON.CMD.Common.Enums.ApiStatus.Running)
                {
                    var apiStatusDelta = new Dictionary<string, object>();

                    //// Update the AutoMerge API status = Running
                    apiStatusDelta.Add(DAL.PropertyName<Models.CMDAPIStatus>(x => x.APIStatus), (int)ALPHAEON.CMD.Common.Enums.ApiStatus.Running);
                    context.Update<Models.CMDAPIStatus>((int)status.Id, apiStatusDelta, false);

                    List<CMDCustomerForAutoMerge> activeCustomersForNpiMatch = GetCustomerListForNpiMatch(context);

                    List<CMDCustomerForAutoMerge> getNpiMatchedCustomerList = null;
                    CustomerStatusBulkUpdate customerBulkUpdate = new CustomerStatusBulkUpdate();

                    if (activeCustomersForNpiMatch != null && activeCustomersForNpiMatch.Count() > 0)
                    {
                        foreach (var item in activeCustomersForNpiMatch)
                        {
                            if (item.Status == ALPHAEON.CMD.Common.Constants.General.PendingforAutoMerge && !item.IsChecked)
                            {
                                var recordIndex = activeCustomersForNpiMatch.IndexOf(item);

                                if (!string.IsNullOrEmpty(item.NPI))
                                {
                                    //// List of customer (NPI) matched with NPI
                                    getNpiMatchedCustomerList = NPIMatchedRecordsList(activeCustomersForNpiMatch, recordIndex, item);

                                    //// Fetches the Existing Customer and possible duplicates for the existing customer and passes it ahead for merging
                                    ProcessMatchedResultfromAnyCombination(context, activeCustomersForNpiMatch, item, getNpiMatchedCustomerList, cmdBusinessunit, true);
                                    getNpiMatchedCustomerList.Add(item);

                                    //// Customers having distinct Customer Id
                                    customerBulkUpdate.CustomerIDsToBeUpdatedList = getNpiMatchedCustomerList.GroupBy(x => x.CMDCustomerId).Select(x => x.First().CMDCustomerId).ToList();
                                    customerBulkUpdate.NewValue = ALPHAEON.CMD.Common.Constants.General.Processed;

                                    //// Call to Bulk Update the Customers status, since Number of customers will be more
                                    CustomerstatusBulkUpdate(customerBulkUpdate);
                                }
                            }
                        }
                    }

                    //// Fetch the customer list with customer email and phone number
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithCustomerEmailAndPhone = GetCustomerListWithCustomerEmailAndPhone(context);

                    //// Fetch the customer list with contact email and phone number of customer
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithContactEmailAndPhone = GetCustomerListWithContactEmailAndPhone(context);

                    //// Fetch the customer list with customer phone and contact email
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithCombinationsOfContactPhoneAndEmail = GetCustomerListWithCombinationOfContactPhoneAndEmail(context, cmdCustomerListWithContactEmailAndPhone);

                    //// Fetch the customer list with customer contact phone and customer email
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithCombinationsOfContactEmailAndPhone = GetCustomerListWithContactEmailCombinations(context, cmdCustomerListWithContactEmailAndPhone);

                    //// Fetch the customer list with customer email and contact phone
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithCustomerEmailAndContactPhone = GetCustomerListWithCustomerEmailAndContactPhone(context);

                    //// Fetch the customer list with customer phone and contact email
                    IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithCustomerPhoneAndContactEmail = GetCustomerListWithCustomerPhoneAndContactEmail(context);

                    //// Fetch the customer list with default phone , email
                    var customerListWithDefaultAndcontactEmailAndPhone = cmdCustomerListWithCustomerEmailAndPhone.Concat(cmdCustomerListWithContactEmailAndPhone).Concat(cmdCustomerListWithCombinationsOfContactEmailAndPhone)
                        .Concat(cmdCustomerListWithCombinationsOfContactPhoneAndEmail)
                        .Concat(cmdCustomerListWithCustomerEmailAndContactPhone)
                        .Concat(cmdCustomerListWithCustomerPhoneAndContactEmail).Distinct();

                    //// Fetch the customer list with email from customer email table
                    IQueryable<CMDCustomerForAutoMerge> customerListWithEmailFromCustomerEmailTable = GetCustomerListWithEmailFromCustomerEmailTable(context, customerListWithDefaultAndcontactEmailAndPhone);

                    //// Fetch the customer list with phone from customer phone table
                    IQueryable<CMDCustomerForAutoMerge> customerListWithPhoneFromCustomerPhoneTable = GetCustomerListWithPhoneFromCustomerPhoneTable(context, customerListWithDefaultAndcontactEmailAndPhone);

                    //// Fetch the customer list with email, phone from customer email & customer phone 
                    IQueryable<CMDCustomerForAutoMerge> customerListWithMultipleEmailAndPhoneAssociatedWithCustomer = GetCustomerListWithMultipleEmailAndPhoneAssociatedWithCustomer(customerListWithDefaultAndcontactEmailAndPhone, customerListWithEmailFromCustomerEmailTable, customerListWithPhoneFromCustomerPhoneTable);

                    cmdCustomerList = customerListWithDefaultAndcontactEmailAndPhone.Where(m => m.IsActive)
                     .Concat(customerListWithEmailFromCustomerEmailTable).Where(m => m.IsActive)
                     .Concat(customerListWithPhoneFromCustomerPhoneTable).Where(m => m.IsActive)
                     .Concat(customerListWithMultipleEmailAndPhoneAssociatedWithCustomer).Where(m => m.IsActive)
                    .OrderBy(x => x.CMDCustomerId)
                    .ThenByDescending(x => x.FirstName)
                    .ThenByDescending(x => x.LastName)
                    .ThenByDescending(x => x.Phone)
                    .ThenByDescending(x => x.Email)
                    .ThenByDescending(x => x.ZipCode)
                    .ThenByDescending(x => x.City)
                    .ThenByDescending(x => x.StreetNumber).Distinct();

                    cmdCustomerList = cmdCustomerList.Where(m => m.IsActive);

                    customerListResult = cmdCustomerList.ToList();
                    int itemIndex = 0;
                    List<CMDCustomerForAutoMerge> getCriteriaMatchedCustomerList = null;
                    foreach (var item in customerListResult)
                    {
                        if (item.Status == ALPHAEON.CMD.Common.Constants.General.PendingforAutoMerge && !item.IsChecked)
                        {
                            ////Find index of current Item in customerMergeListResult
                            itemIndex = customerListResult.IndexOf(item);
                            //// Fetch the list of customers matching the criteria FName, LName, Street#, Phone, City, Zip, Email
                            getCriteriaMatchedCustomerList = CriteriaMatchedRecordsList(customerListResult, itemIndex, item);
                            if (getCriteriaMatchedCustomerList != null && getCriteriaMatchedCustomerList.Count() > 0)
                            {
                                //// Fetches the existing customer and possible duplicates for the existing customer and passes it ahead for merging
                                ProcessMatchedResultfromAnyCombination(context, customerListResult, item, getCriteriaMatchedCustomerList, cmdBusinessunit, false);
                            }
                        }
                    }
                    ////Customers having distinct Customer Id
                    customerBulkUpdate.CustomerIDsToBeUpdatedList = customerListResult.GroupBy(x => x.CMDCustomerId).Select(x => x.First().CMDCustomerId).ToList();
                    customerBulkUpdate.NewValue = ALPHAEON.CMD.Common.Constants.General.Processed;
                    ////Call to Bulk Update the Customers status, since Number of customers will be more
                    CustomerstatusBulkUpdate(customerBulkUpdate);
                    var statusDelta = new Dictionary<string, object>();
                    //// Update the AutoMerge API status to Completed
                    statusDelta.Add(DAL.PropertyName<Models.CMDAPIStatus>(x => x.APIStatus), (int)ALPHAEON.CMD.Common.Enums.ApiStatus.Completed);
                    context.Update<Models.CMDAPIStatus>((int)status.Id, statusDelta, false);
                }
            }
            catch (Exception ex)
            {
                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.General.ObjectName, ALPHAEON.CMD.Common.Constants.General.ApplicationName, classAndMethodName);
                var delta = new Dictionary<string, object>();
                //// Update the AutoMerge API status to Aborted
                delta.Add(DAL.PropertyName<Models.CMDAPIStatus>(x => x.APIStatus), (int)ALPHAEON.CMD.Common.Enums.ApiStatus.Aborted);
                context.Update<Models.CMDAPIStatus>((int)status.Id, delta, false);
            }
        }

        public List<T> ExecuteList<T>(CMDDatabaseContext context, string whereCondition, string orderByCondition, int skip = 0, int take = 0, string queryParameters = null) where T : class
        {
            throw new NotImplementedException();
        }

        T IRunnable.Execute<T>(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        #region Methods - Get Ids List

        /// <summary>
        /// Method for fetching the list of customers matching the criteria 
        /// NPI
        /// </summary>
        /// <param name="customerListResult">List of customers</param>
        /// <param name="itemIndex">Index of the customer that is being compared for duplicates with the rest of the customers</param>
        /// <param name="item">Customer for which we need to find duplicates</param>
        /// <returns>List of Criteria (NPI) matched customers</returns>
        private static List<CMDCustomerForAutoMerge> NPIMatchedRecordsList(
            List<CMDCustomerForAutoMerge> customerListResult,
            int itemIndex,
            CMDCustomerForAutoMerge item)
        {
            var combination1MatchedCustomerList = customerListResult.Where(customer =>
                customer.NPI == item.NPI &&
                itemIndex != customerListResult.IndexOf(customer) &&
                !customer.IsChecked)
                .OrderBy(x => x.CreatedDate).ToList();
            return combination1MatchedCustomerList;
        }

        /// <summary>
        /// Method for fetching the list of customers matching the criteria 
        /// FName, LName, Street#, Phone, City, Zip, Email
        /// </summary>
        /// <param name="customerListResult">List of customers</param>
        /// <param name="itemIndex">Index of the customer that is being compared for 
        /// duplicates with the rest of the customers</param>
        /// <param name="item">Customer for which we need to find duplicates</param>
        /// <returns>List of Criteria 
        /// ( FName, LName, Street#, Phone, City, Zip, Email) matched customers</returns>
        private static List<CMDCustomerForAutoMerge> CriteriaMatchedRecordsList(
            List<CMDCustomerForAutoMerge> customerListResult,
            int itemIndex,
            CMDCustomerForAutoMerge item)
        {
            List<CMDCustomerForAutoMerge> combination1MatchedCustomerList = new List<CMDCustomerForAutoMerge>();
            customerListResult = customerListResult.Where(m => m.CMDCustomerId != item.CMDCustomerId).ToList();
            customerListResult = customerListResult.Where(m => !(string.IsNullOrEmpty(m.FirstName) ||
               string.IsNullOrEmpty(m.LastName) ||
               string.IsNullOrEmpty(m.StreetNumber) ||
               string.IsNullOrEmpty(m.Phone) ||
               string.IsNullOrEmpty(m.City) ||
               string.IsNullOrEmpty(m.ZipCode) ||
               string.IsNullOrEmpty(m.Email))).ToList();

            if (!(string.IsNullOrEmpty(item.FirstName) ||
                string.IsNullOrEmpty(item.LastName) ||
                string.IsNullOrEmpty(item.StreetNumber) ||
                string.IsNullOrEmpty(item.Phone) ||
                string.IsNullOrEmpty(item.City) ||
                string.IsNullOrEmpty(item.ZipCode) ||
                string.IsNullOrEmpty(item.Email)))
            {
                combination1MatchedCustomerList = customerListResult.Where(customer =>
                   customer.FirstName.Trim().ToUpper() == item.FirstName.Trim().ToUpper() &&
                   customer.LastName.Trim().ToUpper() == item.LastName.Trim().ToUpper() &&
                   customer.StreetNumber.Trim().ToUpper() == item.StreetNumber.Trim().ToUpper() &&
                   customer.Phone.Trim().ToUpper() == item.Phone.Trim().ToUpper() &&
                   customer.City.Trim().ToUpper() == item.City.Trim().ToUpper() &&
                   customer.ZipCode.Trim().ToUpper() == item.ZipCode.Trim().ToUpper() &&
                   customer.Email.Trim().ToUpper() == item.Email.Trim().ToUpper() &&
                   !customer.IsChecked)
                   .OrderBy(x => x.CreatedDate).ToList();
            }

            return combination1MatchedCustomerList;
        }

        /// <summary>
        /// This method is internally used by Business Operation methods to map CMDCustomer model to Customer transient message
        /// </summary>
        /// <param name="cmdCustomer">Object of CMDCustomer</param>
        /// <returns>Mapped Customer Transient Message Object</returns>
        private static ALPHAEON.CMD.TransientMessage.Customer MapCMDCustomerToCustomerTransientMessage(CMDCustomer cmdCustomer)
        {
            ALPHAEON.CMD.TransientMessage.Customer customer = new ALPHAEON.CMD.TransientMessage.Customer();

            customer.Id = cmdCustomer.Id;
            customer.FirstName = cmdCustomer.FirstName;
            customer.LastName = cmdCustomer.LastName;
            customer.IsActive = cmdCustomer.IsActive;
            customer.MiddleName = cmdCustomer.MiddleName;
            customer.NPI = cmdCustomer.NPI;
            customer.UpdatedDate = cmdCustomer.UpdatedDate;
            customer.Gender = cmdCustomer.Gender;
            customer.CMDCustomerTypeID = cmdCustomer.CMDCustomerTypeID;
            customer.DateOfBirth = cmdCustomer.DOB;
            customer.Email = cmdCustomer.Email;
            customer.AddressPhoneNumber = cmdCustomer.Phone;
            return customer;
        }

        /// <summary>
        /// This method returns list of emails from CMDCustomerDetails associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of emails Ids associated with the customer</returns>
        private static List<string> GetEmailsListforCMDCustomerfromCMDCustomerEmail(int customerId, CMDDatabaseContext context)
        {
            var customerDetailEmailsList = context.CMDCustomerEmails
                .Where(customerEmail =>
                    customerEmail.CMDCustomerID == customerId &&
                    customerEmail.IsActive && customerEmail.CMDCustomer.IsActive).Select(customerDetailsRecord => customerDetailsRecord.Email).ToList();

            return customerDetailEmailsList;
        }

        /// <summary>
        /// This method returns list of Phones from CMDCustomerPhones associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of Phones Ids associated with the customer</returns>
        private static List<string> GetPhonesListforCMDCustomerfromCMDCustomerPhone(int customerId, CMDDatabaseContext context)
        {
            var customerDetailPhonesList = context.CMDCustomerPhones
                .Where(customerPhone =>
                    customerPhone.CMDCustomerID == customerId &&
                    customerPhone.IsActive).Select(customerDetailsRecord => customerDetailsRecord.Phone).ToList();

            return customerDetailPhonesList;
        }

        /// <summary>
        /// This method checks for given Customer Id is present in CustomersourceTrack returns true if found
        /// </summary>
        /// <param name="context">CMD DB Context</param>
        /// <param name="shoutMDBusinessUnitId">Id of ShoutMD Business Unit</param>
        /// <param name="customerId">CMDCustomer Id </param>
        /// <returns>True if CMDCustomerSourceTrack entry present for given Customer Id, else false</returns>
        private static bool CheckCMDCustomerBelongsToShoutMD(CMDDatabaseContext context, int shoutMDBusinessUnitId, int cmdCustomerId)
        {
            return context.CMDCustomerSourceTracks
                                .Any(cmdCustomerSourceTrack => cmdCustomerSourceTrack.BusinessUnitID == shoutMDBusinessUnitId &&
                                    cmdCustomerSourceTrack.CMDCustomerID == cmdCustomerId &&
                                    cmdCustomerSourceTrack.IsActive);
        }

        /// <summary>
        /// This method returns list of speciality Ids associated with the customer which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of speciality Ids associated with the customer</returns>
        private static List<int> GetCMDSpecialityIDsListforCMDCustomer(int customerId, CMDDatabaseContext context)
        {
            var specialityIdsList = context.CMDCustomerSpecialityMaps
                                  .Where(cmdCustomerSpecialityMap => cmdCustomerSpecialityMap.CMDCustomerID == customerId
                                   && cmdCustomerSpecialityMap.IsActive
                                   && cmdCustomerSpecialityMap.CMDSpeciality.IsActive
                                   && cmdCustomerSpecialityMap.CMDCustomer.IsActive).Select(cmdCustomerSpecialityMap => cmdCustomerSpecialityMap.CMDSpecialityID).ToList();

            return specialityIdsList;
        }

        /// <summary>
        /// This method returns list of practice Ids associated with the customer which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of practice Ids associated with the customer</returns>
        private static List<int> GetCMDPracticeIDsListforCMDCustomer(int customerId, CMDDatabaseContext context)
        {
            var practiceIdsList = context.CMDCustomerPracticeMaps
                                  .Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == customerId
                                   && cmdCustomerPracticeMap.IsActive
                                   && cmdCustomerPracticeMap.CMDPractice.IsActive
                                   && cmdCustomerPracticeMap.CMDCustomer.IsActive).Select(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDPracticeID).ToList();

            return practiceIdsList;
        }

        /// <summary>
        /// This method returns list of product Ids associated with the customer which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of product Ids associated with the customer</returns>
        private static List<int> GetCMDProductIDsListforCMDCustomer(int customerId, CMDDatabaseContext context)
        {
            var productIdsList = context.CMDCustomerProductMaps
                                  .Where(cmdCustomerProductMap => cmdCustomerProductMap.CMDCustomerID == customerId
                                   && cmdCustomerProductMap.IsActive
                                   && cmdCustomerProductMap.CMDProduct.IsActive
                                   && cmdCustomerProductMap.CMDCustomer.IsActive).Select(cmdCustomerProductMap => cmdCustomerProductMap.CMDProductID).ToList();

            return productIdsList;
        }

        /// <summary>
        /// This method returns list of contact Ids associated with the customer which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of contact Ids associated with the customer</returns>
        private static List<int> GetCMDContactIDsListforCMDCustomer(int customerId, CMDDatabaseContext context)
        {
            var customerContactIDsList = context.CMDCustomerContactMaps
                .Where(customerContactMap =>
                    customerContactMap.CMDCustomerID == customerId &&
                    customerContactMap.IsActive &&
                    customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive)
                    .Select(customerContactMap => customerContactMap.CMDContactID).ToList();
            return customerContactIDsList;
        }

        /// <summary>        
        /// This method return a list of customers where email fetched from CMDCustomerEmail table and phone fetched from CMDCustomerPhone table
        /// </summary>
        /// <param name="customerListWithDefaultAndcontactEmailAndPhone">List of customers with phone number and email fetched from CMDCustomer, CMDContact</param>
        /// <param name="customerListWithEmailFromCustomerEmailTable">List of customers with email fetched from CMDCustomerEmail</param>
        /// <param name="customerListWithPhoneFromCustomerPhoneTable">List of customers with phone number fetched from CMDCustomerPhone</param>
        /// <returns>Customer list with email from CMDCustomerEmail and phone from CMDCustomerPhone table</returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithMultipleEmailAndPhoneAssociatedWithCustomer(
                                                                                                                           IQueryable<CMDCustomerForAutoMerge> customerListWithDefaultAndcontactEmailAndPhone, IQueryable<CMDCustomerForAutoMerge> customerListWithEmailFromCustomerEmailTable,
                                                                                                                           IQueryable<CMDCustomerForAutoMerge> customerListWithPhoneFromCustomerPhoneTable)
        {
            return (from cust in customerListWithDefaultAndcontactEmailAndPhone
                    join customerEmail in customerListWithEmailFromCustomerEmailTable
                    on cust.CMDCustomerId equals customerEmail.CMDCustomerId
                    join customerPhone in customerListWithPhoneFromCustomerPhoneTable
                    on cust.CMDCustomerId equals customerPhone.CMDCustomerId
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = cust.CMDCustomerId,
                        FirstName = cust.FirstName,
                        LastName = cust.LastName,
                        Phone = customerPhone.Phone,
                        Email = customerEmail.Email,
                        City = cust.City,
                        ZipCode = cust.ZipCode,
                        StreetNumber = cust.StreetNumber,
                        CreatedDate = cust.CreatedDate,
                        Status = cust.Status,
                        IsChecked = false,
                        IsActive = cust.IsActive
                    }).Distinct();
        }

        /// <summary>
        /// Method fetch the customer list with phone from customer phone table
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="customerListWithDefaultAndcontactEmailAndPhone">List of customers with phone number and email fetched from CMDCustomer, CMDContact</param>
        /// <returns>Customer list with phone from customer phone table</returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithPhoneFromCustomerPhoneTable(CMDDatabaseContext context, IQueryable<CMDCustomerForAutoMerge> customerListWithDefaultAndcontactEmailAndPhone)
        {
            return (from cust in customerListWithDefaultAndcontactEmailAndPhone
                    join customerPhone in context.CMDCustomerPhones
                    on cust.CMDCustomerId equals customerPhone.CMDCustomerID
                    where customerPhone.IsActive && customerPhone.CMDCustomer.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = cust.CMDCustomerId,
                        FirstName = cust.FirstName,
                        LastName = cust.LastName,
                        Phone = customerPhone.Phone,
                        Email = cust.Email,
                        City = cust.City,
                        ZipCode = cust.ZipCode,
                        StreetNumber = cust.StreetNumber,
                        CreatedDate = cust.CreatedDate,
                        Status = cust.Status,
                        IsChecked = false,
                        IsActive = cust.IsActive
                    }).Distinct();
        }

        /// <summary>
        /// Method fetch the customer list with email from customer email table
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="customerListWithDefaultAndcontactEmailAndPhone">List of customers with email number and phone fetched from CMDCustomer, CMDContact</param>
        /// <returns>Customer list with email from customer email table</returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithEmailFromCustomerEmailTable(CMDDatabaseContext context, IQueryable<CMDCustomerForAutoMerge> customerListWithDefaultAndcontactEmailAndPhone)
        {
            return (from cust in customerListWithDefaultAndcontactEmailAndPhone
                    join customerEmail in context.CMDCustomerEmails
                    on cust.CMDCustomerId equals customerEmail.CMDCustomerID
                    where customerEmail.IsActive && customerEmail.CMDCustomer.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = cust.CMDCustomerId,
                        FirstName = cust.FirstName,
                        LastName = cust.LastName,
                        Phone = cust.Phone,
                        Email = customerEmail.Email,
                        City = cust.City,
                        ZipCode = cust.ZipCode,
                        StreetNumber = cust.StreetNumber,
                        CreatedDate = cust.CreatedDate,
                        Status = cust.Status,
                        IsChecked = false,
                        IsActive = cust.IsActive
                    }).Distinct();
        }

        /// <summary>
        /// Method fetch the customer list with customer phone and contact email
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>Customer list with customer phone and contact email</returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithCustomerPhoneAndContactEmail(CMDDatabaseContext context)
        {
            return (from customer in context.CMDCustomers
                    where customer.IsActive && ((customer.IsActive && !customer.IsDuplicate) || (customer.IsDuplicate &&
                    customer.IsActive &&
                    context.CMDCustomerDuplicateMap.Where(customerDuplicateMap => customerDuplicateMap.IsActive &&
                        customerDuplicateMap.DuplicateCustomerID == customer.Id &&
                        customerDuplicateMap.ResolveAction == null).Any()))
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customer.Id equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerContactMap.CMDCustomer.Id,
                        FirstName = customerContactMap.CMDCustomer.FirstName,
                        LastName = customerContactMap.CMDCustomer.LastName,
                        Phone = customerContactMap.CMDCustomer.Phone,
                        Email = customerContactMap.CMDContact.PublicEmail,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerContactMap.CMDCustomer.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customer.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        /// <summary>
        /// Method fetch the customer list with customer email and contact phone
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>Customer list with customer email and contact phone</returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithCustomerEmailAndContactPhone(CMDDatabaseContext context)
        {
            return (from customer in context.CMDCustomers
                    where customer.IsActive && ((customer.IsActive && !customer.IsDuplicate) || (customer.IsDuplicate &&
                    customer.IsActive &&
                    context.CMDCustomerDuplicateMap.Where(customerDuplicateMap => customerDuplicateMap.IsActive &&
                        customerDuplicateMap.DuplicateCustomerID == customer.Id &&
                        customerDuplicateMap.ResolveAction == null).Any()))
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customer.Id equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerContactMap.CMDCustomer.Id,
                        FirstName = customerContactMap.CMDCustomer.FirstName,
                        LastName = customerContactMap.CMDCustomer.LastName,
                        Phone = customerContactMap.CMDContact.Phone,
                        Email = customerContactMap.CMDCustomer.Email,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerContactMap.CMDCustomer.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customer.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        /// <summary>
        /// Method fetch the customer list with combination phone and contact email
        /// </summary>
        /// <param name="context">Database Context</param>
        /// <param name="cmdCustomerListWithContactEmailAndPhone"></param>
        /// <returns></returns>
        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithCombinationOfContactPhoneAndEmail(CMDDatabaseContext context, IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithContactEmailAndPhone)
        {
            return (from customerEmailPhone in cmdCustomerListWithContactEmailAndPhone
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customerEmailPhone.CMDCustomerId equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerEmailPhone.CMDCustomerId,
                        FirstName = customerEmailPhone.FirstName,
                        LastName = customerEmailPhone.LastName,
                        Phone = customerEmailPhone.Phone,
                        Email = customerContactMap.CMDContact.PublicEmail,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerEmailPhone.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customerEmailPhone.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithContactEmailAndPhone(CMDDatabaseContext context)
        {
            //// Fetch the customer list with contact email and phone number of customer
            return (from customer in context.CMDCustomers
                    where customer.IsActive &&
                    ((customer.IsActive && !customer.IsDuplicate) || (customer.IsDuplicate &&
                    customer.IsActive &&
                    context.CMDCustomerDuplicateMap.Where(customerDuplicateMap => customerDuplicateMap.IsActive &&
                        customerDuplicateMap.DuplicateCustomerID == customer.Id &&
                        customerDuplicateMap.ResolveAction == null).Any()))
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customer.Id equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerContactMap.CMDCustomer.Id,
                        FirstName = customerContactMap.CMDCustomer.FirstName,
                        LastName = customerContactMap.CMDCustomer.LastName,
                        Phone = customerContactMap.CMDContact.Phone,
                        Email = customerContactMap.CMDContact.PublicEmail,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerContactMap.CMDCustomer.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customer.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        private static List<CMDCustomerForAutoMerge> GetCustomerListForNpiMatch(CMDDatabaseContext context)
        {
            //// Find all active customers to match with NPI
            return (from cmdCustomer in context.CMDCustomers
                    where cmdCustomer.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = cmdCustomer.Id,
                        FirstName = cmdCustomer.FirstName,
                        LastName = cmdCustomer.LastName,
                        Phone = cmdCustomer.Phone,
                        Email = cmdCustomer.Email,
                        CreatedDate = cmdCustomer.CreatedDate,
                        Status = cmdCustomer.Status,
                        NPI = cmdCustomer.NPI,
                        IsChecked = false,
                    }).ToList();
        }

        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithCustomerEmailAndPhone(CMDDatabaseContext context)
        {
            //// Fetch the customer list with customer email and phone number
            return (from customer in context.CMDCustomers
                    where customer.IsActive &&
                    ((customer.IsActive && !customer.IsDuplicate) || (customer.IsDuplicate &&
                    customer.IsActive &&
                    context.CMDCustomerDuplicateMap.Where(customerDuplicateMap => customerDuplicateMap.IsActive &&
                        customerDuplicateMap.DuplicateCustomerID == customer.Id &&
                        customerDuplicateMap.ResolveAction == null).Any()))
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customer.Id equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerContactMap.CMDCustomer.Id,
                        FirstName = customerContactMap.CMDCustomer.FirstName,
                        LastName = customerContactMap.CMDCustomer.LastName,
                        Phone = customerContactMap.CMDCustomer.Phone,
                        Email = customerContactMap.CMDCustomer.Email,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerContactMap.CMDCustomer.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customer.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        private static IQueryable<CMDCustomerForAutoMerge> GetCustomerListWithContactEmailCombinations(CMDDatabaseContext context, IQueryable<CMDCustomerForAutoMerge> cmdCustomerListWithContactEmailAndPhone)
        {
            //// Fetch the customer list with customer contact phone and customer email
            return (from customerEmailPhone in cmdCustomerListWithContactEmailAndPhone
                    join customerContactMap in context.CMDCustomerContactMaps
                    on customerEmailPhone.CMDCustomerId equals customerContactMap.CMDCustomerID
                    where customerContactMap.CMDCustomer.IsActive &&
                    customerContactMap.CMDContact.IsActive
                    select new CMDCustomerForAutoMerge
                    {
                        CMDCustomerId = customerEmailPhone.CMDCustomerId,
                        FirstName = customerEmailPhone.FirstName,
                        LastName = customerEmailPhone.LastName,
                        Phone = customerContactMap.CMDContact.Phone,
                        Email = customerEmailPhone.Email,
                        City = customerContactMap.CMDContact.City,
                        ZipCode = customerContactMap.CMDContact.ZipCode,
                        StreetNumber = customerContactMap.CMDContact.StreetNumber,
                        CreatedDate = customerEmailPhone.CreatedDate,
                        Status = customerContactMap.CMDCustomer.Status,
                        IsChecked = false,
                        IsActive = customerEmailPhone.IsActive
                    }).Where(m => m.IsActive).Distinct();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="context">Database context</param>
        /// <param name="customerDuplicateMap">Duplicate record of customer</param>
        /// <param name="cmdCustomerSpecialityMap">Speciality map of customer</param>
        private static void DeleteCustomerSpecialityMap(CustomerForAutoMerge dto, CMDDatabaseContext context, CMDCustomerDuplicateMap customerDuplicateMap, CMDCustomerSpecialityMap cmdCustomerSpecialityMap)
        {
            var customerSpecialityMapDelta = new Dictionary<string, object>();

            customerSpecialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.IsActive), false);

            customerSpecialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.UpdatedBy), dto.UpdatedBy);

            customerSpecialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.UpdatedDate), DateTime.UtcNow);

            context.Update<Models.CMDCustomerSpecialityMap>((int)cmdCustomerSpecialityMap.Id, customerSpecialityMapDelta, false);
            var autoMergeAuditLog = new CMDCustomerMergeAuditLog
            {
                CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerSpecialityMap.ToString(),
                ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                RecordID = cmdCustomerSpecialityMap.Id,
                AuditLogDate = DateTime.UtcNow
            };
            context.Create(autoMergeAuditLog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="context">Database Context</param>
        /// <param name="existingCustomerId">Existing customer id to merge</param>
        /// <param name="existingCustomer"></param>
        /// <param name="specialityId">Speciality id to merge</param>
        /// <returns></returns>
        private static CMDCustomerSpecialityMap CreateCustomerSpecialityMap(CustomerForAutoMerge dto, CMDDatabaseContext context, int existingCustomerId, CMDCustomer existingCustomer, int specialityId)
        {
            var newCustomerSpecialityMap = new CMDCustomerSpecialityMap
            {
                CMDCustomerID = Convert.ToInt32(existingCustomerId),

                CMDSpecialityID = specialityId,

                BusinessUnitID = existingCustomer.OriginalBusinessUnitID,

                IsActive = true,

                UpdatedBy = dto.UpdatedBy,

                UpdatedDate = DateTime.UtcNow,

                CreatedDate = DateTime.UtcNow
            };
            context.Create(newCustomerSpecialityMap);
            return newCustomerSpecialityMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Database Context</param>
        /// <param name="customerDuplicateMap">Duplicate record of customer</param>
        /// <param name="customerSpecialityMap">Speciality map of customer</param>
        private static void CreateAutoMergeAuditLog(CMDDatabaseContext context, CMDCustomerDuplicateMap customerDuplicateMap, CMDCustomerSpecialityMap customerSpecialityMap)
        {
            var autoMergeAuditLog = new CMDCustomerMergeAuditLog
            {
                CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerSpecialityMap.ToString(),
                RecordID = customerSpecialityMap.Id,
                AuditLogDate = DateTime.UtcNow
            };
            context.Create(autoMergeAuditLog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="context">Database Context</param>
        /// <param name="customerSpecialityMap">Speciality map of customer</param>
        private static void UpdateCustomerSpecialityMap(CustomerForAutoMerge dto, CMDDatabaseContext context, CMDCustomerSpecialityMap customerSpecialityMap)
        {
            #region If record present and deactivated: Update/activate CustomerSpecialityMap record --Starts

            var specialityMapDelta = new Dictionary<string, object>();

            specialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.IsActive), true);
            specialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.UpdatedBy), dto.UpdatedBy);

            specialityMapDelta.Add(DAL.PropertyName<Models.CMDCustomerSpecialityMap>(x => x.UpdatedDate), DateTime.UtcNow);

            context.Update<Models.CMDCustomerSpecialityMap>((int)customerSpecialityMap.Id, specialityMapDelta, false);

            #endregion If record present and deactivated: Update/activate CustomerSpecialityMap record --Ends
        }

        /// <summary>
        /// Method fetches the Existing Customer and possible duplicates .
        /// for the existing customer and passes it ahead for merging
        /// </summary>
        /// <param name="context">Database Context</param>
        /// <param name="customerListResult">List of customers</param>
        /// <param name="item">Customer which is to be compared against
        /// the rest of customers present in the list</param>
        /// <param name="combinationMatchedCustomerList">List of Customers matching Merge criteria</param>
        /// <param name="businessUnit">BusinessUnit</param>
        private void ProcessMatchedResultfromAnyCombination(CMDDatabaseContext context, List<CMDCustomerForAutoMerge> customerListResult, CMDCustomerForAutoMerge item, List<CMDCustomerForAutoMerge> combinationMatchedCustomerList, string businessUnit, bool isNpiMatch)
        {
            // Considering the record against which we got duplicate records
            if (combinationMatchedCustomerList != null && combinationMatchedCustomerList.Count() > 0)
            {
                combinationMatchedCustomerList.Add(item);

                combinationMatchedCustomerList = combinationMatchedCustomerList.OrderBy(m => m.CreatedDate).ToList();

                ////Marking identified matched records from combination1MatchedCustomerList in CustomerResultList as checked so that it is not considered for next comparison
                foreach (var matchedRecord in combinationMatchedCustomerList)
                {
                    customerListResult[customerListResult.IndexOf(customerListResult.Where(y => y == matchedRecord).FirstOrDefault())].IsChecked = true;
                }

                ////Idebtifying existing and duplicate from the combination1MatchedCustomerList
                var existingCustomerId = combinationMatchedCustomerList[0].CMDCustomerId;
                var duplicateCustomerList = combinationMatchedCustomerList.Except(combinationMatchedCustomerList.Where(z => z.CMDCustomerId == existingCustomerId));
                ////Prepare Transient Object to be patched
                CMDCustomer existingCustomer = context.CMDCustomers.Where(customer => customer.Id == existingCustomerId).FirstOrDefault();

                List<int> duplicateCustomerIDList = new List<int>();

                duplicateCustomerIDList.AddRange(duplicateCustomerList.Select(x => x.CMDCustomerId));

                ////Check if ExistingCustomer belongs to ShoutMD and there are any duplicateCustomers which belong to ShoutMD
                ////If so do not process the DuplicateCustomer belonging to ShoutMD
                int shoutMDBuId = (int)ALPHAEON.CMD.Common.Enums.AlphaeonApplication.ShoutMD;

                List<int> customerIdOfCustomerBelongingToShoutMD = new List<int>();

                if (CheckCMDCustomerBelongsToShoutMD(context, shoutMDBuId, existingCustomerId))
                {
                    foreach (int duplicateCustomerId in duplicateCustomerIDList)
                    {
                        if (CheckCMDCustomerBelongsToShoutMD(context, shoutMDBuId, duplicateCustomerId))
                        {
                            customerIdOfCustomerBelongingToShoutMD.Add(duplicateCustomerId);
                        }
                    }

                    duplicateCustomerIDList = duplicateCustomerIDList.Except(customerIdOfCustomerBelongingToShoutMD).ToList();
                }
                else
                {
                    List<CMDCustomerForAutoMerge> customerBelongingToShoutMd = new List<CMDCustomerForAutoMerge>();

                    ////Check in duplicate customers list if they belong to ShoutMD, if so only send one of the ShoutMD customers for merge
                    foreach (var duplicateCustomer in duplicateCustomerList)
                    {
                        if (CheckCMDCustomerBelongsToShoutMD(context, shoutMDBuId, duplicateCustomer.CMDCustomerId))
                        {
                            customerBelongingToShoutMd.Add(duplicateCustomer);
                            customerIdOfCustomerBelongingToShoutMD.Add(duplicateCustomer.CMDCustomerId);
                        }
                    }

                    customerBelongingToShoutMd = customerBelongingToShoutMd.OrderBy(m => m.CreatedDate).ToList();
                    if (customerBelongingToShoutMd != null && customerBelongingToShoutMd.Count() > 0)
                    {
                        duplicateCustomerIDList = duplicateCustomerIDList.Except(customerIdOfCustomerBelongingToShoutMD).ToList();

                        duplicateCustomerIDList.Add(customerBelongingToShoutMd[0].CMDCustomerId);
                    }
                }

                ////Calling RPC to merge duplicate customers with existing customer
                bool isOperationSuccessful = MapCMDCustomerObjectToResolveDuplicateCustomer(existingCustomerId, duplicateCustomerIDList, existingCustomer, isNpiMatch);

                if (isOperationSuccessful)
                {
                    CMDLogger.LogAudit("Successfully completed Merge Operation", businessUnit);
                }
            }
        }

        /// <summary>
        /// This methods merges the duplicate customer record into one master customer record with
        /// selected values for merging for the fields Practices, Products, Speciality,
        /// Customer info. such as name, DOB, Phone, Email etc.
        /// </summary>
        /// <param name="existingCustomerId">Id of Existing customer</param>
        /// <param name="customerDuplicateIDsList">List of CustomerIDs of duplicate Customer</param>
        /// <param name="existingCustomer">CMDCustomer object of Existingcustomer</param>
        /// <returns></returns>
        private bool MapCMDCustomerObjectToResolveDuplicateCustomer(
            int existingCustomerId,
            List<int> customerDuplicateIDsList,
            CMDCustomer existingCustomer,
            bool isNpiMatch)
        {
            bool isOperationSuccessful = false;

            try
            {
                ALPHAEON.CMD.TransientMessage.CustomerForAutoMerge customerTransientMessageObject = new ALPHAEON.CMD.TransientMessage.CustomerForAutoMerge();

                customerTransientMessageObject.Customer = MapCMDCustomerToCustomerTransientMessage(existingCustomer);

                List<int> customerIdsForMerge = new List<int>();

                customerIdsForMerge.Add(existingCustomerId);

                customerIdsForMerge.AddRange(customerDuplicateIDsList);

                customerTransientMessageObject.UpdatedBy = ALPHAEON.CMD.Common.Constants.Configuration.UpdatedBy;

                customerTransientMessageObject.CustomerIDsForMerge = customerIdsForMerge;

                customerTransientMessageObject.CustomerDuplicateIDsList = customerDuplicateIDsList;

                isOperationSuccessful = MergeCustomers(customerTransientMessageObject, isNpiMatch);
            }
            catch (Exception ex)
            {
                isOperationSuccessful = false;

                CMDLogger.LogException(ex, ALPHAEON.CMD.Common.Constants.Configuration.Customer, ALPHAEON.CMD.Common.Constants.LinqQueries.CMDBusinessUnitNameCMD, "classAndMethodName");
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// Method for Bulk updating the status of customers after being AutoMerged to Processed
        /// </summary>
        /// <param name="customerStatusBulkUpdate">Object of CustomerStatusBulkUpdate</param>      
        private void CustomerstatusBulkUpdate(CustomerStatusBulkUpdate customerStatusBulkUpdate)
        {
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            using (var context = CMDDatabaseContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var businessUnit = context.CMDBusinessUnits.FirstOrDefault(m => m.IsActive && m.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDBusinessUnitNameCMD);

                    try
                    {
                        context.CMDCustomers.Where(x => customerStatusBulkUpdate.CustomerIDsToBeUpdatedList.Contains(x.Id)).ToList().ForEach(a =>
                        {
                            a.Status = ALPHAEON.CMD.Common.Constants.General.Processed;
                            a.UpdatedBy = ALPHAEON.CMD.Common.Constants.Configuration.UpdatedBy;
                            a.UpdatedDate = DateTime.UtcNow;
                        });

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        CMDLogger.LogException(ex, customerStatusBulkUpdate, businessUnit.Name, classAndMethodName);
                    }
                }
            }
        }

        /// <summary>
        /// Method for merging the list of duplicate customers with Existing customer
        /// </summary>
        /// <param name="dto">Object of TransientMessage.DashboardResolveDuplicateCustomer</param>
        /// <returns>Boolean value indiciating whether merge  was successful</returns>
        private bool MergeCustomers(CustomerForAutoMerge dto, bool isNpiMatch)
        {
            #region MergeCode
            string classAndMethodName = string.Format(ALPHAEON.CMD.Common.Constants.General.ClassAndMethodName, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            bool isOperationSuccessful = false;

            Alphaeon.Services.EnterpriseAPI.Models.CMDCustomer cmdCustomer = null;

            using (var context = CMDDatabaseContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var businessUnit = context.CMDBusinessUnits.FirstOrDefault(m => m.IsActive && m.Name == ALPHAEON.CMD.Common.Constants.LinqQueries.CMDBusinessUnitNameCMD);

                    try
                    {
                        if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                        {
                            ////Retrive Existing Customer
                            cmdCustomer = context.CMDCustomers
                               .Where(cmdCustomerRecord => cmdCustomerRecord.Id == dto.Customer.Id && cmdCustomerRecord.IsActive)
                               .Select(cmdCustomerRecord => cmdCustomerRecord).FirstOrDefault();

                            if (cmdCustomer != null)
                            {
                                #region Auto-Merge Scheduler - Make entries in CMDCustomerDuplicateMap table for existing and list of duplicate customers -- Starts
                                //// if the Patch operation is called to update and merge the duplicate customer record from Auto-Merge Scheduler

                                foreach (int customerDuplicateId in dto.CustomerDuplicateIDsList)
                                {
                                    var customerduplicateMapRecord = context.CMDCustomerDuplicateMap
                                        .Where(customerDuplicateMap => customerDuplicateMap.DuplicateCustomerID == customerDuplicateId &&
                                            customerDuplicateMap.ExistingCustomerID == cmdCustomer.Id).FirstOrDefault();

                                    if (customerduplicateMapRecord == null)
                                    {
                                        CMDCustomerDuplicateMap customerduplicateMapEntry = null;

                                        if (isNpiMatch)
                                        {
                                            var cmdCustomerDupMap = new CMDCustomerDuplicateMap
                                            {
                                                DuplicateCustomerID = customerDuplicateId,
                                                CreatedDate = DateTime.UtcNow,
                                                IsActive = true,
                                                ExistingCustomerID = cmdCustomer.Id,
                                                ResolveAction = null,
                                                UpdatedBy = dto.UpdatedBy,
                                                UpdatedDate = DateTime.UtcNow,
                                                Criteria = ALPHAEON.CMD.Common.Constants.General.NPIMatch
                                            };
                                            customerduplicateMapEntry = context.Create(cmdCustomerDupMap);
                                        }
                                        else
                                        {
                                            var cmdCustomerDupMap = new CMDCustomerDuplicateMap
                                            {
                                                DuplicateCustomerID = customerDuplicateId,
                                                CreatedDate = DateTime.UtcNow,
                                                IsActive = true,
                                                ExistingCustomerID = cmdCustomer.Id,
                                                ResolveAction = null,
                                                UpdatedBy = dto.UpdatedBy,
                                                UpdatedDate = DateTime.UtcNow,
                                                Criteria = ALPHAEON.CMD.Common.Constants.General.AutoMergeCriteria
                                            };
                                            customerduplicateMapEntry = context.Create(cmdCustomerDupMap);
                                        }

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerduplicateMapEntry.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerDuplicateMap.ToString(),
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                    else
                                    {
                                        var customerDuplicateMapDelta = new Dictionary<string, object>();
                                        customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.ResolveAction), null);
                                        customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.IsActive), true);
                                        if (isNpiMatch)
                                        {
                                            customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.Criteria), ALPHAEON.CMD.Common.Constants.General.NPIMatch);
                                        }
                                        else
                                        {
                                            customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.Criteria), ALPHAEON.CMD.Common.Constants.General.AutoMergeCriteria);
                                        }

                                        context.Update<Models.CMDCustomerDuplicateMap>((int)customerduplicateMapRecord.Id, customerDuplicateMapDelta, false);
                                    }
                                }

                                #endregion Auto-Merge Scheduler - Make entries in CMDCustomerDuplicateMap table for existing and list of duplicate customers -- Ends

                                #region Merge Code

                                #region Update CustomerSpecialities

                                bool mergeCustomerSpecialityOperation = MergeCustomerSpecialityMap(dto, context);

                                #endregion

                                #region Update CustomerPractices

                                bool mergeCustomerPracticeOperation = MergeCustomerPracticeMap(dto, context);

                                #endregion

                                #region Update CustomerProduct

                                bool mergeCustomerProductOperation = MergeCustomerProductMap(dto, context);

                                #endregion

                                #region Update CustomerContact

                                bool mergeCustomerContactOperation = MergeCustomerContactMap(dto, context);

                                #endregion

                                #region Update CustomerEmail

                                bool mergeCustomerEmailOperation = MergeCustomerEmails(dto, context);

                                #endregion

                                #region Update CustomerPhone

                                bool mergeCustomerPhoneOperation = MergeCustomerPhone(dto, context);

                                #endregion

                                #region Update CustomerShoutScoreAndShoutScoreHistory

                                bool mergeCustomershoutScoreAndShoutscoreHistory = MergeCustomerShoutScoreAndShoutHistory(dto, context);

                                #endregion
                                foreach (var duplicateCustomerId in dto.CustomerDuplicateIDsList)
                                {

                                    List<CMDCustomerSourceTrack> customerSourceTrack = context.CMDCustomerSourceTracks
                                       .Where(cmdCustomerSourceTrackRecord =>
                                                      cmdCustomerSourceTrackRecord.CMDCustomerID == duplicateCustomerId
                                           && cmdCustomerSourceTrackRecord.IsActive
                                           && cmdCustomerSourceTrackRecord.CMDCustomer.IsActive).ToList();

                                    //// Update  CustomerSourceTrack record
                                    #region Update CustomerSourceTrack record
                                    // Update Duplicate Customers CustomerSourceTrack record with CustomerID of Master Customer

                                    foreach (CMDCustomerSourceTrack cmdCustomersourceTrackRecord in customerSourceTrack)
                                    {
                                        var customerSourceTrackDelta = new Dictionary<string, object>();

                                        customerSourceTrackDelta.Add(DAL.PropertyName<Models.CMDCustomerSourceTrack>(x => x.CMDCustomerID), cmdCustomer.Id);

                                        customerSourceTrackDelta.Add(DAL.PropertyName<Models.CMDCustomerSourceTrack>(x => x.AlphaeonId), ALPHAEON.CMD.Common.CommonUtility.GenerateAlphaeonId(cmdCustomer.Id));

                                        customerSourceTrackDelta.Add(DAL.PropertyName<Models.CMDCustomerSourceTrack>(x => x.UpdatedBy), dto.UpdatedBy);

                                        customerSourceTrackDelta.Add(DAL.PropertyName<Models.CMDCustomerSourceTrack>(x => x.UpdatedDate), DateTime.UtcNow);

                                        ////customerSourceTrackDelta.Add(DAL.PropertyName<Models.CMDCustomerSourceTrack>(x => x.MergedWithCMDCustomerId), duplicateCustomerId);

                                        context.Update<Models.CMDCustomerSourceTrack>((int)cmdCustomersourceTrackRecord.Id, customerSourceTrackDelta, false);
                                    }

                                    ////Get customerSourceTrack record for duplicate customer record
                                    #endregion
                                }

                                foreach (var duplicateCustomerId in dto.CustomerDuplicateIDsList)
                                {
                                    var customerDuplicateMapEntry = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, cmdCustomer.Id, context);

                                    #region Update CustomerAccountMap records
                                    ////Get CMDCustomerAccountMap records for duplicate customers record

                                    List<CMDCustomerAccountMap> customerAccountMapList = context.CMDCustomerAccountMaps
                                        .Where(cmdCustomerAccountMapRecord =>
                                                       cmdCustomerAccountMapRecord.CMDCustomerID == duplicateCustomerId
                                            && cmdCustomerAccountMapRecord.IsActive
                                            && cmdCustomerAccountMapRecord.CMDAccount.IsActive
                                            && cmdCustomerAccountMapRecord.CMDCustomer.IsActive).ToList();

                                    List<int> existingCustomerAccountMapBUIDList = context.CMDCustomerAccountMaps
                                        .Where(cmdCustomerAccountMapRecord =>
                                               cmdCustomerAccountMapRecord.CMDCustomerID == cmdCustomer.Id
                                            && cmdCustomerAccountMapRecord.IsActive
                                            && cmdCustomerAccountMapRecord.CMDAccount.IsActive
                                            && cmdCustomerAccountMapRecord.CMDCustomer.IsActive)
                                            .Select(cmdCustomerAccountMapRecord => cmdCustomerAccountMapRecord.CMDAccount.CMDBusinessUnit.Id).ToList();

                                    if (customerAccountMapList != null && customerAccountMapList.Count() > 0)
                                    {
                                        //// Update  CMDCustomerAccountMap record

                                        foreach (CMDCustomerAccountMap customerAccountMap in customerAccountMapList)
                                        {
                                            /* If existing and duplicate Customer is having account for same business unit, then deactivate
                                         * account of duplicate customer for that business unit and update the references of deactivated account
                                         * to existing customers account
                                         */
                                            if (existingCustomerAccountMapBUIDList.Any(cmdBusinessUnitId => cmdBusinessUnitId == customerAccountMap.CMDAccount.CMDBusinessUnit.Id))
                                            {
                                                ////Get the existing customer's account for the same business unit
                                                int existingCustomerAccountId = context.CMDCustomerAccountMaps
                                                     .Where(cmdCustomerAccountMapRecord =>
                                                        cmdCustomerAccountMapRecord.CMDCustomerID == dto.Customer.Id
                                                     && cmdCustomerAccountMapRecord.IsActive
                                                     && cmdCustomerAccountMapRecord.CMDAccount.IsActive
                                                     && cmdCustomerAccountMapRecord.CMDCustomer.IsActive
                                                     && cmdCustomerAccountMapRecord.CMDAccount.CMDBusinessUnit.Id == customerAccountMap.CMDAccount.CMDBusinessUnit.Id)
                                                     .Select(cmdCustomerAccountMapRecord => cmdCustomerAccountMapRecord.CMDAccount.Id).FirstOrDefault();

                                                #region Update CMDTransactions of Duplicate Customer
                                                // Update CMDTransactions of Duplicate Customer with CMDAccountID of the existing customer for selected BU
                                                List<CMDTransaction> customerTransactionList = context.CMDTransactions
                                                       .Where(cmdTransactionRecord =>
                                                          cmdTransactionRecord.CMDAccountID == customerAccountMap.CMDAccount.Id
                                                       && cmdTransactionRecord.IsActive
                                                       && cmdTransactionRecord.CMDAccount.IsActive).ToList();
                                                if (customerTransactionList != null && customerTransactionList.Count() > 0)
                                                {
                                                    foreach (CMDTransaction cmdTransaction in customerTransactionList)
                                                    {
                                                        var transactionDelta = new Dictionary<string, object>();

                                                        transactionDelta.Add(DAL.PropertyName<Models.CMDTransaction>(x => x.CMDAccountID), existingCustomerAccountId);

                                                        transactionDelta.Add(DAL.PropertyName<Models.CMDTransaction>(x => x.UpdatedBy), dto.UpdatedBy);

                                                        transactionDelta.Add(DAL.PropertyName<Models.CMDTransaction>(x => x.UpdatedDate), DateTime.UtcNow);

                                                        context.Update<Models.CMDTransaction>((int)cmdTransaction.Id, transactionDelta, false);
                                                    }
                                                }
                                                #endregion

                                                #region Update CMDAccountContactMap of Duplicate Customer
                                                // Update CMDTransactions of Duplicate Customer with CMDAccountID of the existing customer for selected BU
                                                // Update CMDTransactions of Duplicate Customer with CMDAccountID of the existing customer for selected BU
                                                List<CMDAccountContactMap> accountContactMapList = context.CMDAccountContactMaps
                                                       .Where(cmdAccountContactMapRecord =>
                                                          cmdAccountContactMapRecord.CMDAccountID == customerAccountMap.CMDAccount.Id
                                                       && cmdAccountContactMapRecord.IsActive
                                                       && cmdAccountContactMapRecord.CMDAccount.IsActive).ToList();
                                                if (accountContactMapList != null && accountContactMapList.Count() > 0)
                                                {
                                                    foreach (CMDAccountContactMap cmdAccountContactMap in accountContactMapList)
                                                    {
                                                        var accountContactMapDelta = new Dictionary<string, object>();

                                                        accountContactMapDelta.Add(DAL.PropertyName<Models.CMDAccountContactMap>(x => x.CMDAccountID), existingCustomerAccountId);

                                                        accountContactMapDelta.Add(DAL.PropertyName<Models.CMDAccountContactMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                                        accountContactMapDelta.Add(DAL.PropertyName<Models.CMDAccountContactMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                                        context.Update<Models.CMDAccountContactMap>((int)cmdAccountContactMap.Id, accountContactMapDelta, false);
                                                    }
                                                }
                                                #endregion

                                                #region Update CMDAccountSourceTrack of Duplicate Customer
                                                ////Update AccountSourceTrack record entries of deactivated account to the existing account's AccountSourceRecord entries

                                                ////Get AccountSourceTrack entries for account of duplicate customer
                                                var accountSourceTrackListOfDuplicatCustomerAccount = context.CMDAccountSourceTracks
                                                    .Where(cmdAccountSourceTrackRecord =>
                                                        cmdAccountSourceTrackRecord.CMDAccountID == customerAccountMap.CMDAccountID &&
                                                        cmdAccountSourceTrackRecord.IsActive &&
                                                        cmdAccountSourceTrackRecord.CMDAccount.IsActive)
                                                    .Select(cmdAccountSourceTrackRecord => cmdAccountSourceTrackRecord).ToList();
                                                ////Update CMDAccountSourceTrack of Duplicate Customer 
                                                if (accountSourceTrackListOfDuplicatCustomerAccount != null && accountSourceTrackListOfDuplicatCustomerAccount.Count() > 0)
                                                {
                                                    foreach (CMDAccountSourceTrack cmdAccountSourceTrack in accountSourceTrackListOfDuplicatCustomerAccount)
                                                    {
                                                        var accountSourceTrackDelta = new Dictionary<string, object>();

                                                        accountSourceTrackDelta.Add(DAL.PropertyName<Models.CMDAccountSourceTrack>(x => x.CMDAccountID), existingCustomerAccountId);

                                                        accountSourceTrackDelta.Add(DAL.PropertyName<Models.CMDAccountSourceTrack>(x => x.UpdatedBy), dto.UpdatedBy);

                                                        accountSourceTrackDelta.Add(DAL.PropertyName<Models.CMDAccountSourceTrack>(x => x.UpdatedDate), DateTime.UtcNow);

                                                        context.Update<Models.CMDAccountSourceTrack>((int)cmdAccountSourceTrack.Id, accountSourceTrackDelta, false);

                                                        ////Make entry in Merge Audit log

                                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                                        {
                                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMapEntry.Id,
                                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                                            NewValue = existingCustomerAccountId.ToString(),
                                                            OldValue = cmdAccountSourceTrack.CMDAccountID.ToString(),
                                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.CMDAccountID.ToString(),
                                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDAccountSourceTrack.ToString(),
                                                            RecordID = cmdAccountSourceTrack.Id,
                                                            AuditLogDate = DateTime.UtcNow
                                                        };
                                                        context.Create(autoMergeAuditLog);
                                                    }
                                                }

                                                #endregion Update CMDAccountSourceTrack of Duplicate Customer--Ends

                                                #region Delete CMDCustomerAccountMap record for Duplicate Customer

                                                var customeAccountMapDelta = new Dictionary<string, object>();

                                                customeAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.IsActive), false);

                                                customeAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                                customeAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                                context.Update<Models.CMDCustomerAccountMap>((int)customerAccountMap.Id, customeAccountMapDelta, false);

                                                #endregion

                                                #region Delete CMDAccount of Duplicate Customer

                                                CMDAccount duplicateCustomerAccount = context.CMDAccounts
                                                    .Where(cmdAccount => cmdAccount.Id == customerAccountMap.CMDAccount.Id &&
                                                    cmdAccount.IsActive).FirstOrDefault();

                                                var accountDelta = new Dictionary<string, object>();

                                                accountDelta.Add(DAL.PropertyName<Models.CMDAccount>(x => x.IsActive), false);

                                                accountDelta.Add(DAL.PropertyName<Models.CMDAccount>(x => x.UpdatedBy), dto.UpdatedBy);

                                                accountDelta.Add(DAL.PropertyName<Models.CMDAccount>(x => x.UpdatedDate), DateTime.UtcNow);

                                                context.Update<Models.CMDAccount>((int)duplicateCustomerAccount.Id, accountDelta, false);

                                                #endregion
                                            }
                                            else
                                            {
                                                ////Else update CustomerContact map record for duplicate customer to refer existing customer
                                                //// Assign duplicate customer's account to master customer and update the customer account map record

                                                var customerAccountMapDelta = new Dictionary<string, object>();

                                                customerAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.CMDCustomerID), dto.Customer.Id);

                                                customerAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                                customerAccountMapDelta.Add(DAL.PropertyName<Models.CMDCustomerAccountMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                                context.Update<Models.CMDCustomerAccountMap>((int)customerAccountMap.Id, customerAccountMapDelta, false);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update CustomerDuplicateMap

                                    // Get all CustomerDuplicateMap records having DuplicateCustomerID same as param DuplicateCustomerID
                                    List<CMDCustomerDuplicateMap> cmdCustomerDuplicateMapList = context.CMDCustomerDuplicateMap
                                        .Where(cmdCustomerDuplicateMapRecord =>
                                        cmdCustomerDuplicateMapRecord.IsActive
                                                && cmdCustomerDuplicateMapRecord.DuplicateCustomerID == duplicateCustomerId
                                        && cmdCustomerDuplicateMapRecord.ResolveAction == null).ToList();

                                    if (cmdCustomerDuplicateMapList != null && cmdCustomerDuplicateMapList.Count() > 0)
                                    {
                                        foreach (CMDCustomerDuplicateMap customerDuplicateMap in cmdCustomerDuplicateMapList)
                                        {
                                            ////Delete the CustomerDuplicateMap record which got merged in customer record

                                            var customerDuplicateMapDelta = new Dictionary<string, object>();

                                            customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.IsActive), false);

                                            ////Duplicate Record is getting merged with one existing record only hence resolve action will be merged

                                            if (customerDuplicateMap.ExistingCustomerID == dto.Customer.Id)
                                            {
                                                ////For manual merge from Scheduler resolve action = AutoMergedFromScheduler
                                                customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.ResolveAction), (int)ALPHAEON.CMD.Common.Enums.DuplicateRecordResolveOperations.AutoMergedFromScheduler);
                                            }
                                            else
                                            {
                                                customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.ResolveAction), null);
                                            }

                                            customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                            customerDuplicateMapDelta.Add(DAL.PropertyName<Models.CMDCustomerDuplicateMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                            context.Update<Models.CMDCustomerDuplicateMap>((int)customerDuplicateMap.Id, customerDuplicateMapDelta, false);
                                        }
                                    }

                                    #endregion

                                    #region Delete Duplicate Customer

                                    //// Delete (inactivate) Duplicate Customer Record

                                    var duplicateCustomerdelta = new Dictionary<string, object>();

                                    duplicateCustomerdelta.Add(DAL.PropertyName<Models.CMDCustomer>(x => x.IsActive), false);

                                    duplicateCustomerdelta.Add(DAL.PropertyName<Models.CMDCustomer>(x => x.UpdatedBy), dto.UpdatedBy);

                                    duplicateCustomerdelta.Add(DAL.PropertyName<Models.CMDCustomer>(x => x.UpdatedDate), DateTime.UtcNow);

                                    cmdCustomer = context.Update<Models.CMDCustomer>(duplicateCustomerId, duplicateCustomerdelta, false);

                                    #endregion
                                }

                                if (mergeCustomerSpecialityOperation && mergeCustomerContactOperation &&
                                    mergeCustomerPracticeOperation && mergeCustomerProductOperation &&
                                    mergeCustomerEmailOperation && mergeCustomerPhoneOperation && mergeCustomershoutScoreAndShoutscoreHistory)
                                {
                                    isOperationSuccessful = true;
                                    CMDLogger.LogAudit("Successfully Auto-Merged records", businessUnit.Name);
                                }
                            }
                        }
                        #endregion
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        isOperationSuccessful = false;
                        CMDLogger.LogException(ex, dto, businessUnit.Name, classAndMethodName);
                    }

                    return isOperationSuccessful;
                }
            }
            #endregion
        }

        /// <summary>
        /// This method returns list of Email from CMDCustomerEmail associated with the customer which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>List of Email associated with the customer</returns>
        private List<CMDCustomerEmail> GetCMDCustomerEmailsRecord(int customerId, CMDDatabaseContext context)
        {
            var customerEmailsList = context.CMDCustomerEmails
                                   .Where(cmdCustomerEmail => cmdCustomerEmail.CMDCustomerID == customerId
                                    && cmdCustomerEmail.IsActive).ToList();

            return customerEmailsList;
        }

        /// <summary>
        /// This method will return object of CustomerDuplicate map entry if record is present for passed existing and duplicate customer
        /// </summary>
        /// <param name="duplicateCustomerId">Id of ducplicate Customer</param>
        /// <param name="existingCustomerId">Id of existing Customer</param>
        /// <param name="context">Context of CMD DB</param>
        /// <returns>return object of CustomerDuplicate map entry if record is present for passed existing and duplicate customer</returns>
        private CMDCustomerDuplicateMap GetCmdCustomerDuplicateMapRecord(int duplicateCustomerId, int existingCustomerId, CMDDatabaseContext context)
        {
            var customerDuplicateMap = context.CMDCustomerDuplicateMap
                               .Where(cmdCustomerDuplicateMapRecord =>
                               cmdCustomerDuplicateMapRecord.DuplicateCustomerID == duplicateCustomerId
                               && cmdCustomerDuplicateMapRecord.ExistingCustomerID == existingCustomerId).FirstOrDefault();

            return customerDuplicateMap;
        }

        /// <summary>
        /// This method returns list of Phone from CMDCustomerPhone associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of Phone associated with the customer</returns>
        private List<CMDCustomerPhone> GetCMDCustomerPhoneRecord(int customerId, CMDDatabaseContext context)
        {
            var customerPhoneList = context.CMDCustomerPhones
                                   .Where(cmdCustomerPhone => cmdCustomerPhone.CMDCustomerID == customerId
                                    && cmdCustomerPhone.IsActive && cmdCustomerPhone.CMDCustomer.IsActive).ToList();

            return customerPhoneList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerspecialityMap from CMDCustomerSpecialityMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of customerSpecialityMap entry</returns>
        private List<CMDCustomerSpecialityMap> GetCmdCustomerSpecialityMapRecord(int customerId, CMDDatabaseContext context)
        {
            var specialitiesList = context.CMDCustomerSpecialityMaps
                                  .Where(cmdCustomerSpecialityMap => cmdCustomerSpecialityMap.CMDCustomerID == customerId
                                    && cmdCustomerSpecialityMap.IsActive
                                    && cmdCustomerSpecialityMap.CMDSpeciality.IsActive
                                    && cmdCustomerSpecialityMap.CMDCustomer.IsActive).ToList();

            return specialitiesList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerPracticeMap from CMDCustomerPracticeMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of CMDCustomerPracticeMap entry</returns>
        private List<CMDCustomerPracticeMap> GetCmdCustomerPracticeMapRecord(int customerId, CMDDatabaseContext context)
        {
            var customerPracticesList = context.CMDCustomerPracticeMaps
                                  .Where(cmdCustomerPracticeMap => cmdCustomerPracticeMap.CMDCustomerID == customerId
                                   && cmdCustomerPracticeMap.IsActive
                                   && cmdCustomerPracticeMap.CMDPractice.IsActive
                                   && cmdCustomerPracticeMap.CMDCustomer.IsActive).ToList();

            return customerPracticesList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerProductMap from CMDCustomerProductMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of CMDCustomerProductMap entry</returns>
        private List<CMDCustomerProductMap> GetCmdCustomerProductMapRecord(int customerId, CMDDatabaseContext context)
        {
            var customerProductsList = context.CMDCustomerProductMaps
                                   .Where(cmdCustomerProductMap => cmdCustomerProductMap.CMDCustomerID == customerId
                                    && cmdCustomerProductMap.IsActive
                                    && cmdCustomerProductMap.CMDProduct.IsActive
                                    && cmdCustomerProductMap.CMDCustomer.IsActive).ToList();

            return customerProductsList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerContactMap from CMDCustomerContactMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of CMDCustomerContactMap entry</returns>
        private List<CMDCustomerContactMap> GetCmdCustomerContactMapRecord(int customerId, CMDDatabaseContext context)
        {
            var customerContactList = context.CMDCustomerContactMaps
                                            .Where(cmdCustomerContactMapRecord =>
                                                      cmdCustomerContactMapRecord.CMDCustomerID == customerId &&
                                                      cmdCustomerContactMapRecord.IsActive &&
                                                      cmdCustomerContactMapRecord.CMDContact.IsActive && cmdCustomerContactMapRecord.CMDCustomer.IsActive).ToList();

            return customerContactList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerAccountMap from CMDCustomerAccountMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of CMDCustomerAccountMap entry</returns>
        private List<CMDCustomerAccountMap> GetCmdCustomerAccountMapRecord(int customerId, CMDDatabaseContext context)
        {
            var customerAccountList = context.CMDCustomerAccountMaps
                                .Where(cmdCustomerAccountMapRecord =>
                                       cmdCustomerAccountMapRecord.CMDCustomerID == customerId
                                    && cmdCustomerAccountMapRecord.IsActive
                                    && cmdCustomerAccountMapRecord.CMDAccount.IsActive
                                    && cmdCustomerAccountMapRecord.CMDCustomer.IsActive).ToList();

            return customerAccountList;
        }

        /// <summary>
        /// This method returns list of CMDCustomerDuplicateMap from CMDCustomerDuplicateMap associated with the customer  which is passed as param.
        /// </summary>
        /// <param name="customerId">CMDCustomerId </param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>list of CMDCustomerDuplicateMap entry</returns>
        private List<CMDCustomerDuplicateMap> GetCmdCustomerDuplicateMapRecord(int customerId, CMDDatabaseContext context)
        {
            var customerDuplicateList = context.CMDCustomerDuplicateMap
                                .Where(cmdCustomerDuplicateMapRecord =>
                                cmdCustomerDuplicateMapRecord.IsActive
                                && cmdCustomerDuplicateMapRecord.DuplicateCustomerID == customerId
                                && cmdCustomerDuplicateMapRecord.ResolveAction == null).ToList();

            return customerDuplicateList;
        }

        /// <summary>
        /// This method merges Specialities associated with duplicate customer into Specialities of existing customer
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Specialities merged successfully else false</returns>
        private bool MergeCustomerSpecialityMap(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerIDsForMerge != null && dto.CustomerIDsForMerge.Count > 0)
                {
                    ////Select specialities of existing
                    List<int> existingCustomerSpecialityIdsList = new List<int>();
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);
                    existingCustomerSpecialityIdsList = GetCMDSpecialityIDsListforCMDCustomer(existingCustomerId, context);

                    var existingCustomer = context.CMDCustomers.Where(customer => customer.Id == existingCustomerId).FirstOrDefault();
                    //// assign specialities of duplicate customer to existing
                    #region Check if CustomerIDsforSelectedSpecialitiesList contains duplicateCustomerId -- Starts

                    if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                    {
                        List<int> newSpecialitiesFromDuplicateCustomer = new List<int>();

                        foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                        {
                            var customerDuplicateMap = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                            #region Assigning Specialities of Duplicate Customer to Existing Customer -- Starts
                            ////Select specilaities of duplicate
                            List<int> duplicateCustomerSpecialityIdsList = new List<int>();
                            duplicateCustomerSpecialityIdsList = GetCMDSpecialityIDsListforCMDCustomer(duplicateCustomerId, context);

                            if (duplicateCustomerSpecialityIdsList != null && duplicateCustomerSpecialityIdsList.Count() > 0)
                            {
                                newSpecialitiesFromDuplicateCustomer = duplicateCustomerSpecialityIdsList.Except(existingCustomerSpecialityIdsList).ToList();
                                ////loop of duplicateSpecialities list
                                foreach (int specialityId in newSpecialitiesFromDuplicateCustomer)
                                {
                                    var customerSpecialityMap = context.CMDCustomerSpecialityMaps
                                              .Where(x => x.CMDSpecialityID == specialityId && x.CMDCustomerID == existingCustomerId && !x.IsActive)
                                                               .FirstOrDefault();

                                    if (customerSpecialityMap != null)
                                    {
                                        UpdateCustomerSpecialityMap(dto, context, customerSpecialityMap);

                                        CreateAutoMergeAuditLog(context, customerDuplicateMap, customerSpecialityMap);
                                    }
                                    else
                                    {
                                        #region If record not present: Add new CustomerSpecialityMap record -- Starts

                                        if (!existingCustomerSpecialityIdsList.Contains(specialityId))
                                        {
                                            CMDCustomerSpecialityMap newCustomerSpecialityMap = CreateCustomerSpecialityMap(dto, context, existingCustomerId, existingCustomer, specialityId);

                                            #endregion If record not present: Add new CustomerSpecialityMap record -- Ends

                                            var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                            {
                                                CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerSpecialityMap.ToString(),
                                                RecordID = newCustomerSpecialityMap.Id,
                                                AuditLogDate = DateTime.UtcNow
                                            };
                                            context.Create(autoMergeAuditLog);
                                        }
                                    }
                                }
                            }
                            #endregion Assigning Specialities of Duplicate Customer to Existing Customer -- Ends

                            #region Delete CustomerSpecialityMap for Duplicate Customer -- Starts

                            // After merge, duplicate customer will get deleted hence we need to delete its assigned Specialities from CMDCustomerSpecialityMap table

                            // For duplicate customer, get all the active CMDCustomerSpecialityMap records, which needs to be deleted

                            ////get list of CMDCustomerSpecialityMap Records for duplicate customer
                            List<CMDCustomerSpecialityMap> duplicateCustomerSpecialityMapList =
                                GetCmdCustomerSpecialityMapRecord(duplicateCustomerId, context);

                            //// If retrieved records are not null then delete all records in the list

                            if (duplicateCustomerSpecialityMapList != null && duplicateCustomerSpecialityMapList.Count() > 0)
                            {
                                foreach (CMDCustomerSpecialityMap cmdCustomerSpecialityMap in duplicateCustomerSpecialityMapList)
                                {
                                    DeleteCustomerSpecialityMap(dto, context, customerDuplicateMap, cmdCustomerSpecialityMap);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion Check if CustomerIDsforSelectedSpecialitiesList contains duplicateCustomerId -- Ends
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges Practices associated with duplicate customer into Practices of existing customer
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Practices merged successfully else false</returns>
        private bool MergeCustomerPracticeMap(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerIDsForMerge != null && dto.CustomerIDsForMerge.Count > 0)
                {
                    ////Select practices of existing
                    List<int> existingCustomerPracticeIdsList = new List<int>();
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);
                    existingCustomerPracticeIdsList = GetCMDPracticeIDsListforCMDCustomer(existingCustomerId, context);

                    // assign practices of duplicate customer to existing
                    #region Check if CustomerIDsforSelectedPracticesList contains duplicateCustomerId -- Starts

                    if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                    {
                        List<int> newPracticesFromDuplicateCustomer = new List<int>();

                        foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                        {
                            var customerDuplicateMap = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                            #region Assigning Practices of Duplicate Customer to Existing Customer -- Starts
                            ////Select specilaities of duplicate
                            List<int> duplicateCustomerPracticeIdsList = new List<int>();
                            duplicateCustomerPracticeIdsList = GetCMDPracticeIDsListforCMDCustomer(duplicateCustomerId, context);

                            if (duplicateCustomerPracticeIdsList != null && duplicateCustomerPracticeIdsList.Count() > 0)
                            {
                                newPracticesFromDuplicateCustomer = duplicateCustomerPracticeIdsList.Except(existingCustomerPracticeIdsList).ToList();
                                ////loop of duplicatePractices list
                                foreach (int practiceId in newPracticesFromDuplicateCustomer)
                                {
                                    var customerPracticeMap = context.CMDCustomerPracticeMaps
                                          .Where(x => x.CMDPracticeID == practiceId && x.CMDCustomerID == existingCustomerId && !x.IsActive)
                                                           .FirstOrDefault();

                                    if (customerPracticeMap != null)
                                    {
                                        #region If record present and deactivated: Update/activate CustomerPracticeMap record --Starts

                                        var practiceMapDelta = new Dictionary<string, object>();

                                        practiceMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.IsActive), true);
                                        practiceMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                        practiceMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                        context.Update<Models.CMDCustomerPracticeMap>((int)customerPracticeMap.Id, practiceMapDelta, false);

                                        #endregion If record present and deactivated: Update/activate CustomerPracticeMap record --Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                            NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                            OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPracticeMap.ToString(),
                                            RecordID = customerPracticeMap.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                    else
                                    {
                                        #region If record not present: Add new CustomerPracticeMap record -- Starts

                                        var newCustomerPracticeMap = new CMDCustomerPracticeMap
                                        {
                                            CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                            CMDPracticeID = practiceId,

                                            IsActive = true,

                                            UpdatedBy = dto.UpdatedBy,

                                            UpdatedDate = DateTime.UtcNow,

                                            CreatedDate = DateTime.UtcNow
                                        };
                                        context.Create(newCustomerPracticeMap);
                                        #endregion If record not present: Add new CustomerPracticeMap record -- Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPracticeMap.ToString(),
                                            RecordID = newCustomerPracticeMap.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                }
                            }
                            #endregion Assigning Practices of Duplicate Customer to Existing Customer -- Ends

                            #region Delete CustomerPracticeMap for Duplicate Customer -- Starts

                            // After merge, duplicate customer will get deleted hence we need to delete its assigned Practices from CMDCustomerPracticeMap table
                            // For duplicate customer get all the active CMDCustomerPracticeMap records, which needs to be deleted

                            ////get list of CMDCustomerPracticeMap Records for duplicate customer
                            List<CMDCustomerPracticeMap> duplicateCustomerPracticeMapList =
                                GetCmdCustomerPracticeMapRecord(duplicateCustomerId, context);

                            //// If retrieved records are not null then delete all records in the list

                            if (duplicateCustomerPracticeMapList != null && duplicateCustomerPracticeMapList.Count() > 0)
                            {
                                foreach (CMDCustomerPracticeMap cmdCustomerPracticeMap in duplicateCustomerPracticeMapList)
                                {
                                    ////Delete  CMDCustomerPracticeMap record

                                    var customerPracticeMapDelta = new Dictionary<string, object>();

                                    customerPracticeMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.IsActive), false);

                                    customerPracticeMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                    customerPracticeMapDelta.Add(DAL.PropertyName<Models.CMDCustomerPracticeMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                    context.Update<Models.CMDCustomerPracticeMap>((int)cmdCustomerPracticeMap.Id, customerPracticeMapDelta, false);

                                    var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPracticeMap.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                        OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                        NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                        RecordID = cmdCustomerPracticeMap.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion Check if CustomerIDsforSelectedPracticesList contains duplicateCustomerId -- Ends
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges Products associated with duplicate customer into Products of existing customer
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Products merged successfully else false</returns>
        private bool MergeCustomerProductMap(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerIDsForMerge != null && dto.CustomerIDsForMerge.Count > 0)
                {
                    ////Select products of existing
                    List<int> existingCustomerProductIdsList = new List<int>();
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);
                    existingCustomerProductIdsList = GetCMDProductIDsListforCMDCustomer(existingCustomerId, context);

                    // assign products of duplicate customer to existing
                    #region Check if CustomerIDsforSelectedProductsList contains duplicateCustomerId -- Starts

                    if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                    {
                        List<int> newProductsFromDuplicateCustomer = new List<int>();

                        foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                        {
                            var customerDuplicateMap = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                            #region Assigning Products of Duplicate Customer to Existing Customer -- Starts
                            ////Select products of duplicate
                            List<int> duplicateCustomerProductIdsList = new List<int>();
                            duplicateCustomerProductIdsList = GetCMDProductIDsListforCMDCustomer(duplicateCustomerId, context);

                            if (duplicateCustomerProductIdsList != null && duplicateCustomerProductIdsList.Count() > 0)
                            {
                                newProductsFromDuplicateCustomer = duplicateCustomerProductIdsList.Except(existingCustomerProductIdsList).ToList();
                                ////loop of duplicateProducts list
                                foreach (int productId in newProductsFromDuplicateCustomer)
                                {
                                    var customerProductMap = context.CMDCustomerProductMaps
                                              .Where(x => x.CMDProductID == productId && x.CMDCustomerID == existingCustomerId && !x.IsActive)
                                                               .FirstOrDefault();

                                    if (customerProductMap != null)
                                    {
                                        #region If record present and deactivated: Update/activate CustomerProductMap record --Starts

                                        var productMapDelta = new Dictionary<string, object>();

                                        productMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.IsActive), true);
                                        productMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                        productMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                        context.Update<Models.CMDCustomerProductMap>((int)customerProductMap.Id, productMapDelta, false);

                                        #endregion If record present and deactivated: Update/activate CustomerProductMap record --Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                            NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                            OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerProductMap.ToString(),
                                            RecordID = customerProductMap.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                    else
                                    {
                                        #region If record not present: Add new CustomerProductMap record -- Starts

                                        var newCustomerProductMap = new CMDCustomerProductMap
                                        {
                                            CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                            CMDProductID = productId,

                                            IsActive = true,

                                            UpdatedBy = dto.UpdatedBy,

                                            UpdatedDate = DateTime.UtcNow,

                                            CreatedDate = DateTime.UtcNow
                                        };
                                        context.Create(newCustomerProductMap);
                                        #endregion If record not present: Add new CustomerProductMap record -- Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerProductMap.ToString(),
                                            RecordID = newCustomerProductMap.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                }
                            }
                            #endregion Assigning Products of Duplicate Customer to Existing Customer -- Ends

                            #region Delete CustomerProductMap for Duplicate Customer -- Starts

                            // After merge, duplicate customer will get deleted hence we need to delete its assigned Products from CMDCustomerProductMap table
                            // For duplicate customer get all the active CMDCustomerProductMap records, which needs to be deleted

                            ////get list of CMDCustomerProductMap Records for duplicate customer
                            List<CMDCustomerProductMap> duplicateCustomerProductMapList =
                                GetCmdCustomerProductMapRecord(duplicateCustomerId, context);

                            //// If retrieved records are not null then delete all records in the list

                            if (duplicateCustomerProductMapList != null && duplicateCustomerProductMapList.Count() > 0)
                            {
                                foreach (CMDCustomerProductMap cmdCustomerProductMap in duplicateCustomerProductMapList)
                                {
                                    ////Delete  CMDCustomerProductMap record

                                    var customerProductMapDelta = new Dictionary<string, object>();

                                    customerProductMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.IsActive), false);

                                    customerProductMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                    customerProductMapDelta.Add(DAL.PropertyName<Models.CMDCustomerProductMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                    context.Update<Models.CMDCustomerProductMap>((int)cmdCustomerProductMap.Id, customerProductMapDelta, false);

                                    var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerProductMap.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                        OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                        NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                        RecordID = cmdCustomerProductMap.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion Check if CustomerIDsforSelectedProductsList contains duplicateCustomerId -- Ends
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges Contacts associated with duplicate customer into Contacts
        /// of existing customer in case of Auto-Merge Scheduler        
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Contacts merged successfully else false</returns>
        private bool MergeCustomerContactMap(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                ////Select contacts of existing
                List<int> existingCustomerContactIdsList = new List<int>();
                int existingCustomerId = Convert.ToInt32(dto.Customer.Id);

                existingCustomerContactIdsList = GetCMDContactIDsListforCMDCustomer(existingCustomerId, context);
                dto.SelectedContactIDsList = new List<int>();
                dto.SelectedContactIDsList.AddRange(existingCustomerContactIdsList);

                if (dto.SelectedContactIDsList != null && dto.SelectedContactIDsList.Count > 0)
                {
                    ////Retrieve all the CustomerContactMap records for the Existing Customer

                    List<CMDCustomerContactMap> assignedContactMapList = GetCmdCustomerContactMapRecord(existingCustomerId, context);

                    foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                    {
                        var customerDuplicateMapRecord = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                        List<int> newContactIDsfromDuplicateCustomer = new List<int>();

                        newContactIDsfromDuplicateCustomer = GetCMDContactIDsListforCMDCustomer(duplicateCustomerId, context);
                        if (newContactIDsfromDuplicateCustomer != null && newContactIDsfromDuplicateCustomer.Count() > 0)
                        {
                            #region Update/Add CustomerContacts

                            bool isDefaultContactPresentForExistingCustomer = false;
                            bool isDefaultContactPresentForDuplicateCustomer = false;
                            int contactIDForDefaultContactOfExistingCustomer = 0;
                            int contactIDForDefaultContactOfDuplicateCustomer = 0;

                            ////check for Default contact is already assigned to Existing Customer
                            if (assignedContactMapList != null && assignedContactMapList.Count > 0)
                            {
                                #region Update Contact for Duplicate Customer if set to Default

                                isDefaultContactPresentForExistingCustomer =
                               assignedContactMapList.Any(cmdCustomerContactMap => cmdCustomerContactMap.CMDContact.IsDefault);

                                if (isDefaultContactPresentForExistingCustomer)
                                {
                                    contactIDForDefaultContactOfExistingCustomer =
                                   assignedContactMapList.Where(cmdCustomerContactMap => cmdCustomerContactMap.CMDContact.IsDefault).Select(cmdCustomerContactMap => cmdCustomerContactMap).FirstOrDefault().CMDContactID;
                                }
                            }

                            ////check for Default contact is already assigned to Duplicate Customer
                            isDefaultContactPresentForDuplicateCustomer = context.CMDCustomerContactMaps
                                .Where(cmdCustomerContactMap => cmdCustomerContactMap.CMDContact.IsActive &&
                                    cmdCustomerContactMap.IsActive &&
                                cmdCustomerContactMap.CMDContact.IsDefault &&
                            cmdCustomerContactMap.CMDCustomerID == duplicateCustomerId).Any();

                            if (isDefaultContactPresentForDuplicateCustomer)
                            {
                                contactIDForDefaultContactOfDuplicateCustomer = context.CMDCustomerContactMaps
                                    .Where(cmdCustomerContactMap => cmdCustomerContactMap.CMDContact.IsActive && cmdCustomerContactMap.IsActive &&
                                cmdCustomerContactMap.CMDContact.IsDefault &&
                            cmdCustomerContactMap.CMDCustomerID == duplicateCustomerId)
                                .Select(cmdCustomerContactMap => cmdCustomerContactMap).FirstOrDefault().CMDContactID;
                            }
                            ////If existing and duplicate customer is having contact as default contact, update contact for duplicate customer to set isDefault to false
                            if (isDefaultContactPresentForExistingCustomer && isDefaultContactPresentForDuplicateCustomer
                                && contactIDForDefaultContactOfExistingCustomer != 0 && contactIDForDefaultContactOfDuplicateCustomer != 0)
                            {
                                CMDContact contact = context.CMDContacts.Where(cmdContact => cmdContact.Id == contactIDForDefaultContactOfDuplicateCustomer && cmdContact.Id != contactIDForDefaultContactOfExistingCustomer).FirstOrDefault();
                                if (contact != null)
                                {
                                    var contactDelta = new Dictionary<string, object>();
                                    contactDelta.Add(DAL.PropertyName<Models.CMDContact>(x => x.IsDefault), false);
                                    contactDelta.Add(DAL.PropertyName<Models.CMDContact>(x => x.UpdatedBy), dto.UpdatedBy);
                                    context.Update<Models.CMDContact>((int)contact.Id, contactDelta, false);

                                    var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                        NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isDefault.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDContact.ToString(),
                                        RecordID = contact.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog1);
                                }
                                else
                                {
                                    ////CMDLogger.LogBusinessException(string.Format("contact ID not found in Contacts Table for Contact ID : {0}", contactIDForDefaultContactOfDuplicateCustomer), dto, businessUnit.Name, this.GetAccessToken(), this.GetLogger());
                                }
                            }

                            #endregion

                            foreach (int contactID in newContactIDsfromDuplicateCustomer)
                            {
                                if (!assignedContactMapList.Any(customerContactMapRecord => customerContactMapRecord.CMDContactID == contactID))
                                {
                                    #region Add if contact is not present --Starts

                                    // Retreive BusinessUnit ID of from the sourcetrack record of contact which is going to be merged
                                    int? cmdBusinessUnitId = context.CMDContactSourceTracks
                                        .Where(
                                            cmdContactSourceTrack => cmdContactSourceTrack.IsActive &&
                                            cmdContactSourceTrack.CMDContact.IsActive &&
                                            cmdContactSourceTrack.CMDContactID == contactID)
                                        .Select(cmdContactSourceTrack => cmdContactSourceTrack.BusinessUnitID).FirstOrDefault();

                                    ////Add new CustomerContactMap
                                    var newCustomerContactMap = new CMDCustomerContactMap
                                    {
                                        CMDCustomerID = existingCustomerId,

                                        CMDContactID = contactID,

                                        IsActive = true,

                                        CMDBusinessUnitID = cmdBusinessUnitId,

                                        UpdatedBy = dto.UpdatedBy,

                                        UpdatedDate = DateTime.UtcNow,

                                        CreatedDate = DateTime.UtcNow
                                    };
                                    context.Create(newCustomerContactMap);

                                    var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerContactMap.ToString(),
                                        RecordID = newCustomerContactMap.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog1);
                                    #endregion Add if contact is not present --Ends
                                }
                                else
                                {
                                    #region Update/Activate if contact is present and inactive --Starts

                                    var cmdCustomerContactMap = context.CMDCustomerContactMaps
                                        .Where(cmdCustomerContactMapRecord =>
                                            cmdCustomerContactMapRecord.CMDContactID == contactID
                                        && cmdCustomerContactMapRecord.CMDCustomerID == dto.Customer.Id).FirstOrDefault();
                                    ////If record present: Update CustomerContactMap record

                                    if (cmdCustomerContactMap != null && !cmdCustomerContactMap.IsActive)
                                    {
                                        var contactMapDelta = new Dictionary<string, object>();

                                        contactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.IsActive), true);

                                        contactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                        contactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                        context.Update<Models.CMDCustomerContactMap>((int)cmdCustomerContactMap.Id, contactMapDelta, false);

                                        var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                            OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                            NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerContactMap.ToString(),
                                            RecordID = cmdCustomerContactMap.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog1);
                                    }

                                    #endregion Update/Activate if contact is present and inactive --Ends
                                }
                            }
                            #endregion
                        }

                        #region Delete CustomerContactMap for Duplicate Customer

                        // After merge, duplicate customer will get deleted, hence we need to delete its assigned Contacts from CMDCustomerContactMap table
                        //// For selected customer get all the active CMDCustomerContactMap records, which needs to be deleted

                        List<CMDCustomerContactMap> customerContactMapListToBeDeleted = GetCmdCustomerContactMapRecord(duplicateCustomerId, context);

                        //// If retrieved records are not null then delete all records in the list

                        if (customerContactMapListToBeDeleted != null && customerContactMapListToBeDeleted.Count() > 0)
                        {
                            foreach (CMDCustomerContactMap cmdCustomerContactMap in customerContactMapListToBeDeleted)
                            {
                                ////Delete  CMDCustomerContactMap record

                                var customerContactMapDelta = new Dictionary<string, object>();

                                customerContactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.IsActive), false);

                                customerContactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.UpdatedBy), dto.UpdatedBy);

                                customerContactMapDelta.Add(DAL.PropertyName<Models.CMDCustomerContactMap>(x => x.UpdatedDate), DateTime.UtcNow);

                                context.Update<Models.CMDCustomerContactMap>((int)cmdCustomerContactMap.Id, customerContactMapDelta, false);

                                var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                {
                                    CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                    Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                    TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerContactMap.ToString(),
                                    ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                    OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                    NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                    RecordID = cmdCustomerContactMap.Id,
                                    AuditLogDate = DateTime.UtcNow
                                };
                                context.Create(autoMergeAuditLog);
                            }
                        }

                        #endregion
                    }
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges Emails associated with duplicate customer into Emails of existing customer in CustomerDetails in case of Auto-Merge Scheduler        
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Emails merged successfully else false</returns>
        private bool MergeCustomerEmails(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerIDsForMerge != null && dto.CustomerIDsForMerge.Count > 0)
                {
                    ////Select emails of existing
                    List<string> existingCustomerEmailList = new List<string>();
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);
                    existingCustomerEmailList = GetEmailsListforCMDCustomerfromCMDCustomerEmail(existingCustomerId, context);

                    // assign Details of duplicate customer to existing
                    #region Check if CustomerIDsforSelectedDetailsList contains duplicateCustomerId -- Starts

                    if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                    {
                        List<string> newDetailsFromDuplicateCustomer = new List<string>();

                        foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                        {
                            var customerDuplicateMapRecord = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                            #region Assigning Details of Duplicate Customer to Existing Customer -- Starts

                            List<string> emailList = new List<string>();
                            emailList = GetEmailsListforCMDCustomerfromCMDCustomerEmail(duplicateCustomerId, context);

                            var existingCustomerEmail = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.Id == existingCustomerId)
                  .Select(cmdCustomer => cmdCustomer.Email).FirstOrDefault();

                            if (emailList != null && emailList.Count() > 0)
                            {
                                newDetailsFromDuplicateCustomer = emailList.Except(existingCustomerEmailList).ToList();
                                ////loop of duplicateDetails list
                                foreach (string email in newDetailsFromDuplicateCustomer)
                                {
                                    var customerEmailDetails = context.CMDCustomerEmails
                                          .Where(x => x.Email == email && x.CMDCustomerID == existingCustomerId && !x.IsActive)
                                                           .FirstOrDefault();

                                    if (customerEmailDetails != null)
                                    {
                                        #region If record present and deactivated: Update/activate CMDCustomerEmail record --Starts

                                        var customerEmailsDelta = new Dictionary<string, object>();

                                        customerEmailsDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.IsActive), true);
                                        customerEmailsDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.UpdatedBy), dto.UpdatedBy);

                                        customerEmailsDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.UpdatedDate), DateTime.UtcNow);

                                        context.Update<Models.CMDCustomerEmail>((int)customerEmailDetails.Id, customerEmailsDelta, false);

                                        #endregion If record present and deactivated: Update/activate CMDCustomerEmail record --Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                            NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                            OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerEmail.ToString(),
                                            RecordID = customerEmailDetails.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                    else
                                    {
                                        #region If record not present: Add new CMDCustomerEmail record -- Starts

                                        if (!existingCustomerEmailList.Contains(email) && !email.Equals(existingCustomerEmail))
                                        {
                                            var newCustomerEmailRecord = new CMDCustomerEmail
                                            {
                                                CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                                Email = email,

                                                IsActive = true,

                                                UpdatedBy = dto.UpdatedBy,

                                                UpdatedDate = DateTime.UtcNow,

                                                CreatedDate = DateTime.UtcNow
                                            };
                                            context.Create(newCustomerEmailRecord);

                                            #endregion If record not present: Add new CMDCustomerEmail record -- Ends

                                            var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                            {
                                                CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerEmail.ToString(),
                                                RecordID = newCustomerEmailRecord.Id,
                                                AuditLogDate = DateTime.UtcNow
                                            };
                                            context.Create(autoMergeAuditLog);
                                        }
                                    }
                                }
                            }
                            #endregion Assigning Details of Duplicate Customer to Existing Customer -- Ends

                            #region Add (Default)Email from Duplicate Customer to Existing Customers Details -- Starts

                            ////retrive email of Duplicate Customer
                            var duplicateCustomerEmail = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.Id == duplicateCustomerId)
                                .Select(cmdCustomer => cmdCustomer.Email).FirstOrDefault();

                            if (!string.IsNullOrEmpty(duplicateCustomerEmail))
                            {
                                /*Check if primary email of Duplicate customer is neither equal to primary email of existing customer nor present in 
                        any assigned emails in CMDCustomerEmails for existing customer*/

                                if (!duplicateCustomerEmail.Equals(existingCustomerEmail) && !existingCustomerEmailList.Contains(duplicateCustomerEmail))
                                {
                                    ////Add Email from Duplicate Customer to Existing Customers Details
                                    var newCustomerEmailfromDuplicateCustomer = new CMDCustomerEmail
                                    {
                                        CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                        Email = duplicateCustomerEmail,

                                        IsActive = true,

                                        UpdatedBy = dto.UpdatedBy,

                                        UpdatedDate = DateTime.UtcNow,

                                        CreatedDate = DateTime.UtcNow
                                    };
                                    context.Create(newCustomerEmailfromDuplicateCustomer);
                                    #endregion Add Email from Duplicate Customer to Existing Customers Details-- Ends

                                    var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerEmail.ToString(),
                                        RecordID = newCustomerEmailfromDuplicateCustomer.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog1);
                                }
                            }
                            #region Delete CMDCustomerEmail for Duplicate Customer -- Starts

                            // After merge, duplicate customer will get deleted hence we need to delete its assigned Details from CMDCustomerEmail table
                            // For duplicate customer get all the active CMDCustomerEmail records, which needs to be deleted

                            ////get list of CMDCustomerEmail Records for duplicate customer
                            List<CMDCustomerEmail> dupCustEmailRecords = GetCMDCustomerEmailsRecord(duplicateCustomerId, context);
                            //// If retrieved records are not null then delete all records in the list

                            if (emailList != null && emailList.Count() > 0)
                            {
                                foreach (CMDCustomerEmail cmdCustomerEmail in dupCustEmailRecords)
                                {
                                    ////Delete  CMDCustomerEmail record

                                    var customerEmailDelta = new Dictionary<string, object>();

                                    customerEmailDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.IsActive), false);

                                    customerEmailDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.UpdatedBy), dto.UpdatedBy);

                                    customerEmailDelta.Add(DAL.PropertyName<Models.CMDCustomerEmail>(x => x.UpdatedDate), DateTime.UtcNow);

                                    context.Update<Models.CMDCustomerEmail>((int)cmdCustomerEmail.Id, customerEmailDelta, false);

                                    var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMapRecord.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerEmail.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                        OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                        NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                        RecordID = cmdCustomerEmail.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion Check if CustomerIDsforSelectedDetailsList contains duplicateCustomerId -- Ends
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges Emails associated with duplicate customer into Emails 
        /// of existing customer in CustomerDetails in case of Auto-Merge Scheduler     
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if Emails merged successfully else false</returns>
        private bool MergeCustomerPhone(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerIDsForMerge != null && dto.CustomerIDsForMerge.Count > 0)
                {
                    ////Select emails of existing
                    List<string> existingCustomerPhoneList = new List<string>();
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);
                    existingCustomerPhoneList = GetPhonesListforCMDCustomerfromCMDCustomerPhone(existingCustomerId, context);

                    // assign Details of duplicate customer to existing
                    #region Check if CustomerIDsforSelectedDetailsList contains duplicateCustomerId -- Starts

                    if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                    {
                        List<string> newDetailsFromDuplicateCustomer = new List<string>();

                        foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                        {
                            var customerDuplicateMap = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                            #region Assigning Details of Duplicate Customer to Existing Customer -- Starts
                            ////Select specilaities of duplicate
                            List<string> phoneList = new List<string>();
                            phoneList = GetPhonesListforCMDCustomerfromCMDCustomerPhone(duplicateCustomerId, context);

                            var existingCustomerPhone = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.Id == existingCustomerId)
                          .Select(cmdCustomer => cmdCustomer.Phone).FirstOrDefault();
                            if (phoneList != null && phoneList.Count() > 0)
                            {
                                newDetailsFromDuplicateCustomer = phoneList.Except(existingCustomerPhoneList).ToList();
                                ////loop of duplicateDetails list
                                foreach (string phone in newDetailsFromDuplicateCustomer)
                                {
                                    var customerPhoneDetails = context.CMDCustomerPhones
                                          .Where(x => x.Phone == phone && x.CMDCustomerID == existingCustomerId && !x.IsActive)
                                                           .FirstOrDefault();

                                    if (customerPhoneDetails != null)
                                    {
                                        #region If record present and deactivated: Update/activate CMDCustomerPhone record --Starts

                                        var customerPhoneDelta = new Dictionary<string, object>();

                                        customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.IsActive), true);
                                        customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.UpdatedBy), dto.UpdatedBy);

                                        customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.UpdatedDate), DateTime.UtcNow);

                                        context.Update<Models.CMDCustomerPhone>((int)customerPhoneDetails.Id, customerPhoneDelta, false);

                                        #endregion If record present and deactivated: Update/activate CMDCustomerPhone record --Ends

                                        var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                        {
                                            CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                            Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                            NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                            OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                            ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                            TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                            RecordID = customerPhoneDetails.Id,
                                            AuditLogDate = DateTime.UtcNow
                                        };
                                        context.Create(autoMergeAuditLog);
                                    }
                                    else
                                    {
                                        #region If record not present: Add new CMDCustomerPhone record -- Starts

                                        if (!existingCustomerPhoneList.Contains(phone) && !phone.Equals(existingCustomerPhone))
                                        {
                                            var newCustomerPhoneRecord = new CMDCustomerPhone
                                            {
                                                CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                                Phone = phone,

                                                IsActive = true,

                                                UpdatedBy = dto.UpdatedBy,

                                                UpdatedDate = DateTime.UtcNow,

                                                CreatedDate = DateTime.UtcNow
                                            };
                                            context.Create(newCustomerPhoneRecord);

                                            #endregion If record not present: Add new CMDCustomerPhone record -- Ends

                                            var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                            {
                                                CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                                RecordID = newCustomerPhoneRecord.Id,
                                                AuditLogDate = DateTime.UtcNow
                                            };
                                            context.Create(autoMergeAuditLog);
                                        }
                                    }
                                }
                            }
                            #endregion Assigning Details of Duplicate Customer to Existing Customer -- Ends

                            #region Add (Default)Phone from Duplicate Customer to Existing Customers Details -- Starts

                            ////retrive email of Duplicate Customer
                            var duplicateCustomerPhone = context.CMDCustomers.Where(cmdCustomer => cmdCustomer.Id == duplicateCustomerId)
                                .Select(cmdCustomer => cmdCustomer.Phone).FirstOrDefault();

                            if (!string.IsNullOrEmpty(duplicateCustomerPhone))
                            {
                                /*Check if primary phone of Duplicate customer is neither equal to primary phone of existing customer nor present in 
                           any assigned phones in CMDCustomerPhones for existing customer*/

                                if (!duplicateCustomerPhone.Equals(existingCustomerPhone) && !existingCustomerPhoneList.Contains(duplicateCustomerPhone))
                                {
                                    ////Add Email from Duplicate Customer to Existing Customers Details
                                    var newCustomerPhonefromDuplicateCustomer = new CMDCustomerPhone
                                    {
                                        CMDCustomerID = Convert.ToInt32(existingCustomerId),

                                        Phone = duplicateCustomerPhone,

                                        IsActive = true,

                                        UpdatedBy = dto.UpdatedBy,

                                        UpdatedDate = DateTime.UtcNow,

                                        CreatedDate = DateTime.UtcNow
                                    };
                                    context.Create(newCustomerPhonefromDuplicateCustomer);
                                    #endregion Add Email from Duplicate Customer to Existing Customers Details-- Ends

                                    var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.NewRecord.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                        RecordID = newCustomerPhonefromDuplicateCustomer.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog1);
                                }
                            }

                            #region Delete CustomerPhone for Duplicate Customer -- Starts

                            // After merge, duplicate customer will get deleted hence we need to delete its assigned Details from CMDCustomerPhone table
                            // For duplicate customer get all the active CMDCustomerPhone records, which needs to be deleted

                            ////get list of CMDCustomerPhone Records for duplicate customer
                            List<CMDCustomerPhone> dupCustPhoneRecords = GetCMDCustomerPhoneRecord(duplicateCustomerId, context);
                            //// If retrieved records are not null then delete all records in the list

                            if (phoneList != null && phoneList.Count() > 0)
                            {
                                foreach (CMDCustomerPhone cmdCustomerPhone in dupCustPhoneRecords)
                                {
                                    ////Delete  CMDCustomerPhone record

                                    var customerPhoneDelta = new Dictionary<string, object>();

                                    customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.IsActive), false);

                                    customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.UpdatedBy), dto.UpdatedBy);

                                    customerPhoneDelta.Add(DAL.PropertyName<Models.CMDCustomerPhone>(x => x.UpdatedDate), DateTime.UtcNow);

                                    context.Update<Models.CMDCustomerPhone>((int)cmdCustomerPhone.Id, customerPhoneDelta, false);

                                    var autoMergeAuditLog = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.isActive.ToString(),
                                        OldValue = ALPHAEON.CMD.Common.Enums.BooleanValue.True.ToString(),
                                        NewValue = ALPHAEON.CMD.Common.Enums.BooleanValue.False.ToString(),
                                        RecordID = cmdCustomerPhone.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion Check if CustomerIDsforSelectedDetailsList contains duplicateCustomerId -- Ends
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }

        /// <summary>
        /// This method merges the ShoutScore associated with Duplicate customer with Existing customer by updating 
        /// the customerID associated with CMDShoutScoreEntry with Existing customer ID
        /// </summary>
        /// <param name="dto">Transient message object for customer merge</param>
        /// <param name="context">CMD DB Context</param>
        /// <returns>True if ShoutScore merged successfully else false</returns>
        private bool MergeCustomerShoutScoreAndShoutHistory(CustomerForAutoMerge dto, CMDDatabaseContext context)
        {
            bool isOperationSuccessful = false;

            try
            {
                if (dto.CustomerDuplicateIDsList != null && dto.CustomerDuplicateIDsList.Count() > 0)
                {
                    int existingCustomerId = Convert.ToInt32(dto.Customer.Id);

                    foreach (int duplicateCustomerId in dto.CustomerDuplicateIDsList)
                    {
                        var customerDuplicateMap = GetCmdCustomerDuplicateMapRecord(duplicateCustomerId, existingCustomerId, context);

                        #region Update CustomerShoutScore record

                        // Update Duplicate Customers ShoutScore record with CustomerID of Master Customer

                        ////Get Customer ShoutScore record for duplicate customer record

                        CMDShoutScore shoutScoreOfDuplicateCustomer = context.CMDShoutScores
                            .Where(cmdCustomerShoutScoreRecord =>
                                           cmdCustomerShoutScoreRecord.CMDCustomerID == duplicateCustomerId
                                && cmdCustomerShoutScoreRecord.IsActive
                                && cmdCustomerShoutScoreRecord.CMDCustomer.IsActive).FirstOrDefault();

                        //// Update CustomerShoutScore record

                        if (shoutScoreOfDuplicateCustomer != null)
                        {
                            var customerShoutScoreDelta = new Dictionary<string, object>();

                            customerShoutScoreDelta.Add(DAL.PropertyName<Models.CMDShoutScore>(x => x.CMDCustomerID), dto.Customer.Id);

                            customerShoutScoreDelta.Add(DAL.PropertyName<Models.CMDShoutScore>(x => x.UpdatedBy), dto.UpdatedBy);

                            customerShoutScoreDelta.Add(DAL.PropertyName<Models.CMDShoutScore>(x => x.UpdatedDate), DateTime.UtcNow);

                            context.Update<Models.CMDShoutScore>((int)shoutScoreOfDuplicateCustomer.Id, customerShoutScoreDelta, false);

                            ////Add entry in Merge Log table

                            var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                            {
                                CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.CMDCustomerID.ToString(),
                                OldValue = duplicateCustomerId.ToString(),
                                NewValue = dto.Customer.Id.ToString(),
                                TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                RecordID = shoutScoreOfDuplicateCustomer.Id,
                                AuditLogDate = DateTime.UtcNow
                            };
                            context.Create(autoMergeAuditLog1);
                        }
                        #endregion Update CustomerShoutScore record --Ends

                        #region Update Customer ShoutScore History records
                        // Update Duplicate Customers ShoutScore History records with CustomerID of Master Customer

                        ////Get list of Customers CMDShoutScoreHistory records for duplicate customer record

                        if (shoutScoreOfDuplicateCustomer != null)
                        {
                            ////Get list of CMDShoutScoreHistory for duplicate Customer

                            List<CMDShoutScoreHistory> shoutScoreHistoryListOfDuplicateCustomer =
                                context.CMDShoutScoreHistorys.Where(cmdShoutScoreHistory => cmdShoutScoreHistory.CMDCustomerID == duplicateCustomerId).ToList();

                            if (shoutScoreHistoryListOfDuplicateCustomer != null && shoutScoreHistoryListOfDuplicateCustomer.Count() > 0)
                            {
                                foreach (CMDShoutScoreHistory cmdShoutScoreHistory in shoutScoreHistoryListOfDuplicateCustomer)
                                {
                                    //// Update  Customer's CMDShoutScoreHistory records

                                    var customerShoutScoreHistoryDelta = new Dictionary<string, object>();

                                    customerShoutScoreHistoryDelta.Add(DAL.PropertyName<Models.CMDShoutScoreHistory>(x => x.CMDCustomerID), dto.Customer.Id);

                                    customerShoutScoreHistoryDelta.Add(DAL.PropertyName<Models.CMDShoutScoreHistory>(x => x.UpdatedBy), dto.UpdatedBy);

                                    customerShoutScoreHistoryDelta.Add(DAL.PropertyName<Models.CMDShoutScoreHistory>(x => x.UpdatedDate), DateTime.UtcNow);

                                    context.Update<Models.CMDShoutScoreHistory>((int)cmdShoutScoreHistory.Id, customerShoutScoreHistoryDelta, false);

                                    ////Add entry in Merge Log table

                                    var autoMergeAuditLog1 = new CMDCustomerMergeAuditLog
                                    {
                                        CMDCustomerDuplicateMapRecordId = customerDuplicateMap.Id,
                                        Action = ALPHAEON.CMD.Common.Enums.ActionForTableColumnChange.Updated.ToString(),
                                        ColumnName = ALPHAEON.CMD.Common.Enums.ColumnName.CMDCustomerID.ToString(),
                                        OldValue = duplicateCustomerId.ToString(),
                                        NewValue = dto.Customer.Id.ToString(),
                                        TableName = ALPHAEON.CMD.Common.Enums.TableName.CMDCustomerPhone.ToString(),
                                        RecordID = shoutScoreOfDuplicateCustomer.Id,
                                        AuditLogDate = DateTime.UtcNow
                                    };
                                    context.Create(autoMergeAuditLog1);
                                }
                            }
                        }

                        #endregion Update CustomerShoutScore History record --Ends
                    }
                }

                isOperationSuccessful = true;
            }
            catch (Exception)
            {
                throw;
            }

            return isOperationSuccessful;
        }
        #endregion
    }
}