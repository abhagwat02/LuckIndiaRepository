namespace LuckIndia.DataModel.DAL.Enums
{
    //Make sure none of these values go over a short (32767)
    enum LogCategory
    {
        None = 0,

        LegacyRule = 100,
        LegacyRuleCreate = 101,
        LegacyRuleRead = 102,
        LegacyRuleUpdate = 103,
        LegacyRuleDelete = 104,

        CrudRule = 200,
        CrudRuleCreate = 201,
        CrudRuleRead = 202,
        CrudRuleUpdate = 203,
        CrudRuleDelete = 204,

        CrudHook = 300,
        CrudHookCreate = 301,
        CrudHookRead = 302,
        CrudHookUpdate = 303,
        CrudHookDelete = 304,

        ModelEvent = 400,
        ModelEventCreate = 401,
        ModelEventRead = 402,
        ModelEventUpdate = 403,
        ModelEventDelete = 404,

        Runnable = 500,
        ApplicationValidation = 501,
        UserValidation = 502,
        CrudOperation = 503,

        DataRule = 600,
        DataRuleCreate = 601,
        DataRuleRead = 602,
        DataRuleUpdate = 603,
        DataRuleDelete = 604,

        BusinessRule = 700,
        BusinessRuleCreate = 701,
        BusinessRuleRead = 702,
        BusinessRuleUpdate = 703,
        BusinessRuleDelete = 704,
    }
}
