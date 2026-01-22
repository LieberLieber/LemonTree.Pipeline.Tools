using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    /// <summary>
    /// Registry of standardized SQL-based checks.
    /// Centralizes check configuration and execution logic.
    /// Supports loading checks from JSON configuration file or using hardcoded defaults.
    /// </summary>
    internal static class SqlCheckRegistry
    {
        private static List<SqlCheck> _checks = null;
        private static List<string> _checkOrder = null;
        private static string _configPath = null;

        /// <summary>
        /// Set the path to the checks configuration file
        /// </summary>
        internal static void SetConfigPath(string configPath)
        {
            _configPath = configPath;
            // Reset cached values so they'll be reinitialized with the new config
            _checks = null;
            _checkOrder = null;
        }

        /// <summary>
        /// Get the ordered list of check IDs for execution
        /// </summary>
        internal static List<string> GetCheckOrder()
        {
            if (_checkOrder == null)
            {
                _checkOrder = InitializeCheckOrder();
            }
            return _checkOrder;
        }

        /// <summary>
        /// Define the order in which checks should be executed
        /// Attempts to load from JSON file first, falls back to hardcoded defaults
        /// </summary>
        private static List<string> InitializeCheckOrder()
        {
            // Try to load from JSON configuration file
            var jsonChecks = TryLoadChecksFromJson(_configPath);
            if (jsonChecks != null && jsonChecks.Count > 0)
            {
                // Extract order from the checks as they appear in JSON
                var order = new List<string>();
                foreach (var check in jsonChecks)
                {
                    order.Add(check.Id);
                }
                return order;
            }

            // Fallback to hardcoded defaults
            return new List<string>
            {
                "DiagramImagemaps",
                "Baselines",
                "AuditingEnabled",
                "AuditLogs",
                "UserSecurity",
                "VCSConnection",
                "ExtDoc",
                "ModelDocuments",
                "TImages",
                "ResourceAllocation",
                "Journal"
            };
        }

        /// <summary>
        /// Get or initialize the list of all available checks
        /// </summary>
        internal static List<SqlCheck> GetAllChecks()
        {
            if (_checks == null)
            {
                _checks = InitializeChecks();
            }
            return _checks;
        }

        /// <summary>
        /// Initialize all SQL-based checks with their queries and messages
        /// Attempts to load from JSON file first, falls back to hardcoded defaults
        /// </summary>
        private static List<SqlCheck> InitializeChecks()
        {
            // Try to load from JSON configuration file
            var jsonChecks = TryLoadChecksFromJson(_configPath);
            if (jsonChecks != null && jsonChecks.Count > 0)
            {
                return jsonChecks;
            }

            // Fallback to hardcoded defaults
            return HardcodedChecks.GetHardcodedChecks();
        }

        /// <summary>
        /// Execute all checks and return their results
        /// </summary>
        internal static Issues ExecuteAllChecks()
        {
            Issues results = new Issues();
            foreach (var check in GetAllChecks())
            {
                results.AddIfNotNull(ExecuteCheck(check));
            }
            return results;
        }

        /// <summary>
        /// Execute all SQL-based checks in their defined order
        /// </summary>
        internal static Issues ExecuteAllSqlChecks()
        {
            Issues results = new Issues();
            foreach (string checkId in GetCheckOrder())
            {
                results.AddIfNotNull(ExecuteCheck(checkId));
            }
            return results;
        }

        /// <summary>
        /// Execute a specific check by ID
        /// </summary>
        internal static Issue ExecuteCheck(string checkId)
        {
            var check = GetAllChecks().Find(c => c.Id == checkId);
            if (check == null)
            {
                return null;
            }
            return ExecuteCheck(check);
        }

        /// <summary>
        /// Execute a check and handle wildcard replacement for database-specific SQL
        /// </summary>
        private static Issue ExecuteCheck(SqlCheck check)
        {
            // Replace wildcard placeholder with database-specific wildcard
            string query = check.Query.Replace("{wildcard}", ModelAccess.GetWildcard());
            
            // Temporarily modify the check's query for execution
            string originalQuery = check.Query;
            check.Query = query;
            
            try
            {
                return check.Execute();
            }
            finally
            {
                // Restore original query
                check.Query = originalQuery;
            }
        }

        /// <summary>
        /// Try to load checks from JSON configuration file
        /// Returns null if file doesn't exist or fails to parse
        /// </summary>
        private static List<SqlCheck> TryLoadChecksFromJson(string customConfigPath = null)
        {
            try
            {
                // Use custom path if provided, otherwise fall back to default location
                string configPath = customConfigPath;
                if (string.IsNullOrEmpty(configPath))
                {
                    configPath = Path.Combine(AppContext.BaseDirectory, "checks-config.json");
                }
                
                if (!File.Exists(configPath))
                {
                    return null;
                }

                string json = File.ReadAllText(configPath);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("checks", out var checksElement))
                    {
                        var checks = new List<SqlCheck>();
                        foreach (var checkJson in checksElement.EnumerateArray())
                        {
                            var check = ParseCheckFromJson(checkJson);
                            if (check != null)
                            {
                                checks.Add(check);
                            }
                        }
                        return checks.Count > 0 ? checks : null;
                    }
                }
                return null;
            }
            catch
            {
                // If JSON loading fails, return null to use hardcoded defaults
                return null;
            }
        }

        /// <summary>
        /// Parse a single check from JSON element
        /// </summary>
        private static SqlCheck ParseCheckFromJson(JsonElement checkElement)
        {
            try
            {
                var check = new SqlCheck();

                if (checkElement.TryGetProperty("id", out var id))
                    check.Id = id.GetString();

                if (checkElement.TryGetProperty("query", out var query))
                    check.Query = query.GetString();

                if (checkElement.TryGetProperty("passedTitle", out var passedTitle))
                    check.PassedTitle = passedTitle.ValueKind == JsonValueKind.Null ? null : passedTitle.GetString();

                if (checkElement.TryGetProperty("failedTitle", out var failedTitle))
                    check.FailedTitle = failedTitle.GetString();

                if (checkElement.TryGetProperty("passedDetail", out var passedDetail))
                    check.PassedDetail = passedDetail.ValueKind == JsonValueKind.Null ? null : passedDetail.GetString();

                if (checkElement.TryGetProperty("failedDetail", out var failedDetail))
                    check.FailedDetail = failedDetail.ValueKind == JsonValueKind.Null ? null : failedDetail.GetString();

                if (checkElement.TryGetProperty("passedLevel", out var passedLevel))
                    check.PassedLevel = ParseIssueLevel(passedLevel.GetString(),IssueLevel.Passed);

                if (checkElement.TryGetProperty("failedLevel", out var failedLevel))
                    check.FailedLevel = ParseIssueLevel(failedLevel.GetString(),IssueLevel.Error);

                return check;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse issue level from string
        /// </summary>
        private static IssueLevel ParseIssueLevel(string level, IssueLevel defaultLevel = IssueLevel.Information)
        {
            return level?.ToLower() switch
            {
                "error" => IssueLevel.Error,
                "warning" => IssueLevel.Warning,
                "information" => IssueLevel.Information,
                "passed" => IssueLevel.Passed,
                _ => defaultLevel
            };
        }
    }
}
