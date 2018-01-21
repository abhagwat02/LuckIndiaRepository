using LuckIndia.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    [Include]
    public class Account : Model
    {
        public override int Id { get; set; }

        private DateTime _createdDate;
        [NonPatchable]
        [Column(TypeName = "datetime2")]
        public DateTime DateCreated
        {
            get { return DateTime.SpecifyKind(_createdDate, DateTimeKind.Utc); }
            set { _createdDate = DateTime.SpecifyKind(value, DateTimeKind.Utc); }

        }
        private DateTime _updatedDate;

        [NonPatchable]
        [Column(TypeName = "datetime2")]
        public DateTime DateModified
        {
            get { return DateTime.SpecifyKind(_updatedDate, DateTimeKind.Utc); }
            set { _updatedDate = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }
        public int CardNumber { get; set; }
        public int? AccountTypeID { get; set; }
        public int LuckUserID { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }

        [Include]
        public LuckUser user { get; set; }
        [Include]
        public AccountType Type { get; set; }
        public int ParentAccountID { get; set; }
    }
}