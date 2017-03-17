using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GnuPG_Buildkit_Package_Lister
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static async Task Process()
        {
            try
            {
                string url = "";
                var components = GetComponents();
                using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
                {
                    XElement element = XElement.Load(sr);
                    url = element.Element("url").Value;
                }

                string content = await GetWeb(url);

                List<string> versions = new List<string>();

                foreach (var item in components)
                    versions.Add(ExtractVersion(content, item));

                CreateResult(components, versions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static async Task<string> GetWeb(string location)
        {
            var request = WebRequest.Create(location);
            var response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string data = await reader.ReadToEndAsync();
            return data;
        }

        static void CreateResult(List<string> components, List<string> versions)
        {
            var template = GetTemplate();

            for (int i = 0; i < components.Count; i++)
            {
                Regex replacement = new Regex(string.Format("<{0}>", components[i]));
                template = replacement.Replace(template, versions[i]);
            }


            using (var sw = new StreamWriter(new FileStream(GetOutputName(), FileMode.Create)))
            {
                sw.Write(template);
                sw.Flush();
            }
        }

        static string ExtractVersion(string data, string component)
        {
            Regex pattern = new Regex(component + @"-(?<version>.*?)" + ".tar.bz2");
            Match match = pattern.Match(data);
            return match.Groups["version"].Value;
        }

        static List<string> GetComponents()
        {
            List<string> components = new List<string>();
            using (var sr = File.OpenText("config.xml"))
            {
                XElement element = XElement.Load(sr);
                var component_group = element.Element("components");

                foreach (var item in component_group.Elements())
                    components.Add(item.Value);
            }

            return components;
        }

        static string GetTemplate()
        {
            using (var sr = new StreamReader(new FileStream(GetTemplateName(), FileMode.Open)))
            {
                return sr.ReadToEnd();
            }
        }

        static string GetTemplateName()
        {
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                XElement element = XElement.Load(sr);
                return element.Element("template").Value;
            }
        }

        static string GetOutputName()
        {
            using (var sr = new StreamReader(new FileStream("config.xml", FileMode.Open)))
            {
                XElement element = XElement.Load(sr);
                return element.Element("output").Value;
            }
        }
    }
}