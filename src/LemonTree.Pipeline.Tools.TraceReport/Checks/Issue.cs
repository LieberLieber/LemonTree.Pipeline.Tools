namespace LemonTree.Pipeline.Tools.TraceReport.Checks
{
    internal class Issue
    {
        internal IssueLevel Level { get; set; }
        internal string Detail { get; set; }
        internal string Title { get; set; }

        internal string Symbol
        {
            get
            {
                //https://github.com/ikatyang/emoji-cheat-sheet/blob/master/README.md
                switch (Level)
                {
                    case IssueLevel.Error:
                        return ":red_circle:";
                    case IssueLevel.Warning:
                        return ":orange_circle:";
                    case IssueLevel.Information:
                        return ":yellow_circle:";
                    case IssueLevel.Passed:
                        return ":green_circle:";
                    default:
                        return string.Empty;
                        
                }
               
            }
        }
    }
}
