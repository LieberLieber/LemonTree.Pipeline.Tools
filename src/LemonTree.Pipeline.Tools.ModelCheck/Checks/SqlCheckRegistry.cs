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
            var jsonChecks = TryLoadChecksFromJson();
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
            var jsonChecks = TryLoadChecksFromJson();
            if (jsonChecks != null && jsonChecks.Count > 0)
            {
                return jsonChecks;
            }

            // Fallback to hardcoded defaults
            return GetHardcodedChecks();
        }

        /// <summary>
        /// Get the hardcoded default checks
        /// </summary>
        private static List<SqlCheck> GetHardcodedChecks()
        {
            return new List<SqlCheck>
            {
                // DIAGRAMIMAGEMAP Check
                new SqlCheck
                {
                    Id = "DiagramImagemaps",
                    Query = "Select Count(*) from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP'",
                    PassedTitle = "No DIAGRAMIMAGEMAP entries in the model",
                    FailedTitle = "Model has {count} DIAGRAMIMAGEMAPS",
                    PassedDetail = null,
                    FailedDetail = "This is perfect if you use it with WebEA and Prolaborate but makes merging/diffing harder",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Information,
                    IncludeCountInTitle = true
                },

                // T_Image Check
                new SqlCheck
                {
                    Id = "TImages",
                    Query = "Select Count(*) from t_image",
                    PassedTitle = "No t_image entries in the model",
                    FailedTitle = "Model has {count} t_image entries",
                    PassedDetail = null,
                    FailedDetail = "Binary image data makes the model bigger!",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Information,
                    IncludeCountInTitle = true
                },

                // Baseline Check
                new SqlCheck
                {
                    Id = "Baselines",
                    Query = "SELECT Count(*) FROM t_document where t_document.DocType = 'Baseline'",
                    PassedTitle = "No Baseline entries in the model",
                    FailedTitle = "Model has {count} Baselines",
                    PassedDetail = null,
                    FailedDetail = "Baselines are not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                    IncludeCountInTitle = true
                },

                // ExtDoc Check
                new SqlCheck
                {
                    Id = "ExtDoc",
                    Query = "SELECT Count(*) FROM t_document where DocType = 'ExtDoc'",
                    PassedTitle = "No embedded binary images or document entries in the model",
                    FailedTitle = "Model has {count} embedded binary images or document entries.",
                    PassedDetail = null,
                    FailedDetail = "Embedded binary files will increase your model size, it is advised to check if they are required.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                    IncludeCountInTitle = true
                },

                // ModelDocument Check
                new SqlCheck
                {
                    Id = "ModelDocuments",
                    Query = "SELECT Count(*) FROM t_document where DocType = 'ModelDocument'",
                    PassedTitle = "No ModelDocument entries in the model",
                    FailedTitle = "Model has {count} ModelDocument entries.",
                    PassedDetail = null,
                    FailedDetail = "ModelDocuments will increase your model size, it is advised to check if they are required.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                    IncludeCountInTitle = true
                },

                // Audit Logs Check
                new SqlCheck
                {
                    Id = "AuditLogs",
                    Query = "SELECT Count(*) from t_snapshot",
                    PassedTitle = "No Audit entries in the model",
                    FailedTitle = "Model has {count} Audit Entires",
                    PassedDetail = null,
                    FailedDetail = "Audits are not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                    IncludeCountInTitle = true
                },

                // Journal Check
                new SqlCheck
                {
                    Id = "Journal",
                    Query = "Select Count(*) from t_document where t_document.DocType = \"JEntry\"",
                    PassedTitle = "No Journal entries in the model",
                    FailedTitle = "Model has {count} Journal Entires",
                    PassedDetail = null,
                    FailedDetail = "Journal entries are not merged by LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                    IncludeCountInTitle = true
                },

                // Auditing Enabled Check
                new SqlCheck
                {
                    Id = "AuditingEnabled",
                    Query = "SELECT Count(*) FROM t_genopt where AppliesTo =\"auditing\" and Option like \"{wildcard}enabled=1;{wildcard}\"",
                    PassedTitle = "Auditing is disabled in the model",
                    FailedTitle = "Auditing is enabled.",
                    PassedDetail = null,
                    FailedDetail = "Auditing is not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                    IncludeCountInTitle = false
                },

                // Resource Allocation Check
                new SqlCheck
                {
                    Id = "ResourceAllocation",
                    Query = "SELECT Count(*) from t_objectresource",
                    PassedTitle = "No Resource Allocation entries in the model",
                    FailedTitle = "Model has {count} Resource Allocation Entires",
                    PassedDetail = null,
                    FailedDetail = "Resource Allocations are not supported when using LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                    IncludeCountInTitle = true
                },

                // User Security Check
                new SqlCheck
                {
                    Id = "UserSecurity",
                    Query = "SELECT Count(*) from t_secpolicies where t_secpolicies.Property = 'UserSecurity' and t_secpolicies.Value = 'Enabled'",
                    PassedTitle = "User Security not enabled in the Model",
                    FailedTitle = "Model has {count} User Security Entries",
                    PassedDetail = null,
                    FailedDetail = "User Security is enabled in the Model! Can cause higher complexity with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                    IncludeCountInTitle = true
                },

                // VCS Connection Check
                new SqlCheck
                {
                    Id = "VCSConnection",
                    Query = "SELECT count(*) FROM t_package WHERE IsControlled = True",
                    PassedTitle = "VCS is not configured in the Model",
                    FailedTitle = "Model has {count} VCS enabled Packages",
                    PassedDetail = null,
                    FailedDetail = "Models with Package based VCS are not a supported scenario.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                    IncludeCountInTitle = true
                }
            };
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
        private static List<SqlCheck> TryLoadChecksFromJson()
        {
            try
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "checks-config.json");
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
                    check.PassedLevel = ParseIssueLevel(passedLevel.GetString());

                if (checkElement.TryGetProperty("failedLevel", out var failedLevel))
                    check.FailedLevel = ParseIssueLevel(failedLevel.GetString());

                if (checkElement.TryGetProperty("includeCountInTitle", out var includeCount))
                    check.IncludeCountInTitle = includeCount.GetBoolean();

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
        private static IssueLevel ParseIssueLevel(string level)
        {
            return level?.ToLower() switch
            {
                "error" => IssueLevel.Error,
                "warning" => IssueLevel.Warning,
                "information" => IssueLevel.Information,
                "passed" => IssueLevel.Passed,
                _ => IssueLevel.Information
            };
        }
    }
}
