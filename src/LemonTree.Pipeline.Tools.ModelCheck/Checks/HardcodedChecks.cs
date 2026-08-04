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
                    QueryOnFail = "Select * from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP'",
                    PassedTitle = "No DIAGRAMIMAGEMAP entries in the model",
                    FailedTitle = "Model has {count} DIAGRAMIMAGEMAPS",
                    PassedDetail = null,
                    FailedDetail = "This is perfect if you use it with WebEA and Prolaborate but makes merging/diffing harder",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Information,
                },

                // T_Image Check
                new SqlCheck
                {
                    Id = "TImages",
                    Query = "Select Count(*) from t_image",
                    QueryOnFail = "Select * from t_image",
                    PassedTitle = "No t_image entries in the model",
                    FailedTitle = "Model has {count} t_image entries",
                    PassedDetail = null,
                    FailedDetail = "Binary image data makes the model bigger!",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Information,
                },

                // Baseline Check
                new SqlCheck
                {
                    Id = "Baselines",
                    Query = "SELECT Count(*) FROM t_document where t_document.DocType = 'Baseline'",
                    QueryOnFail = "SELECT * FROM t_document where t_document.DocType = 'Baseline'",
                    PassedTitle = "No Baseline entries in the model",
                    FailedTitle = "Model has {count} Baselines",
                    PassedDetail = null,
                    FailedDetail = "Baselines are not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // ExtDoc Check
                new SqlCheck
                {
                    Id = "ExtDoc",
                    Query = "SELECT Count(*) FROM t_document where DocType = 'ExtDoc'",
                    QueryOnFail = "SELECT * FROM t_document where DocType = 'ExtDoc'",
                    PassedTitle = "No embedded binary images or document entries in the model",
                    FailedTitle = "Model has {count} embedded binary images or document entries.",
                    PassedDetail = null,
                    FailedDetail = "Embedded binary files will increase your model size, it is advised to check if they are required.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // ModelDocument Check
                new SqlCheck
                {
                    Id = "ModelDocuments",
                    Query = "SELECT Count(*) FROM t_document where DocType = 'ModelDocument'",
                    QueryOnFail = "SELECT * FROM t_document where DocType = 'ModelDocument'",
                    PassedTitle = "No ModelDocument entries in the model",
                    FailedTitle = "Model has {count} ModelDocument entries.",
                    PassedDetail = null,
                    FailedDetail = "ModelDocuments will increase your model size, it is advised to check if they are required.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // Audit Logs Check
                new SqlCheck
                {
                    Id = "AuditLogs",
                    Query = "SELECT Count(*) from t_snapshot",
                    QueryOnFail = "SELECT * FROM t_snapshot",
                    PassedTitle = "No Audit entries in the model",
                    FailedTitle = "Model has {count} Audit Entries",
                    PassedDetail = null,
                    FailedDetail = "Audits are not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                },

                // Journal Check
                new SqlCheck
                {
                    Id = "Journal",
                    Query = "Select Count(*) from t_document where t_document.DocType = \"JEntry\"",
                    QueryOnFail = "Select * from t_document where t_document.DocType = \"JEntry\"",
                    PassedTitle = "No Journal entries in the model",
                    FailedTitle = "Model has {count} Journal Entries",
                    PassedDetail = null,
                    FailedDetail = "Journal entries are not merged by LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                },

                // Auditing Enabled Check
                new SqlCheck
                {
                    Id = "AuditingEnabled",
                    Query = "SELECT Count(*) FROM t_genopt where AppliesTo =\"auditing\" and Option like \"{wildcard}enabled=1;{wildcard}\"",
                    QueryOnFail = "SELECT * FROM t_genopt where AppliesTo =\"auditing\"",
                    PassedTitle = "Auditing is disabled in the model",
                    FailedTitle = "Auditing is enabled.",
                    PassedDetail = null,
                    FailedDetail = "Auditing is not helpful or required if you manage a model inside a VCS with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                },

                // Resource Allocation Check
                new SqlCheck
                {
                    Id = "ResourceAllocation",
                    Query = "SELECT Count(*) from t_objectresource",
                    QueryOnFail = "SELECT * FROM t_objectresource",
                    PassedTitle = "No Resource Allocation entries in the model",
                    FailedTitle = "Model has {count} Resource Allocation Entries",
                    PassedDetail = null,
                    FailedDetail = "Resource Allocations are not supported when using LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                },

                // User Security Check
                new SqlCheck
                {
                    Id = "UserSecurity",
                    Query = "SELECT Count(*) from t_secpolicies where t_secpolicies.Property = 'UserSecurity' and t_secpolicies.Value = 'Enabled'",
                    QueryOnFail = "SELECT * FROM t_secpolicies where t_secpolicies.Property = 'UserSecurity'",
                    PassedTitle = "User Security not enabled in the Model",
                    FailedTitle = "Model has {count} User Security Entries",
                    PassedDetail = null,
                    FailedDetail = "User Security is enabled in the Model! Can cause higher complexity with LemonTree.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // VCS Connection Check
                new SqlCheck
                {
                    Id = "VCSConnection",
                    Query = "SELECT count(*) FROM t_package WHERE IsControlled = True",
                    QueryOnFail = "SELECT * FROM t_package WHERE IsControlled = True",
                    PassedTitle = "VCS is not configured in the Model",
                    FailedTitle = "Model has {count} VCS enabled Packages",
                    PassedDetail = null,
                    FailedDetail = "Models with Package based VCS are not a supported scenario.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // Unnamed Elements Check
                new SqlCheck
                {
                    Id = "UnnamedElements",
                    Query = "SELECT Count(*) FROM t_object WHERE (Name IS NULL OR Name = '') AND Object_Type NOT IN ('Note', 'Text')",
                    QueryOnFail = "SELECT Object_ID, Object_Type, Stereotype, Package_ID FROM t_object WHERE (Name IS NULL OR Name = '') AND Object_Type NOT IN ('Note', 'Text')",
                    PassedTitle = "All elements have names",
                    FailedTitle = "Model has {count} elements without a name",
                    PassedDetail = null,
                    FailedDetail = "Elements without a name are hard to identify and may cause issues in diagrams and reports.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Warning,
                },

                // Suspected Traceability Links Check
                new SqlCheck
                {
                    Id = "SuspectedTraceabilityLinks",
                    Query = "SELECT Count(*) FROM t_xref WHERE Description LIKE '%suspected%'",
                    QueryOnFail = "SELECT obj.ea_guid as CLASSGUID, obj.object_type as CLASSTYPE, obj.name as TargetName_of_SuspectedLink FROM t_object obj, t_connector conn WHERE obj.object_id = conn.start_object_id AND conn.ea_guid IN (SELECT client FROM t_xref WHERE t_xref.Description LIKE '%suspected%')",
                    PassedTitle = "There are no suspected traceability links",
                    FailedTitle = "Model has {count} suspected traceability links",
                    PassedDetail = null,
                    FailedDetail = "Some requirements have suspected traceability links. Please review and confirm the traceability links.",
                    PassedLevel = IssueLevel.Passed,
                    FailedLevel = IssueLevel.Error,
                }
            };
        }
    }
}
