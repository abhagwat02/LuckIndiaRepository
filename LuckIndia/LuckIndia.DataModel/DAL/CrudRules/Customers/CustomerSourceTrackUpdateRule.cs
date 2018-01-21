using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Common;
using Alphaeon.Services.EnterpriseAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ALPHAEON.CMD.Common.Enums;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudRules.Customers
{
    sealed class CustomerSourceTrackUpdateRule : UpdateRule<CMDCustomerSourceTrack>
    {
        public CustomerSourceTrackUpdateRule(CMDCustomerSourceTrack model, IDictionary<string, object> oldValues, CMDDatabaseContext context)
            : base(model, oldValues, context)
        {
        }

        public override void Execute()
        {
            //get all resources (Product lines for this customer)   
            List<CMDCustomerSourceTrack> cmdCustomerSourceTrackList = GetCMDCustomerSourceTrackList();
            PushAlphaeonId.PushMessageToClients(cmdCustomerSourceTrackList, Operations.Updated, ALPHAEON.CMD.Common.Enums.Objects.Customer);
        }

        private List<CMDCustomerSourceTrack> GetCMDCustomerSourceTrackList()
        {
            return Context.CMDCustomerSourceTracks.Where(c => c.CMDCustomerID == Model.CMDCustomerID).ToList();
        }
    }
}