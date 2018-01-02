using System.Collections.Generic;

namespace LuckIndia.Models
{
    //[NonDeletable]
    public class ModelClass : Model
    {
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


        public string PluralTitle
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
                //new StringValidationService(value)
                //.ValidateNotNull()
                //.ValidateMaxLength(2000);

                this.SetPropertyValue(value);
            }
        }

        //public virtual ICollection<ApplicationPermission> ApplicationPermissions { get; set; }

        //public virtual ICollection<CollectionFilter> CollectionFilters { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        //public virtual ICollection<ModelEvent> ModelEvents { get; set; }

        //public virtual ICollection<CrudHook> CrudHooks { get; set; }

        //public virtual ICollection<ModelClassDocument> ModelClassDocuments { get; set; }
    }
}
