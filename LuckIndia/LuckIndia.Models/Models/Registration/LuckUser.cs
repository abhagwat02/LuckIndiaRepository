using LuckIndia.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    [Include]
    public class LuckUser : Model
    {
        public LuckUser()
        {
            accounts = new HashSet<Account>();
        }
        public override int Id { get; set; }
        public String FirstName { get; set; }
        public String MiddleName  { get; set; }
        public String LastName { get; set; }
        public long PhoeNumber { get; set; }
        public String Address { get; set; }
        public int ParentAccountID { get; set; }
        [Include]
        public ICollection<Account> accounts { get; set; }

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

    }
}