using LuckIndia.Models.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuckIndia.Models
{
    [ManyToManyObject]
    [NonDeletable]
    public class Role : Model
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }

        [NonPatchable]
        public int RoleTypeId { get; set; }


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


        public string Description
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
            //    new StringValidationService(value)
            //    .ValidateMaxLength(1000);

                this.SetPropertyValue(value);
            }
        }

        [ManyToManyObject]
        public virtual RoleType RoleType { get; set; }



        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        //public virtual ICollection<ApplicationAccessRole> ApplicationAccessRoles { get; set; }


        //public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }

    }
}
