﻿namespace LuckIndia.DataModel.DAL.Enums
{
    enum RemoteProcedureCallValue
    {
        RPC_Authenticate = 1,
        RPC_Authorize = 2,
        RPC_Logout = 3,
        RPC_Me = 4,
        RPC_AuthorizeSurrogate = 5,
        RPC_AccountRoleAdd = 6,
        RPC_AccountRoleRemove = 7,
        RPC_DistributorRoleAdd = 8,
        RPC_DistributorRoleRemove = 9,
        RPC_EmailMessage = 10,
        RPC_ErrorEmail = 11,
        RPC_NewAccountEmail = 13,
        RPC_PresentationRoleAdd = 14,
        RPC_PresentationRoleRemove = 15,
        RPC_ProfileImage = 16,
        RPC_RobotSurrogate = 17,
        RPC_SetContentMenuModeTopic = 18,
        Notify_PurchaseOrderMilestones = 19,
        Reports_AccountCreation = 20,
        Reports_EmrUsage = 21,
        Reports_LoginsOfCustomer = 22,
        Reports_PurchaseOrderFulfillment = 23,
        RPC_Ping = 24,
        RPC_CreateUserPassword = 25,
        Reports_LoginEventSummary = 26,
        RPC_CustomerRoleAdd = 27,
        RPC_CustomerRoleRemove = 28,
        RPC_ApplicationVersion = 29,
        RPC_ImageProcessorCallBack = 30,
        RPC_AuthenticateTouchCard = 31,
        RPC_PasswordResetStart = 32,
        RPC_PasswordResetComplete = 33,
        RPC_GetPresentationRobotUserToken = 34,
        RPC_GetUserFromIntegrationMessageInfo = 35,



        #region CMD And API

        //CMD
        RPC_CMDExceptionLogAdd = 36,
        RPC_CMDCustomerAdd = 37,
        RPC_CMDPracticeContactAdd = 38,
        RPC_CMDAccountAdd = 39,
        RPC_CMDDistributorAdd = 40,
        rpc_CMDTransactionAdd = 41,
        RPC_CMDTransactionalObjectUpdate = 43,
        #endregion

        #region Dashboard
        //Get method enum
        RPC_GetCMDCustomer = 42,
        RPC_GetCMDPractice = 43,
        RPC_GetCMDTransaction = 44,
        RPC_GetCMDPhysicianListByPracticeId = 45,
        RPC_GetCMDProductListByPracticeId = 46,
        RPC_GetCMDBusinessUnit = 47,
        RPC_GetCMDPracticeListByCustomerId = 48,
        RPC_GetCMDProductListByCustomerId = 49,
        RPC_GetCMDTransactionListByCustomerId = 50,
        RPC_GetCMDBusinessUnitListByCustomerId = 51,
        RPC_GetCMDTransactionListByPracticeId = 52,
        RPC_GetCMDBusinessUnitListByPracticeId = 53,
        RPC_GetCMDProductListByBusinessUnitId = 54,
        RPC_GetCMDTransactionListByBusinessUnitId = 55,
        RPC_GetCMDPracticeListByStateId = 56,
        RPC_GetDuplicateCMDCustomerListByCustomerId = 57,
        RPC_GetExistingCMDCustomerListforDuplicates = 58,
        RPC_GetDuplicateCMDPracticeListByPracticeId = 59,
        RPC_GetExistingCMDPracticeListforDuplicates = 60,
        RPC_GetCMDTransactionDetailsByTransactionId = 61,
        RPC_GetExistingCMDCustomerListByDuplicateCustomerId = 62,
        RPC_GetExistingCMDPracticeListByDuplicatePracticeId = 63,
        RPC_CMDDashboardCustomers = 64,
        RPC_CMDDashboardCustomerPracticeMaps = 65,
        RPC_CMDDashboardResolveDuplicateCustomers = 66,
        RPC_CMDDashboardPractices = 67,
        RPC_CMDDashboardResolveDuplicatePractices = 68,
        RPC_CMDDashboardProducts = 69,
        RPC_CMDDashboardBusinessUnits = 70,
        RPC_CMDDashboardDistributors = 71,
        RPC_CMDDashboardSpecialities = 72,
        RPC_CMDDashboardCountries = 73,
        RPC_CMDDashboardStates = 74,
        RPC_CMDDashboardCustomerNotes = 75,
        RPC_CMDDashboardPracticeNotes = 76,
        RPC_CMDDashboardBusinessUnitNotes = 77,
        RPC_CMDDashboardContacts = 78,
        RPC_GetCMDDashboardActiveCustomersReport = 79,
        RPC_GetCMDProductByProductId = 80,
        RPC_CMDTransactionContactAdd = 81,
        RPC_CMDTransactionItems = 82,
        RPC_GetCMDProductListByCMDCustomerId = 83,
        RPC_GetCMDProductListByCMDPracticeId = 84,
        RPC_CMDProductAdd = 85,
        #endregion

        RPC_EntityExistCheck = 86,
        RPC_GetCMDContactListByEntityId = 87,
        RPC_GetCMDSourceRecordListByEntityId = 88,
        RPC_CMDGlobalSearch = 89,
        RPC_CMDCustomerSpeciality = 90,

        #region ShoutScore
        RPC_CMDShoutScoreAdd = 91,
        #endregion

        RPC_CMDCustomersExcludingCustomersOfPassedBusinessUnitId = 92,
        RPC_CMDCustomerAutoMerge = 93,
        RPC_GetCMDSourceRecordListByCustomerId = 94,
        RPC_GetCMDSourceRecordListByPracticeId = 95,
                
        RPC_GetCMDCustomerListForListAndTileView = 96,
        RPC_GetCMDCustomerListForDuplicateListView = 97,
        RPC_GetCMDCustomerListForDetailsView = 98,
        RPC_GetCMDCustomerListForDuplicateMergeView = 99,
        RPC_GetCMDSpecialityListByCustomerId = 100,
        RPC_GetCMDPracticeDetails = 101,
        RPC_GetCMDPracticeList = 102,
        RPC_GetCMDPracticeDuplicateList = 103,
        RPC_GetCMDPracticeDuplicateMerge = 104
    }
}
