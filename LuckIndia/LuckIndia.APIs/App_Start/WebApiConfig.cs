using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using LuckIndia.APIs.Factories;
using LuckIndia.APIs.Handlers;

namespace LuckIndia.APIs
{
    public static class WebApiConfig
    {

        public const int PageSize = 1000;

        public const string HeaderNameAccessToken = "X-AccessToken";
        public const string HeaderNameMetadata = "X-Metadata";

        internal const string WIDTH_QUERY_PARAMETER = "w";
        internal const string HEIGHT_QUERY_PARAMETER = "h";
        internal const string CROP_QUERY_PARAMETER = "crop";
        internal const string ROTATION_QUERY_PARAMETER = "rotate";
        internal const string SIZE_QUERY_PARAMETER = "size";

        internal const string ROUTE_PARAM_CONTROLLER = "controller";

        internal const string AUTHORIZATION_HEADER_SCHEME = "Bearer";

        public static string[] QueryStringNamesAccessToken = { "ACCESSTOKEN", "AccessToken", "accessToken", "accesstoken" };
        public static string LoggerHttpContextItemsKey = "logger";
        public static string EnableLoggingQueryString = "enablelogging";
        public static string EnableLoggingInAPI = "enablelogging";
        public static bool IsModelEventLogsEnabled;
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            SetupCustomHandlers(config);
            //config.Routes.MapHttpRoute(
            //    name: "Users",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }

            //);

            //config.Routes.MapHttpRoute(
            //   name: "Account",
            //   routeTemplate: "api/Users/{userId}/accounts/{id}",
            //   defaults: new { controller = "accounts", id = RouteParameter.Optional }
            //   );

            //config.Routes.MapHttpRoute(
            //name: "AccountWithUsername",
            //routeTemplate: "api/accounts/{username}",
            //defaults: new { controller = "accounts"}
            //);
            FactoryFactory.Init();

            config.EnableQuerySupport();
            config.Routes.MapHttpRoute("REST", "{" + ROUTE_PARAM_CONTROLLER + "}/{id}", new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute("QUESTIONS", "Questions/{Qid}/{"+ROUTE_PARAM_CONTROLLER + "}/{id}", new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Users", "Users/{uid}/{" + ROUTE_PARAM_CONTROLLER + "}/{id}", new { id = RouteParameter.Optional });


            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
            = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        private static void SetupCustomHandlers(HttpConfiguration config)
        {
            //First thing in the pipeline, parse for an AccessToken and set the Pricipal so all later calls know what user it is.
            config.MessageHandlers.Add(new SetApiPrincipalHandler());

            ////allows the X-HTTP-Metod-Override to be used.
            //config.MessageHandlers.Add(new MethodOverrideHandler());

            ////allows odata isactive function to be used.
            //config.MessageHandlers.Add(new ODataIsActiveHandler());

            ////allows myuserid() to be used
            //config.MessageHandlers.Add(new ODataMyUserIdHandler());

            ////allows odata utcnow function to be used. May want to pass this last so other handlers can use utcnow()
            //config.MessageHandlers.Add(new ODataUtcNowHandler());
        }

        public static JsonSerializerSettings StandardJsonSerializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }
}
