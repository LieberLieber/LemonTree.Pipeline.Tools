using CommandLine;

namespace LemonTree.Pipeline.Tools.Semantic.CommandLineOptions
{
    [Verb("Semantic", HelpText = "Checks if a Model is optimized for LemonTree")]
    internal class SemanticOptions:BaseOptions
    {
        [Option("DiffFile", Required = true, HelpText = ".xml Diff File that contains the model changes!")]
        public string DiffFile { get; set; }
  
    }
}
