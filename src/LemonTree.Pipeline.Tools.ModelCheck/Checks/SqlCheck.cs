using System;
using System.Data;
using System.Text;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    /// <summary>
    /// Standardized SQL-based check configuration.
    /// Defines a single check that can be executed via SQL query.
    /// </summary>
    internal class SqlCheck
    {
        /// <summary>
        /// Unique identifier for the check
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        /// SQL query that returns a count (scalar) value
        /// </summary>
        internal string Query { get; set; }

        /// <summary>
        /// Optional SQL query run when the check fails (count > 0).
        /// Should return the rows that caused the failure for reporting.
        /// </summary>
        internal string QueryOnFail { get; set; }

        /// <summary>
        /// Title when the check passes (count == 0)
        /// </summary>
        internal string PassedTitle { get; set; }

        /// <summary>
        /// Title when the check fails (count > 0)
        /// </summary>
        internal string FailedTitle { get; set; }

        /// <summary>
        /// Detail/explanation when the check passes
        /// </summary>
        internal string PassedDetail { get; set; }

        /// <summary>
        /// Detail/explanation when the check fails
        /// </summary>
        internal string FailedDetail { get; set; }

        /// <summary>
        /// Issue level when check passes (default: Passed)
        /// </summary>
        internal IssueLevel PassedLevel { get; set; } = IssueLevel.Passed;

        /// <summary>
        /// Issue level when check fails (default: Warning)
        /// </summary>
        internal IssueLevel FailedLevel { get; set; } = IssueLevel.Warning;

        /// <summary>
        /// Execute this check against the model database
        /// </summary>
        internal Issue Execute()
        {
            long count = ModelAccess.RunSQLQueryScalar(Query);

            Issue result = new Issue();

            if (count == 0)
            {
                result.Level = PassedLevel;
                result.Title = PassedTitle;
                result.Detail = PassedDetail;
            }
            else
            {
                result.Level = FailedLevel;
                result.Title = FailedTitle.Replace("{count}", count.ToString());
                result.Detail = FailedDetail;

                if (!string.IsNullOrWhiteSpace(QueryOnFail))
                {
                    DataTable dt = ModelAccess.RunSql(QueryOnFail);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        result.Markdown = DataTableToMarkdown(dt, $"Details: {Id}");
                    }
                }
            }

            return result;
        }

        private static string DataTableToMarkdown(DataTable dt, string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"### {title}");
            sb.Append("|");
            foreach (DataColumn col in dt.Columns)
                sb.Append($" {col.ColumnName} |");
            sb.AppendLine();
            sb.Append("|");
            foreach (DataColumn _ in dt.Columns)
                sb.Append("---|");
            sb.AppendLine();
            foreach (DataRow row in dt.Rows)
            {
                sb.Append("|");
                foreach (DataColumn col in dt.Columns)
                    sb.Append($" {row[col]} |");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
