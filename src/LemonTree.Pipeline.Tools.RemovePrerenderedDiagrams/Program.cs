using CommandLine;
using LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.CommandLineOptions;
using System;
using System.Data.OleDb;
using System.IO;

namespace RemovePrerenderedDiagrams
{
    class Program
    {
        private static OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();

        public static int RunSQLnonQuery(string sql)
        {
            int RecordCount = -1;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    RecordCount = cmd.ExecuteNonQuery();
                }
            }

            return RecordCount;
        }
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
                _builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                _builder.DataSource = opts.Model;

                int retVal = RunSQLnonQuery("Delete from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");
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
