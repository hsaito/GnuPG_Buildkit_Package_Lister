using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;

namespace GnuPG_Buildkit_Package_Lister
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        ///     Main Function
        /// </summary>
        private static int Main()
        {
            try
            {
                // Configuration for logging
                var log4NetConfig = new XmlDocument();

                using (var reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4NetConfig.Load(reader);
                }

                var rep = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                    typeof(Hierarchy));
                XmlConfigurator.Configure(rep, log4NetConfig["log4net"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.Messages.LoggingError);
                Console.WriteLine(ex.Message);
                return -1;
            }

            try
            {
                // Start the program
                Log.Info(Resources.Messages.ProgramStarting);
#if DEBUG
                Log.Info(Resources.Messages.DebugMode);
#endif
                var retval = GnuPrivacyGuardProcessor.Process().Result;
                //Process().Wait();
                Log.Info(Resources.Messages.ProgramCompleted);
                return retval;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                Log.Debug(ex.StackTrace);
                return -1;
            }
        }
    }
}