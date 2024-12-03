using System.IO;

namespace FleetMgt
{
    public class Logger
    {
        private static readonly string LogFilePath = "logs/error_log.txt";
        static Logger()
        {
            Directory.CreateDirectory("logs"); // Ensure the directory exists
        }

        public static void LogError(string message, Exception ex = null)
        {
            // Construct the log message
            string logMessage = $"{DateTime.Now}: {message}";
            if (ex != null)
            {
                logMessage += $" Exception: {ex.Message}\nStack Trace: {ex.StackTrace}";
            }
            logMessage += Environment.NewLine;

            // Write to console
            Console.WriteLine(logMessage);

            // Write to log file
            File.AppendAllText(LogFilePath, logMessage);
        }
    }
}
