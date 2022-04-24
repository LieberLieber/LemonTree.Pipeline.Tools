using CommandLine;

namespace LemonTree.Pipeline.Tools.SetFilterInSessionFile.CommandLineOptions
{
    internal class BaseOptions
    {
        [Option("sfs", Required = true, HelpText = "The session as single file session.")]
        public string SingleFileSession { get; set; }
    }
}
