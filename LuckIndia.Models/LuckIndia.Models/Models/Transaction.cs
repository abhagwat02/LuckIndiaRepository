using LuckIndia.Models.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuckIndia.Models
{
    public class Transaction : Model
    {

        public override int Id { get; set; }
        public int Amount { get; set; }
        public DateTime TxnDate { get; set; }

        public Bids Bid { get; set; }
        public Account Player { get; set; }
        public TxnType Type { get; set; }
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