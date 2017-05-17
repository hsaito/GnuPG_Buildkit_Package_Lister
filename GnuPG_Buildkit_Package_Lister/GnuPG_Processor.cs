using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace GnuPG_Buildkit_Package_Lister
{
    public class GnuPG_Processor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
       
        /// <summary>
        /// Actual Process
        /// </summary>
        public async Task Process()
        {
            try
            {
                var url = "";

                // Load components list ("product" names)
                var components = GetComponents();


                // Open URL
                url = GetElementByName("url");
                log.Info(Resources.messages.url_fetch + url);

                // Get the contents
                log.Info(Resources.messages.get_remote);
                var content = await GetWeb(url);

                // Enumerate versions into the list
                List<string> versions = new List<string>();

                log.Info(Resources.messages.extract_version);
                // Extract versions from the HTML dump
                foreach (var item in components)
                {
                    (var ver, var name) = ExtractVersion(content, item);
                    log.Info("Adding: " + name);
                    versions.Add(ver);
                }

                // Merge into packages file
                log.Info(Resources.messages.generate_list);
                CreateResult(components, versions);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Debug(ex.StackTrace);
            }
        }

        /// <summary>
        /// Retrieve content from the web.
        /// </summary>
        /// <param name="location">URL of the website</param>
        /// <returns>String of the website</returns>
        public async Task<string> GetWeb(string location)
        {
            // Make a request
            var request = WebRequest.Create(location);
            // Get the response
            var response = await request.GetResponseAsync();
            // Get the stream
            var stream = response.GetResponseStream();
            // Read the stream
            StreamReader reader = new StreamReader(stream);
            string data = await reader.ReadToEndAsync();
            return data;
        }

        /// <summary>
        /// Merge and create a package list
        /// </summary>
        /// <param name="components">List of the components</param>
        /// <param name="versions">List of the versions</param>
        public void CreateResult(List<string> components, List<string> versions)
        {
            // Get the template string
            var template = GetTemplate();

            // Merge the components and versions into the string
            for (int i = 0; i < components.Count; i++)
            {
                Regex replacement = new Regex(string.Format("<{0}>", components[i]));
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
        public (string, string) ExtractVersion(string data, string component)
        {
            // Kind of a hack, just match it up in the filename in the HTML
            Regex pattern = new Regex(component + @"-(?<version>.*?)" + ".tar.bz2");
            Match match = pattern.Match(data);
            return (match.Groups["version"].Value, match.ToString());
        }

        /// <summary>
        /// Get the list of components
        /// </summary>
        /// <returns>List of the components</returns>
        private List<string> GetComponents()
        {
            List<string> components = new List<string>();
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                XElement element = XElement.Load(sr);
                var component_group = element.Element("components");

                // Add the component to the list
                foreach (var item in component_group.Elements())
                    components.Add(item.Value);
            }

            return components;
        }

        /// <summary>
        /// Get the components
        /// </summary>
        /// <returns>Template as the string</returns>
        private string GetTemplate()
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
        private string GetTemplateName()
        {
            return GetElementByName("template");
        }

        /// <summary>
        /// Get the name of the output
        /// </summary>
        /// <returns>Name of the output as the string</returns>
        private string GetOutputName()
        {
            return GetElementByName("output");
        }

        /// <summary>
        /// Get XML element from config by name.
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <returns>Element value as string.</returns>
        public static string GetElementByName(string name)
        {
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                XElement element = XElement.Load(sr);
                return element.Element(name).Value;
            }
        }
    }
}