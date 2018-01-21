using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Common;
using Alphaeon.Services.EnterpriseAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ALPHAEON.CMD.Common.Enums;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudRules.Customers
{
    sealed class CustomerContactUpdateRule : UpdateRule<CMDContact>
    {
        public CustomerContactUpdateRule(CMDContact model, IDictionary<string, object> oldValues, CMDDatabaseContext context)
            : base(model, oldValues, context)
        {
        }

        public override void Execute()
        {
            //get all resources (Product lines for this customer)   
            List<CMDCustomerSourceTrack> cmdCustomerSourceTrackList = GetCMDCustomerSourceTrackList();
            PushAlphaeonId.PushMessageToClients(cmdCustomerSourceTrackList, Operations.Updated, ALPHAEON.CMD.Common.Enums.Objects.CustomerContact);
        }

        
        private List<CMDCustomerSourceTrack> GetCMDCustomerSourceTrackList()
        {
            //get customers linked with contact
         var cmdCustomerIdList=   Context.CMDCustomerContactMaps.Where(c => c.CMDContactID == Model.Id).Select(m=>m.CMDCustomerID).ToList();

            return Context.CMDCustomerSourceTracks.Where(c => cmdCustomerIdList.Contains(c.CMDCustomerID)).ToList() ;
        }
    }
}