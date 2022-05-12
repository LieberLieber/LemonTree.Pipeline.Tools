using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    internal class Issues : List<Issue>
    {
        internal string ToMd()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# LemonTree ModelCheck results");
            sb.AppendLine("| Severity | Issue | Message |");
            sb.AppendLine("|----------|---------|---------|");

            var mySortedList = this.OrderBy(x => x.Level);
            foreach (Issue issue in mySortedList)
            {
                sb.AppendLine($"|{issue.Level}|{issue.Title}|{issue.Detail}|");
            }
            return sb.ToString();
        }

        internal void AddIfNotNull(Issue element)
        {
            if (element != null)
            {
                this.Add(element);
            }
        }
    }
}
