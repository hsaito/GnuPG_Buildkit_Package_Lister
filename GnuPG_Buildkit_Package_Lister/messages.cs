namespace GnuPG_Buildkit_Package_Lister
{
    public class Resources
    {
        // This is suboptimal way of handling message strings.
        // But the problem is that .NET Core currently do not have built-in support for Resource Compiler...

        /// <summary>
        /// Messages string for the program.
        /// </summary>
        internal struct messages
        {
            public static readonly string logging_error = "Problem with the logging facility.\nPerhaps missing a config?";
            public static readonly string debug_mode = "Compiled for debug.";
            public static readonly string extract_version = "Extracting version informations...";
            public static readonly string generate_list = "Generating the package list...";
            public static readonly string get_remote = "Getting the contents from the remote...";
            public static readonly string program_completed = "Program completed.";
            public static readonly string program_starting = "Starting program...";
            public static readonly string url_fetch = "URL to fetch: ";
        }
    }
}