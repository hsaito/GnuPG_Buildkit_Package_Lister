using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace GnuPG_Buildkit_Package_Lister
{
    class Program
    {
        static void Main(string[] args)
        {
            GetWeb("https://gnupg.org/download/index.html").Wait();
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
    }
}