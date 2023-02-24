using CommandLine;
using LemonTree.Pipeline.Tools.Semantic.CommandLineOptions;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.Semantic
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //Sample Options
            // --model Semantic-Change.eapx --difffile export_202302240831421465.xml

            // Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            return parser.ParseArguments<SemanticOptions>(args)
                .MapResult(
                    (SemanticOptions opts) => RunModelCheck(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }

        private static int RunModelCheck(SemanticOptions opts)
        {
            try
            {
                if (!File.Exists(opts.Model))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.Model}");
                    return (int)Exitcode.ErrorCmdParameter;
                }

                if (!File.Exists(opts.DiffFile))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.DiffFile}");
                    return (int)Exitcode.ErrorCmdParameter;
                }

                Console.WriteLine($"Semantic on {opts.Model}");
                ModelAccess.ConfigureAccess(opts.Model);


                if (!File.Exists(opts.DiffFile))
                {
                    //Check File exists and is kinda XML.
                    Console.WriteLine($"File not Found: {opts.DiffFile}");
                    return (int)Exitcode.ErrorCmdParameter;
                }

                var doc = XDocument.Parse(File.ReadAllText(opts.DiffFile));
                var elements = doc.Root.Descendants().Where(item => item.Name.LocalName == "element");
                var modified = elements.Where(item => item.Attributes().Any(attribute => attribute.Name.LocalName == "diffState" && attribute.Value == "Modified"));

                foreach (var modifedElement in modified)
                {

                    string guid = modifedElement.Attribute("guid").Value.ToString();
                    string version = GetVersionInfoFormElement(guid);
                    string newVersion = "2.0";
                    Console.WriteLine($"{guid} - {version} ==> {newVersion}");
                    UpdateVersion(guid, newVersion);

                }
                // run the rules here and update the Model Element via SQL UPdate.

                return (int)Exitcode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
                return (int)Exitcode.Error;
            }
        }

        private static void UpdateVersion(string guid, string newVersion)
        {
            ModelAccess.RunSql($"Update t_object Set t_object.version =\"{newVersion}\" where t_object.ea_guid = \"{ guid}\"");
        }

        private static string GetVersionInfoFormElement(string guid)
        {
            return ModelAccess.RunSQLQueryScalarAsString($"SELECT DISTINCT t_object.version FROM t_object  Where t_object.ea_guid = \"{ guid}\"");
        }

        private static bool EvaluateVersionRules(XElement modifedElement)
        {
            return true;
        }
    }
}
