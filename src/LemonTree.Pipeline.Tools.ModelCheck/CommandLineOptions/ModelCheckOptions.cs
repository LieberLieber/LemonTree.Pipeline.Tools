using CommandLine;

namespace LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions
{
    [Verb("ModelCheck", HelpText = "Checks if a Model is optimized for LemonTree")]
    internal class ModelCheckOptions:BaseOptions
    {
        [Option("Out", Required = false, HelpText = "File to output .md e.g.: 'out.md'")]
        public string Out { get; set; }

        [Option("NoCompact", Required = false, HelpText = "If set the Checks that compact the Model are not run!")]
        public bool NoCompact { get; set; }

        [Option("FailOnErrors", Required = false, HelpText = "If set the Exitcode will be 2 if there is at least on Check of Status Error!")]
        public bool FailOnErrors { get; set; }

        [Option("FailOnWarnings", Required = false, HelpText = "If set the Exitcode will be 1 if there is at least on Check of Status Warning!")]
        public bool FailOnWarnings { get; set; }

        [Option("TableSize", Required = false, HelpText = "If set the size of the tables in the database will be reported!")]
        public bool TableSize { get; set; }

        [Option("Orphans", Required = false, HelpText = "If Model Orphans will be reported!")]
        public bool Orphans { get; set; }
    }
}
