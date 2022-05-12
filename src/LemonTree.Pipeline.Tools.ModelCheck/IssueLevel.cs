using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonTree.Pipeline.Tools.ModelCheck
{
    internal enum IssueLevel
    {
        Error = -2,
        Warning = -1,
        Information = 0
    }

    internal class Issue
    {
        internal IssueLevel Level { get; set; }
        internal string Text { get; set; }
        internal string Name { get; set; }
    }

    internal class Issues : SortedList<IssueLevel, Issue>
    {

    }

}
