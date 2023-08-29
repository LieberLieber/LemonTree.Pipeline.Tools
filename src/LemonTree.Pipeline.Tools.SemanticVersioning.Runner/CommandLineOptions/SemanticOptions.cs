using CommandLine;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner.CommandLineOptions
{
	[Verb("Semantic", HelpText = "Run semantic versioning on model for changes from input file")]
	internal class SemanticOptions : BaseOptions
	{
		[Option("Changes", Required = true, HelpText = ".xml file that contains the model changes")]
		public string Changes { get; set; }
	}
}
