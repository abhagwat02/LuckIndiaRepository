namespace LuckIndia.Models.Interfaces
{
    /// <summary>
    /// Interface: model persmission related get/set information
    /// </summary>
    public interface IModelPermission
    {
        /// <summary>
        /// Property contain the class id for model
        /// </summary>
        int ModelClassId { get; set; }

        /// <summary>
        /// Property contain the boolean value for create
        /// </summary>
        bool CanCreate { get; set; }

        /// <summary>
        /// Property contain the boolean value for read
        /// </summary>
        bool CanRead { get; set; }

        /// <summary>
        /// Property contain the boolean value for update
        /// </summary>
        bool CanUpdate { get; set; }

        /// <summary>
        /// Property contain the boolean value for delete
        /// </summary>
        bool CanDelete { get; set; }
    }
}