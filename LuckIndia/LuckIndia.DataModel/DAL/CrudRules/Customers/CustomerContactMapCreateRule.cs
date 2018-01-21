using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Common;
using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common.AlphaeonId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ALPHAEON.CMD.Common.Enums;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudRules.Customers
{
    sealed class CustomerContactMapCreateRule : CreateRule<CMDCustomerContactMap>
    {
        /// <summary>
        /// we always create sourcetrack for customer creation and
        /// in some cases we only create cmdCustomerContactMap so we are using CMDCustomerContactMap instead of cmdcontact.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        public CustomerContactMapCreateRule(CMDCustomerContactMap model, CMDDatabaseContext context)
            : base(model, context)
        {
        }
        public override void Execute()
        {
            List<CMDCustomerSourceTrack> cmdCustomerSourceTrackList = GetCMDCustomerSourceTrackList();
            PushAlphaeonId.PushMessageToClients(cmdCustomerSourceTrackList, Operations.Created, ALPHAEON.CMD.Common.Enums.Objects.CustomerContact);

        }

        private List<CMDCustomerSourceTrack> GetCMDCustomerSourceTrackList()
        {
            return Context.CMDCustomerSourceTracks.Where(c => c.CMDCustomerID == Model.CMDCustomerID).ToList();
        }
    }
}