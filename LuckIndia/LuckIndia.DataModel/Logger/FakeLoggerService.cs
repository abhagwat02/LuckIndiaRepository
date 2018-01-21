using LuckIndia.DataModel.Interfaces;
using System;
using System.Diagnostics;

namespace LuckIndia.DataModel.LoggingServices
{
    /// <summary>
    /// An empty logger that doesn't do anything.
    /// </summary>
    sealed public class FakeLoggerService : ILogger
    {
        public void LogException(Exception exception,  EventLogEntryType entryType, short subcategoryId = 0)
        {
            //fake logging doesn't do anything
        }

        public void LogInformation(string description, short subcategoryId = 0)
        {
            //fake logging doesn't do anything
        }

        public void LogWarning(string description,  short subcategoryId = 0)
        {
            //fake logging doesn't do anything
        }

        public void AppendLine(string line)
        {
            //fake logging doesn't do anything
        }

        public string GetLog()
        {
            return string.Empty;
        }
    }
}
