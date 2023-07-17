using System.Diagnostics;

namespace FoundryHost
{
    public static class LogManager
    {
        private static EventLog eventLog = new EventLog("Application", Environment.MachineName, ".NET Runtime");

        public static void Startup(string parameters)
        {
            string message = $"Foundry Host Service launched node.exe with the following parameters: {parameters}";
            eventLog.WriteEntry(message, EventLogEntryType.Information, 1000);
        }

        public static void Exited()
        {
            string message = $"Foundry Host Service has shut down, node.exe exited.";
            eventLog.WriteEntry(message, EventLogEntryType.Information, 1000);
        }

        public static void Cancelled()
        {
            string message = $"Foundry Host Service was stopped.";
            eventLog.WriteEntry(message, EventLogEntryType.Information, 1000);
        }

        public static void InvalidConfig()
        {
            string message = $"Invalid Message";
            eventLog.WriteEntry(message, EventLogEntryType.Error, 1000);
        }

        public static void Error(Exception ex)
        {
            string message = $"Foundry Host Service encountered an error:\r\n" +
                $"Message:\t{ex.Message}\r\n" +
                $"Source:\t{ex.Source}\r\n" +
                $"StackTrace:\t{ex.StackTrace}";
            eventLog.WriteEntry(message, EventLogEntryType.Error, 1000);
        }
    }
}
