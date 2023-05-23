using CommandLine;
using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using LemonTree.Pipeline.Tools.SemanticVersioning.Runner.CommandLineOptions;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner
{
    internal class Program
    {
	    private static IEnumerable<ISemanticVersioningRule> _rules;

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Unhandled Exception: {(e.ExceptionObject as Exception).Message}");
		}

		static int Main(string[] args)
        {
			//Sample Options
			// --model Semantic-Change.qeax --changes export_202302240831421465.xml

			int returnValue = -1;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			try
			{
				// Parser.Default is case sensitive - so we use a custom parser to support caseINsensitive verbs and options
				var parser = new Parser(settings =>
				{
					settings.CaseSensitive = false;
					settings.HelpWriter = Console.Error;
				});

				var pluginLoader = new PluginLoader();
				pluginLoader.Run();
				_rules = pluginLoader.Rules;

				returnValue = parser.ParseArguments<SemanticOptions>(args)
					.MapResult(
						RunSemanticVersioning,
						_ => (int)ExitCode.ErrorCmdParameter);

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			return returnValue;
        }

        private static int RunSemanticVersioning(SemanticOptions opts)
        {
            try
            {
                if (!File.Exists(opts.Model))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.Model}");
                    return (int)ExitCode.ErrorCmdParameter;
                }

                if (!File.Exists(opts.Changes))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.Changes}");
                    return (int)ExitCode.ErrorCmdParameter;
                }

                Console.WriteLine($"Semantic on: {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);

                var semVer = new SemanticVersioning(_rules);
                semVer.Run(opts.Changes);

				WriteStatistics(semVer.RunStatistics);

				return (int)ExitCode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return (int)ExitCode.Error;
            }
        }

		private static void WriteStatistics(Statistics runStatistics)
		{
			Console.WriteLine("Applied rules:");
			var numberOfAppliedByRuleName = runStatistics.NumberOfAppliedByRuleName();
			foreach (var item in numberOfAppliedByRuleName)
			{
				Console.WriteLine($"- {item.Key}: {item.Value}");
			}

			Console.WriteLine("Applied changes:");
			var numberOfAppliedByChangeLevel = runStatistics.NumberOfAppliedByChangeLevel();
			foreach (var item in numberOfAppliedByChangeLevel)
			{
				Console.WriteLine($"- {item.Key}: {item.Value}");
			}
		}
	}
}
