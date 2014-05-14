using System;
using Serilog;
using Serilog.Sinks.EventLog;

namespace AppLogging
{
    public class Mlogger : IMlogger
    {
        private LoggerConfiguration LoggerConfig { get; set; }
        public ILogger logger { get; set; }

        public Mlogger()
        {
            var log = new LoggerConfiguration()
#if DEBUG
                .WriteTo.ColoredConsole()
#else
                .WriteTo.EventLog("AzureApiReporter")
#endif
                .CreateLogger();
            logger = log;
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }
        public void Debug(Exception e, string message)
        {
            logger.Debug(e, message);
        }

        public void Info(string message)
        {
            logger.Information(message);
        }
        public void Info(Exception e, string message)
        {
            logger.Information(e, message);
        }

        public void Warn(string message)
        {
            logger.Warning(message);
        }
        public void Warn(Exception e, string message)
        {
            logger.Warning(e, message);
        }
    }

    public interface IMlogger
    {
        void Debug(string message);
        void Debug(Exception e, string message);

        void Info(string message);
        void Info(Exception e, string message);

        void Warn(string message);
        void Warn(Exception e, string message);
    }
}
