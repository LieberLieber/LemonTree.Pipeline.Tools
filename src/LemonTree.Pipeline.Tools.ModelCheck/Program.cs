using CommandLine;
using LemonTree.Pipeline.Tools.ModelCheck.Checks;
using LemonTree.Pipeline.Tools.ModelCheck.CommandLineOptions;
using System;
using System.IO;
using System.Reflection;
using System.Text;

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

            return parser.ParseArguments<ModelCheckOptions>(args)
                .MapResult(
                    (ModelCheckOptions opts) => RunModelCheck(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }

        private static int RunModelCheck(ModelCheckOptions opts)
        {
            try
            {
                var issues = new Issues();

                if (!File.Exists(opts.Model))
                {
                    Console.WriteLine($"File '{opts.Model}' does not exist.");
                    throw new FileNotFoundException("File not found.", opts.Model);
                }

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

                issues.AddIfNotNull(Checks.Checks.CheckResourceAllocation(opts.Model));

                issues.AddIfNotNull(Checks.Checks.CheckJournal(opts.Model));

                if (opts.NoCompact != true)
                {
                    issues.AddIfNotNull(Checks.Checks.CheckCompact(opts.Model));

                    issues.AddIfNotNull(Checks.Checks.CheckStrippedCompact(opts.Model));
                }

                Issue resultTableSize = null;
                if (opts.TableSize == true)
                {
                    if (ModelAccess.IsSqlLite())
                    {
                         resultTableSize = Checks.Checks.CheckTableSize(opts.Model);
                       
                        issues.AddIfNotNull(resultTableSize);
                    }
                    else
                    {
                        Console.WriteLine("Talbesize reporting only supported for SqlLite!");
                    }
                }

                Issue resultOrphans = null;
                if (opts.Orphans == true)
                {
                    if (ModelAccess.IsSqlLite())
                    {
                        resultOrphans = Checks.Checks.CheckModelOrphans(opts.Model);
                        issues.AddIfNotNull(resultOrphans);
                    }
                    else
                    {
                        Console.WriteLine("Orphans reporting only supported for SqlLite!");
                    }
                }

                Console.WriteLine(issues.ToString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(issues.ToMd());
                sb.AppendLine("# Project Statistics");
                sb.AppendLine(Checks.Checks.CheckProjectStatitics(opts.Model).Markdown);
                if (resultTableSize != null)
                {
                    sb.AppendLine(resultTableSize.Markdown);
                }
                if (resultOrphans != null)
                {
                    sb.AppendLine(resultOrphans.Markdown);
                }

                if (opts.Out != null)
                {
                    File.WriteAllText(opts.Out, sb.ToString());
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return (int)Exitcode.Error;
            }
        }
    }
}
