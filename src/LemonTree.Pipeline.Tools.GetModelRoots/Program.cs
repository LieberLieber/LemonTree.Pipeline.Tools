using CommandLine;
using LemonTree.Pipeline.Tools.GetModelRoots.CommandLineOptions;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.GetModelRoots
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //Sample Options
            // --Model Model.qeax --Ignore {5BEEF931-53C8-4FE3-A607-71FACADE381B}

            // Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            return parser.ParseArguments<GetModelRootsOptions>(args)
                .MapResult(
                    (GetModelRootsOptions opts) => RunGetModelRoots(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }

        private static int RunGetModelRoots(GetModelRootsOptions opts)
        {
       
            try
            {
                if (!File.Exists(opts.Model))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.Model}");
                    return (int)Exitcode.ErrorCmdParameter;
                }

                //Console.WriteLine($"Get Model rootls from {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);

                DataTable dataTable = ModelAccess.RunSql("select ea_guid,name from t_package where parent_id = 0");

                foreach (DataRow row in dataTable.Rows)
                {
                    if (opts.Ignore != row.ItemArray[0].ToString())
                    {
                        Console.WriteLine($"{row.ItemArray[0]}");
                    }
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
