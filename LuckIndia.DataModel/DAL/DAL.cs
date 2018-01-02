using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.LoggingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL
{
    sealed class DAL
    {
        public static string PropertyName<T>(Expression<Func<T, Object>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (null != memberExpression)
            {
                return memberExpression.Member.Name;
            }

            var unaryExpression = expression.Body as UnaryExpression;
            if (null == unaryExpression)
            {
                throw new InvalidDataException();
            }

            var tempUnary = unaryExpression.Operand as MemberExpression;
            if (null != tempUnary)
            {
                return tempUnary.Member.Name;
            }

            throw new InvalidDataException();
        }

        public static void LogException(Exception ex, LogCategory subCategory)
        {
            var dalException = ex as DataAccessLayerException;

            if (dalException != null)
            {
                EventLogEntryType eventType;

                switch (dalException.ExceptionSeverity)
                {
                    case ExceptionSeverity.Information:
                        eventType = EventLogEntryType.Information;
                        break;
                    case ExceptionSeverity.Error:
                        eventType = EventLogEntryType.Error;
                        break;
                    default:
                        eventType = EventLogEntryType.Warning;
                        break;
                }

                CMDApiLogger.LogException(ex, EventCategory.DataAccessLayer, eventType, (short)subCategory);
            }
            else
            {
                CMDApiLogger.LogException(ex, EventCategory.DataAccessLayer, EventLogEntryType.Error, (short)subCategory);
            }
        }

        public static void Init()
        {
            //Load all the caching services
            ApplicationCrudHooksCachingService.GetCurrent();
            ApplicationPermissionCachingService.GetCurrent();
            BusinessRulesCachingService.GetCurrent();
            CollectionFilterCachingService.GetCurrent();
            CrudHooksCachingService.GetCurrent();
            CrudRulesCachingService.GetCurrent();
            DataRulesCachingService.GetCurrent();
            ModelClassCachingService.GetCurrent();
            RolePermissionCachingService.GetCurrent();

            //run the old config...soon to be obsolete
            Config.Init();
        }
    }
}
