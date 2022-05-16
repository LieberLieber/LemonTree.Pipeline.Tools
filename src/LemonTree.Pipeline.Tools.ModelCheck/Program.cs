using CommandLine;
using LemonTree.Pipeline.Tools.ModelCheck.Checks;
using LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions;
using System;
using System.IO;

namespace LemonTree.Pipeline.Tools.ModelCheck
{
    internal class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("RemovePrerenderedDiagrams is starting");

            // Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            return parser.ParseArguments<ModelCheckOptions>(args)
                .MapResult(
                    (ModelCheckOptions opts) => RunModelCheck(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }

        private static int RunModelCheck(ModelCheckOptions opts)
        {

            try
            {
                Issues issues = new Issues();

                Console.WriteLine($"ModelCheck on {opts.Model}");
                ModelAccess.ConfigureAccess("Microsoft.Jet.OLEDB.4.0", opts.Model);

                issues.AddIfNotNull(Checks.Checks.CheckDiagramImagemaps(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckBaseline(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckAudit(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckUserSecurity(opts.Model));
                
                issues.AddIfNotNull(Checks.Checks.CheckVCSConnection(opts.Model));
                
                issues.AddIfNotNull(Checks.Checks.CheckExtDoc(opts.Model));
                
                issues.AddIfNotNull(Checks.Checks.CheckModelDocument(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckCompact(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckStrippedCompact(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckT_image(opts.Model));

                if (opts.Out != null)
                {

                    File.WriteAllText(opts.Out, issues.ToMd());
                }

                return (int)Exitcode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
                return (int)Exitcode.Error;
            }
        }


    }
}
