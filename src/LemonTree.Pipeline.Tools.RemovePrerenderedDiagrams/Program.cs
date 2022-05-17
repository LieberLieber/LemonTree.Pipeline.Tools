using CommandLine;
using LemonTree.Pipeline.Tools;
using LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.CommandLineOptions;
using System;
using System.Data.OleDb;
using System.IO;

namespace RemovePrerenderedDiagrams
{
    class Program
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

            return parser.ParseArguments<RemoveOptions>(args)
                .MapResult(
                    (RemoveOptions opts) => RunRemove(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);

        }

     

        private static int RunRemove(RemoveOptions opts)
        {
            try
            {


                Console.WriteLine($"RemovePrerenderedDiagrams from {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);
                

                int retVal = ModelAccess.RunSQLnonQuery("Delete from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");
                Console.WriteLine($"Removed {retVal} Prerendered Diagrams from {opts.Model}");
                Console.WriteLine("RemovePrerenderedDiagrams is finished");
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
