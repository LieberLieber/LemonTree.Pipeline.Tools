using System.Linq;
using System.Xml;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    internal static class JUnitReporter
    {
        private const string TestSuiteName = "LemonTree ModelCheck";
        private const string ClassName = "LemonTree.ModelCheck";

        /// <summary>
        /// Generates a JUnit XML report from the given issues and writes it to the specified file.
        /// 
        /// Severity mapping:
        ///   Passed      → testcase (no child elements = passed)
        ///   Information → testcase with &lt;skipped&gt; child (inconclusive)
        ///   Warning     → testcase with &lt;failure type="Warning"&gt; child (assertion did not pass)
        ///   Error       → testcase with &lt;failure type="Error"&gt; child (validation found an error)
        /// </summary>
        internal static void WriteJUnitReport(Issues issues, string outputPath)
        {
            var sortedIssues = issues.OrderBy(x => x.Level).ToList();

            int tests = sortedIssues.Count;
            int failures = sortedIssues.Count(i => i.Level == IssueLevel.Error || i.Level == IssueLevel.Warning);
            int errors = 0;
            int skipped = sortedIssues.Count(i => i.Level == IssueLevel.Information);

            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            var testsuites = doc.CreateElement("testsuites");
            doc.AppendChild(testsuites);

            var testsuite = doc.CreateElement("testsuite");
            testsuite.SetAttribute("name", TestSuiteName);
            testsuite.SetAttribute("tests", tests.ToString());
            testsuite.SetAttribute("failures", failures.ToString());
            testsuite.SetAttribute("errors", errors.ToString());
            testsuite.SetAttribute("skipped", skipped.ToString());
            testsuites.AppendChild(testsuite);

            foreach (var issue in sortedIssues)
            {
                var testcase = doc.CreateElement("testcase");
                testcase.SetAttribute("classname", ClassName);
                testcase.SetAttribute("name", issue.Title ?? string.Empty);

                switch (issue.Level)
                {
                    case IssueLevel.Error:
                        var errorFailure = doc.CreateElement("failure");
                        errorFailure.SetAttribute("type", "Error");
                        errorFailure.SetAttribute("message", issue.Detail ?? string.Empty);
                        errorFailure.InnerText = issue.Detail ?? issue.Level.ToString();
                        testcase.AppendChild(errorFailure);
                        break;

                    case IssueLevel.Warning:
                        var failure = doc.CreateElement("failure");
                        failure.SetAttribute("type", "Warning");
                        failure.SetAttribute("message", issue.Detail ?? string.Empty);
                        failure.InnerText = issue.Detail ?? issue.Level.ToString();
                        testcase.AppendChild(failure);
                        break;

                    case IssueLevel.Information:
                        var skippedNode = doc.CreateElement("skipped");
                        skippedNode.SetAttribute("message", issue.Detail ?? string.Empty);
                        testcase.AppendChild(skippedNode);
                        break;

                    // IssueLevel.Passed: no child element needed
                }

                testsuite.AppendChild(testcase);
            }

            doc.Save(outputPath);
        }
    }
}
