
using LuckIndia.DataModel.Authorizations.Authorizations;
using LuckIndia.DataModel.Enums;
using LuckIndia.DataModel.Interfaces;
using System;
using System.Configuration;

namespace LuckIndia.DataModel.Authorizations

{
    public static class AuthProviderFactory
    {
      
     
        public static IAuthProvider GetAuthProvider()
        {
            AuthEnums.ProviderType providerType = (AuthEnums.ProviderType)Convert.ToInt32(ConfigurationManager.AppSettings["AuthProviderType"]);
            IAuthProvider retVal = null;
            switch (providerType)
            {
                case AuthEnums.ProviderType.LuckIndia:
                    retVal = new LuckIndiaAuthProvider();
                    break;

            }
            return retVal;
        }
       
        
    }
}