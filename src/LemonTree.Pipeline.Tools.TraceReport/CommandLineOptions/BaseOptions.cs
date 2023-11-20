using CommandLine;

namespace LemonTree.Pipeline.Tools.TraceReport.CommandLineOptions
{
    internal class BaseOptions
    {
        [Option("Model", Required = true, HelpText = "The  'Model' used for the operation.")]
        public string Model { get; set; }
    }
}
