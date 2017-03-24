namespace GnuPG_Buildkit_Package_Lister
{
    partial class Program
    {
        // This is suboptimal way of handling message strings.
        // But the problem is that .NET Core currently do not have built-in support for Resource Compiler...

        /// <summary>
        /// Messages string for the program.
        /// </summary>
        private struct messages
        {
            public static string logging_error = "Problem with the logging facility.\nPerhaps missing a config?";
            public static string debug_mode = "Compiled for debug.";
            public static string extract_version = "Extracting version informations...";
            public static string generate_list = "Generating the package list...";
            public static string get_remote = "Getting the contents from the remote...";
            public static string program_completed = "Program completed.";
            public static string program_starting = "Starting program...";
            public static string url_fetch = "URL to fetch: ";
        }
    }
}