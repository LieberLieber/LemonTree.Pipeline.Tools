using CommandLine;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner.CommandLineOptions
{
	internal class BaseOptions
	{
		[Option("Model", Required = true, HelpText = "The 'model' to apply the semantic versioning")]
		public string Model { get; set; }

		[Option("TryRun", Required = false, HelpText = "Just analyze, but do not write to model")]
		public bool TryRun { get; set; }
	}
}
