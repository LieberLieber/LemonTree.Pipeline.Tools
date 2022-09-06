using CommandLine;
using LemonTree.Pipeline.Tools.ModelCheck.Checks;
using LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions;
using System;
using System.IO;
using System.Reflection;

namespace LemonTree.Pipeline.Tools.ModelCheck
{
    internal class Program
    {
        static int Main(string[] args)
        {
            // Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            //RunMyTest();

            return parser.ParseArguments<ModelCheckOptions>(args)
                .MapResult(
                    (ModelCheckOptions opts) => RunModelCheck(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }



        //private static void RunMyTest()
        //{
        //    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        //    UriBuilder uri = new UriBuilder(codeBase);
        //    string path = Uri.UnescapeDataString(uri.Path);
        //    var path2 = Path.GetDirectoryName(path);
        //    var path3 = Path.Combine(path2, "Model.qeax");

        //    ModelCheckOptions myOpts = new ModelCheckOptions
        //    {
        //        Model = path3
        //    };

        //    ModelAccess.ConfigureAccess(myOpts.Model);

        //    var issue = Checks.Checks.CheckProjectStatitics(myOpts.Model);
        //}

        private static int RunModelCheck(ModelCheckOptions opts)
        {



            try
            {
                Issues issues = new Issues();

                Console.WriteLine($"ModelCheck on {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);

                issues.AddIfNotNull(Checks.Checks.CheckDiagramImagemaps(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckBaseline(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckAuditEnabled(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckAuditLogs(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckUserSecurity(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckVCSConnection(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckExtDoc(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckModelDocument(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckT_image(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckProjectStatitics(opts.Model));

                if (opts.NoCompact != true)
                {
                    issues.AddIfNotNull(Checks.Checks.CheckCompact(opts.Model));

                    issues.AddIfNotNull(Checks.Checks.CheckStrippedCompact(opts.Model));
                }

                Console.WriteLine(issues.ToString());

                if (opts.Out != null)
                {

                    File.WriteAllText(opts.Out, issues.ToMd());
                }

                if (opts.FailOnErrors == true)
                {
                    if (issues.HasErrors())
                    {
                        return (int)Exitcode.ModelError;
                    }
                }

                if (opts.FailOnWarnings == true)
                {
                    if (issues.HasWarnings())
                    {
                        return (int)Exitcode.ModelWarning;
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
