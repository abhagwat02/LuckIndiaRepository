using LuckIndia.Models.Attributes;
using LuckIndia.Models.Interfaces;

namespace LuckIndia.Models
{
    [ManyToManyObject]
    public class RolePermission : Model, IModelPermission
    {
        public RolePermission()
        {
            this.CanCreate = false;
            this.CanRead = false;
            this.CanUpdate = false;
            this.CanDelete = false;
        }

        public override int Id { get; set; }

        [NonPatchable]
        public int RoleId { get; set; }

        [NonPatchable]
        public int ModelClassId { get; set; }

        public bool CanCreate { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }

        [ManyToManyObject]
        public virtual Role Role { get; set; }

        [ManyToManyObject]
        public virtual ModelClass ModelClass { get; set; }
    }
}
