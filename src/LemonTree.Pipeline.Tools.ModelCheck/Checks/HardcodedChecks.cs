using System.Collections.Generic;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    /// <summary>
    /// Defines the hardcoded default checks for LemonTree ModelCheck
    /// </summary>
    internal static class HardcodedChecks
    {
        /// <summary>
        /// Get the hardcoded default checks
        /// </summary>
        internal static List<SqlCheck> GetHardcodedChecks()
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
    }
}
