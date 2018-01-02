using Alphaeon.Services.EnterpriseAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ALPHAEON.CMD.Common.Enums;
using ALPHAEON.CMD.Common;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Common;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudRules.Customers
{
    sealed class CustomerCreateRule : CreateRule<CMDCustomerSourceTrack>
    {
        /// <summary>
        /// we always create sourcetrack for customer creation and
        /// in some cases we only create cmdCustomerSourceTrack so we are using CMDCustomerSourceTrack instead of cmdcustomer.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        public CustomerCreateRule(CMDCustomerSourceTrack model, CMDDatabaseContext context)
            : base(model, context)
        {
        }
        public override void Execute()
        {
            List<CMDCustomerSourceTrack> cmdCustomerSourceTrackList = new List<CMDCustomerSourceTrack> { Model };
            PushAlphaeonId.PushMessageToClients(cmdCustomerSourceTrackList, Operations.Created, ALPHAEON.CMD.Common.Enums.Objects.Customer);
        }

    }
}