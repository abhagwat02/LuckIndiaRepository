using LuckIndia.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    [Include]
    public class Question:Model
    {

        public Question()
        {
            Options = new HashSet<Option>();
        }
        public override int Id { get; set; }

        public String Statement { get; set; }

        [Include]
        public ICollection<Option> Options { get; set; }
    
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