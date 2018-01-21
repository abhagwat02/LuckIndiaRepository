using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LuckIndia.Models.Attributes;

namespace LuckIndia.Models
{
    [NonDeletable]
    public class RoleType : Model
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }

        
        public string Title
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
                //new StringValidationService(value)
                //.ValidateNotNullOrWhiteSpace()
                //.ValidateMaxLength(100);

                this.SetPropertyValue(value);
            }
        }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
