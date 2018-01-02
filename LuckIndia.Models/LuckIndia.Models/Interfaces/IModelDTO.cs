namespace LuckIndia.Models.Interfaces
{

    public interface IModelDTO : IDTO
    {
        /// <summary>
        /// property contains id 
        /// </summary>
        int? Id { get; set; }
    }
}