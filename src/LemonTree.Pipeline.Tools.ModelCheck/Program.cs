using CommandLine;
using LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions;
using System;

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


                Console.WriteLine($"ModelCheck on {opts.Model}");


                //ModelAccess.ConfigureAccess("Microsoft.Jet.OLEDB.4.0", opts.Model);

                //int retVal = ModelAccess.RunSQLnonQuery("Delete from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");
            
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
