using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using log4net;
using System.Xml;
using log4net.Repository;
using System.Reflection;
using log4net.Config;

namespace GnuPG_Buildkit_Package_Lister
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Main Function
        /// </summary>
        static int Main(string[] args)
        {

            try
            {
                // Configuration for logging
                XmlDocument log4netConfig = new XmlDocument();

                using (StreamReader reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4netConfig.Load(reader);
                }

                ILoggerRepository rep = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(rep, log4netConfig["log4net"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.messages.logging_error);
                Console.WriteLine(ex.Message);
                return -1;
            }

            try
            {
                // Start the program
                log.Info(Resources.messages.program_starting);
#if DEBUG
                log.Info(Resources.messages.debug_mode);
#endif
                GnuPG_Processor processor = new GnuPG_Processor();
                var retval = processor.Process().Result;
                //Process().Wait();
                log.Info(Resources.messages.program_completed);
                return retval;
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                log.Debug(ex.StackTrace);
                return -1;
            }
        }
    }
}