using CommandLine;

namespace LemonTree.Pipeline.Tools.GetModelRoots.CommandLineOptions
{
    [Verb("Roots", HelpText = "Checks if a Model is optimized for LemonTree")]
    internal class GetModelRootsOptions : BaseOptions
    {
        [Option("Ignore", Required = false, HelpText = "A guid of a specific root you don't want to be listed e.g MPMS Stuff")]
        public string Ignore { get; set; }

    }
}
