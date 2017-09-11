using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace GnuPG_Buildkit_Package_Lister
{
    public static class GnuPrivacyGuardProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
       
        /// <summary>
        /// Actual Process
        /// </summary>
        public static async Task<int> Process()
        {
            try
            {
                // Load components list ("product" names)
                var components = GetComponents();


                // Open URL
                var url = GetElementByName("url");
                Log.Info(Resources.Messages.UrlFetch + url);

                // Get the contents
                Log.Info(Resources.Messages.GetRemote);
                var content = await GnuPgBuildkitPackageListerUtils.GetWeb(url);

                // Enumerate versions into the list
                var versions = new List<string>();

                Log.Info(Resources.Messages.ExtractVersion);
                // Extract versions from the HTML dump
                foreach (var item in components)
                {
                    (var ver, var name) = ExtractVersion(content, item);
                    Log.Info("Adding: " + name);
                    versions.Add(ver);
                }

                // Merge into packages file
                Log.Info(Resources.Messages.GenerateList);
                CreateResult(components, versions);
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Debug(ex.StackTrace);
                return -1;
            }
        }



        /// <summary>
        /// Merge and create a package list
        /// </summary>
        /// <param name="components">List of the components</param>
        /// <param name="versions">List of the versions</param>
        private static void CreateResult(IReadOnlyList<string> components, IReadOnlyList<string> versions)
        {
            // Get the template string
            var template = GetTemplate();

            // Merge the components and versions into the string
            for (var i = 0; i < components.Count; i++)
            {
                var replacement = new Regex(string.Format("<{0}>", components[i]));
                template = replacement.Replace(template, versions[i]);
            }

            // Write it out.
            using (var sw = new StreamWriter(new FileStream(GetOutputName(), FileMode.Create), new UTF8Encoding(false)))
            {
                sw.Write(template);
                sw.Flush();
            }
        }

        /// <summary>
        /// Get the version by the name of the component
        /// </summary>
        /// <param name="data">String of the data dump</param>
        /// <param name="component">Name of the component to retrieve</param>
        /// <returns>Version as the string</returns>
        private static (string, string) ExtractVersion(string data, string component)
        {
            // Kind of a hack, just match it up in the filename in the HTML
            var pattern = new Regex(component + @"-(?<version>.*?)" + ".tar.bz2");
            var match = pattern.Match(data);
            return (match.Groups["version"].Value, match.ToString());
        }

        /// <summary>
        /// Get the list of components
        /// </summary>
        /// <returns>List of the components</returns>
        private static List<string> GetComponents()
        {
            var components = new List<string>();
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                var element = XElement.Load(sr);
                var componentGroup = element.Element("components");

                // Add the component to the list
                if (componentGroup == null) return components;
                components.AddRange(componentGroup.Elements().Select(item => item.Value));
            }

            return components;
        }

        /// <summary>
        /// Get the components
        /// </summary>
        /// <returns>Template as the string</returns>
        private static string GetTemplate()
        {
            using (var sr = new StreamReader(new FileStream(GetTemplateName(), FileMode.Open)))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Get the name of the template
        /// </summary>
        /// <returns>Name of the template as the string</returns>
        private static string GetTemplateName()
        {
            return GetElementByName("template");
        }

        /// <summary>
        /// Get the name of the output
        /// </summary>
        /// <returns>Name of the output as the string</returns>
        private static string GetOutputName()
        {
            return GetElementByName("output");
        }

        /// <summary>
        /// Get XML element from config by name.
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <returns>Element value as string.</returns>
        private static string GetElementByName(string name)
        {
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                var element = XElement.Load(sr);
                return element.Element(name)?.Value;
            }
        }
    }
}