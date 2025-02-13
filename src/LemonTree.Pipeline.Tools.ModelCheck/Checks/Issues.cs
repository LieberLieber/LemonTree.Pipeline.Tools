﻿using System;
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
            sb.AppendLine("| | Severity | Issue | Message |");
            sb.AppendLine("|----------|----------|---------|---------|");

            var mySortedList = this.OrderBy(x => x.Level);
            foreach (Issue issue in mySortedList)
            {
                sb.AppendLine($"|{issue.Symbol}|{issue.Level}|{issue.Title}|{issue.Detail}|");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var mySortedList = this.OrderBy(x => x.Level);
            foreach (Issue issue in mySortedList)
            {
                sb.AppendLine($"{issue.Level}; {issue.Title}");
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

        internal bool HasErrors()
        {
            //this can be done with Linq
            var mySortedList = this.OrderBy(x => x.Level);
            foreach (Issue issue in mySortedList)
            {
                if (issue.Level == IssueLevel.Error)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool HasWarnings()
        {
            //this can be done with Linq
            var mySortedList = this.OrderBy(x => x.Level);
            foreach (Issue issue in mySortedList)
            {
                if (issue.Level == IssueLevel.Warning)
                    return true;
            }

            return false;
        }

        internal void WriteOutPut(Issue issue)
        {
            throw new NotImplementedException();
        }
    }
}
