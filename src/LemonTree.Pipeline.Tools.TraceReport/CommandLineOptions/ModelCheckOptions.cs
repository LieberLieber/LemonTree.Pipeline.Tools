using CommandLine;

namespace LemonTree.Pipeline.Tools.TraceReport.CommandLineOptions
{
    [Verb("TraceReport", HelpText = "Creates a Tracebility Report")]
    internal class TraceReportOptions :BaseOptions
    {
        [Option("Out", Required = false, HelpText = "File to output .md e.g.: 'out.md'")]
        public string Out { get; set; }

        [Option("FailOnErrors", Required = false, HelpText = "If set the Exitcode will be 2 if there is at least on Check of Status Error!")]
        public bool FailOnErrors { get; set; }

        [Option("FailOnWarnings", Required = false, HelpText = "If set the Exitcode will be 1 if there is at least on Check of Status Warning!")]
        public bool FailOnWarnings { get; set; }
    }
}
