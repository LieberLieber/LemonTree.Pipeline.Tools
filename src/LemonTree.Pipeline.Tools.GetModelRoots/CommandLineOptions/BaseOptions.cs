using CommandLine;

namespace LemonTree.Pipeline.Tools.GetModelRoots.CommandLineOptions
{
    internal class BaseOptions
    {
        [Option("Model", Required = true, HelpText = "The  'Model' used for the operation.")]
        public string Model { get; set; }
    }
}
