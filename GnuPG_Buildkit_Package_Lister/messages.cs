namespace GnuPG_Buildkit_Package_Lister
{
    public static class Resources
    {
        // This is suboptimal way of handling message strings.
        // But the problem is that .NET Core currently do not have built-in support for Resource Compiler...

        /// <summary>
        /// Messages string for the program.
        /// </summary>
        internal struct Messages
        {
            public const string LoggingError = "Problem with the logging facility.\nPerhaps missing a config?";
            public const string DebugMode = "Compiled for debug.";
            public const string ExtractVersion = "Extracting version informations...";
            public const string GenerateList = "Generating the package list...";
            public const string GetRemote = "Getting the contents from the remote...";
            public const string ProgramCompleted = "Program completed.";
            public const string ProgramStarting = "Starting program...";
            public const string UrlFetch = "URL to fetch: ";
        }
    }
}