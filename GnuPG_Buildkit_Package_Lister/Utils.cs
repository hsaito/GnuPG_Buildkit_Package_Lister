using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GnuPG_Buildkit_Package_Lister
{
    public static class GnuPG_Buildkit_Package_Lister_Utils
    {
        /// <summary>
        /// Retrieve content from the web.
        /// </summary>
        /// <param name="location">URL of the website</param>
        /// <returns>String of the website</returns>
        public static async Task<string> GetWeb(string location)
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
    }
}