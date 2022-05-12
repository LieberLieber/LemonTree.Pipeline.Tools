using System.Globalization;
using System.IO;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    internal static class Checks
    {
        internal static Issue CheckDiagramImagemaps(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("Select Count(*) from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No DIAGRAMIMAGEMAP entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Information;
                result.Detail = $"This is perfect if you use it with WebEA and Prolaborate but makes merging/diffing harder";
                result.Title = $"Model has {retVal} DIAGRAMIMAGEMAPS";

            }


            return result;
        }

        internal static Issue CheckBaseline(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where t_document.DocType = 'Baseline'");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No Baseline entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Warning;
                result.Detail = $"Baselines are not helpful or required if you manage a model inside a VCS with LemonTree.";
                result.Title = $"Model has {retVal} Baselines";

            }

            return result;
        }

        internal static Issue CheckExtDoc(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where DocType = 'ExtDoc'");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No embedded binary images or document entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Warning;
                result.Detail = $"Embedded binary files will increase your model size, it is advised to check if they are required.";
                result.Title = $"Model has {retVal} embedded binary images or document entries.";

            }

            return result;
        }

        internal static Issue CheckCompact(string model)
        {
            Issue result = new Issue();

            long  modelSize = new FileInfo(model).Length;

            string tempFile = Path.GetTempFileName() +".mdb"; 

            if (ModelAccess.CompactAndRepairJetDB(model,tempFile))
            {
                long modelCompactSize = new FileInfo(tempFile).Length;
                var  ratio = (double)modelCompactSize / (double)modelSize;   

                if (ratio < 0.50)
                {
                    result.Level = IssueLevel.Error;
                }
                else if (ratio < 0.75)
                {
                    result.Level = IssueLevel.Warning;
                }
                else
                {
                    result.Level = IssueLevel.Passed;
                }

                result.Title = $"Model size before compact {Helper.ToSize(modelSize, Helper.SizeUnits.MB)} MB after {Helper.ToSize(modelCompactSize, Helper.SizeUnits.MB)} MB";
                result.Detail = $"If you run compact on the model you can reduce the size to {ratio.ToString("P")}";

            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Title = "Trying to compact the model failed.";
            }

            

            return result;
        }

        internal static Issue CheckStrippedCompact(string model)
        {
            Issue result = new Issue();

            long modelSize = new FileInfo(model).Length;

            string tempFile = Path.GetTempFileName() + ".mdb";
            File.Copy(model,tempFile,true);
            ModelAccess.ConfigureAccess("Microsoft.Jet.OLEDB.4.0", tempFile);

            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'ModelDocument'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'ExtDoc'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'BaseLine'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'DIAGRAMIMAGEMAP'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_snapshot");

            string tempFileCompact = Path.GetTempFileName() + ".mdb";

            ModelAccess.ConfigureAccess("Microsoft.Jet.OLEDB.4.0", model);

            if (ModelAccess.CompactAndRepairJetDB(tempFile, tempFileCompact))
            {
                long modelCompactSize = new FileInfo(tempFileCompact).Length;

                var ratio = (double)modelCompactSize / (double)modelSize;
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                result.Level = IssueLevel.Information;
                result.Title = $"Model size before strip and compact {Helper.ToSize(modelSize, Helper.SizeUnits.MB)} MB after {Helper.ToSize(modelCompactSize, Helper.SizeUnits.MB)} MB";
                result.Detail = $"If you strip the model from binary content and run compact on the model you can reduce the size to {ratio.ToString("P")}";
            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Title = "Trying to compact the model failed.";
            }



            return result;
        }

        internal static Issue CheckModelDocument(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where DocType = 'ModelDocument'");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No ModelDocument entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Warning;
                result.Detail = $"ModelDocuments will increase your model size, it is advised to check if they are required.";
                result.Title = $"Model has {retVal} ModelDocument entries.";

            }

            return result;
        }



        internal static Issue CheckAudit(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) from t_snapshot");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No Audit entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Detail = $"Audits are not helpful or required if you manage a model inside a VCS with LemonTree.";
                result.Title = $"Model has {retVal} Audit Entires";
            }

            return result;
        }

        internal static Issue CheckUserSecurity(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) from  t_secpolicies where t_secpolicies.Property = 'UserSecurity' and t_secpolicies.Value = 'Enabled'");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "User Security not enabled in the Model";
            }
            else
            {
                result.Level = IssueLevel.Warning;
                result.Detail = $"User Security is enabled in the Model! Can cause higher complexity with LemonTree.";
                result.Title = $"Model has {retVal} User Security Entries";

            }

            return result;
        }


        internal static Issue CheckVCSConnection(string model)
        {
            int retVal = ModelAccess.RunSQLQueryScalar("SELECT count(*) FROM t_package WHERE IsControlled = True");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "VCS is not configured in the Model";
            }
            else
            {
                result.Level = IssueLevel.Warning;
                result.Detail = $"Models with Package based VCS  are not a supported scenario.";
                result.Title = $"Model has {retVal} VCS enabled Packages";

            }


            return result;
        }
    }
}
