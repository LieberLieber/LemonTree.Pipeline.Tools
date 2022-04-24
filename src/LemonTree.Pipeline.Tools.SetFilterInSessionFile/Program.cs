using CommandLine;
using LemonTree.Pipeline.Tools.SetFilterInSessionFile.CommandLineOptions;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace SetFilterInSessionFile
{
    class Program
    {
        static int Main(string[] args)
        {


            //Sample Commandline: SampleCompareSession.ltsfs "#Conflicted" "$HideGraphicalChanges "
            Console.WriteLine("SetFilterInSessionFile is starting");

            // Parser.Default is case sensitive - so we use a custom parser to support in case sensitive verbs and options
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
            });

            return parser.ParseArguments<ConflictedOptions, CustomOptions>(args)
                .MapResult(
                    (ConflictedOptions opts) => RunConflicted(opts),
                    (CustomOptions opts) => RunCustom(opts),
                    _ => (int)Exitcode.ErrorCmdParameter);
        }

        private static int RunCustom(CustomOptions opts)
        {
            return (int)SetFilterInFile(opts.SingleFileSession, opts.ImpactedElements, opts.ImpactedDiagrams);
        }

        private static int RunConflicted(ConflictedOptions opts)
        {
            return (int)SetFilterInFile(opts.SingleFileSession, "#Conflicted", "#Conflicted");
        }

        private static Exitcode SetFilterInFile(string filename, string impactedElementsValue, string impactedDiagramsValue)
        {
            try
            {
                using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Update))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".ltses"))
                        {
                            string text = "";
                            Console.WriteLine($"Modifying {entry.FullName} inside {filename}");
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {
                                //*.ltses files from Automation miss the UiConfiguration so we rebuild it and set filters.
                                text = reader.ReadToEnd();
                                XmlDocument xmldoc = new XmlDocument();
                                xmldoc.LoadXml(text);
                                AddOrUpdateXmlElement("", xmldoc, "/LemonTreeSession/UIConfiguration", "WorkflowActions");
                                AddOrUpdateXmlElement(impactedElementsValue, xmldoc, "/LemonTreeSession/UIConfiguration", "ImpactedElementsFilter");
                                AddOrUpdateXmlElement(impactedDiagramsValue, xmldoc, "/LemonTreeSession/UIConfiguration", "ImpactedDiagramsFilter");
                                AddOrUpdateXmlElement("Diff", xmldoc, "/LemonTreeSession/UIConfiguration", "Perspective");
                                AddOrUpdateXmlElement("true", xmldoc, "/LemonTreeSession/UIConfiguration", "NewSessionDialogThreeWayComparison");
                                text = xmldoc.OuterXml;
                            }

                            using (var stream = entry.Open())
                            {
                                stream.SetLength(text.Length);
                                using (StreamWriter writer = new StreamWriter(stream))
                                {
                                    writer.Write(text);
                                    writer.Flush();
                                }
                            }
                        }
                    }
                    WriteReadmeIntoZip(new string[] { filename, impactedElementsValue, impactedDiagramsValue}, archive);
                    Console.WriteLine("SetFilterInSessionFile is finished!");
                    return Exitcode.Success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
                return Exitcode.Error;
            }
        }

        private static void WriteReadmeIntoZip(string[] args, ZipArchive archive)
        {
            var entry2 = archive.CreateEntry("readme.txt");
            using (var writer = new StreamWriter(entry2.Open()))
            {
                writer.WriteLine("The .ltses File was modified by SetFilterinSession");
                writer.WriteLine("With Commmandline " + string.Join(" ", args));
            }
        }

        private static void AddOrUpdateXmlElement(string xmlElementValue, XmlDocument xmldoc, string XmlPath, string XmlElementName)
        {
            Console.WriteLine($"Set {XmlPath}/{XmlElementName} to Value \"{xmlElementValue}\"");
            XmlElement node1 = xmldoc.SelectSingleNode($"{XmlPath}/{XmlElementName}") as XmlElement;
            if (node1 != null)
            {
                node1.InnerText = xmlElementValue; // if you want a text
            }
            else
            {
                XmlElement node3 = xmldoc.SelectSingleNode(XmlPath) as XmlElement;

                //node3.InnerText = $"<{XmlElementName}>{xmlElementValue}</{XmlElementName}>";
                if (node3 != null)
                {
                    var newRec = xmldoc.CreateElement(XmlElementName);
                    newRec.InnerText = xmlElementValue;
                    node3.AppendChild(newRec);
                    node3.Attributes.RemoveAll();
                }
            }
        }
    }
}
