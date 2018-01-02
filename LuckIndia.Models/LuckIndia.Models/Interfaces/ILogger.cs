namespace LuckIndia.Models.Interfaces
{
    /// <summary>
    /// Interface: Logger to append log and get log message.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// method to appen string message
        /// </summary>
        /// <param name="line"></param>
        void AppendLine(string line);

        /// <summary>
        /// method Get log message
        /// </summary>
        /// <returns></returns>
        string GetLog();
    }
}
