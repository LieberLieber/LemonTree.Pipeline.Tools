using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    internal static class JsonReporter
    {
        /// <summary>
        /// Writes a JSON details report of all check results to the specified file.
        /// All checks are included; affectedElements is populated only for failed checks with QueryOnFail results.
        /// </summary>
        internal static void WriteJsonReport(Issues issues, string outputPath)
        {
            var checks = issues
                .OrderBy(x => x.Level)
                .Select(i => new
                {
                    id = i.Id,
                    level = i.Level.ToString(),
                    title = i.Title,
                    detail = i.Detail,
                    affectedElements = i.AffectedElements ?? new List<Dictionary<string, string>>()
                })
                .ToList();

            var report = new { checks };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(report, options);
            File.WriteAllText(outputPath, json);
        }
    }
}
