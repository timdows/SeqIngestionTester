using Serilog;
using Serilog.Core;
using System;
using System.Threading;

namespace SeqIngestionTester
{
	class Program
    {
        static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            AppDomain.CurrentDomain.UnhandledException += AppUnhandledException;

            using (var logger = BuildSerilog())
            {
                try
                {
                    logger.Information("Hello world");

                    var demo = new Thread(() => {
                        throw new Exception("It's a feature, I promise!");
                    });
                    //demo.Start();

                    //Task.Delay(10000).Wait();

                    logger.Error("Hello error");
                }
                catch (Exception e)
                {
                    UnhandledExceptions(e);
                }
            }
        }

        private static void AppUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Log.Logger != null && e.ExceptionObject is Exception exception)
            {
                UnhandledExceptions(exception);

                // It's not necessary to flush if the application isn't terminating.
                if (e.IsTerminating)
                {
                    Log.CloseAndFlush();
                }
            }
        }

        private static void UnhandledExceptions(Exception e)
        {
            Log.Logger?.Error(e, "Console application crashed");
        }

        private static Logger BuildSerilog()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Seq("https://log.server:5341", apiKey: "<123>")
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }
    }
}
