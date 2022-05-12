using CommandLine;

namespace LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions
{
    [Verb("ModelCheck", HelpText = "Checks if a Model is optimized for LemonTree")]
    internal class ModelCheckOptions:BaseOptions
    {
        [Option("Out", Required = false, HelpText = "File to output .md e.g.: 'out.md'")]
        public string Out { get; set; }
    }
}
