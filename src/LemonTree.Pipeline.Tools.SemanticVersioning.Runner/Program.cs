using CommandLine;
using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using LemonTree.Pipeline.Tools.SemanticVersioning.Runner.CommandLineOptions;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner
{
    internal class Program
    {
	    private static IEnumerable<ISemanticVersioningRule> _rules;

        static int Main(string[] args)
        {
			//Sample Options
			// --model Semantic-Change.eapx --changes export_202302240831421465.xml

			// Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
			var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            var pluginLoader = new PluginLoader();
            pluginLoader.Run();
            _rules = pluginLoader.Rules;

				return parser.ParseArguments<SemanticOptions>(args)
                .MapResult(
	                RunSemanticVersioning,
                    _ => (int)ExitCode.ErrorCmdParameter);
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

                Console.WriteLine($"Semantic on {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);

                var semVer = new SemanticVersioning(_rules);
                semVer.Run(opts.Changes);

                return (int)ExitCode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return (int)ExitCode.Error;
            }
        }
    }
}
