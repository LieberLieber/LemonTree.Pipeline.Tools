
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace LemonTree.Pipeline.Tools.ModelCheck.Checks
{
    internal static class Checks
    {
        internal static Issue CheckDiagramImagemaps(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("Select Count(*) from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");

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



        internal static Issue CheckT_image(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("Select Count(*) from t_image");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No t_image entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Information;
                result.Detail = $"Binary image data makes the model bigger!";
                result.Title = $"Model has {retVal} t_image entries";

            }


            return result;
        }

        internal static Issue CheckBaseline(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where t_document.DocType = 'Baseline'");

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
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where DocType = 'ExtDoc'");

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

            long modelSize = new FileInfo(model).Length;

            string tempFile = Path.GetTempFileName() + ModelAccess.GetExtension();

            if (ModelAccess.Compact(model, tempFile))
            {
                long modelCompactSize = new FileInfo(tempFile).Length;
                var ratio = (double)modelCompactSize / (double)modelSize;

                if (ratio < 0.50)
                {
                    result.Level = IssueLevel.Warning;
                }
                else if (ratio < 0.75)
                {
                    result.Level = IssueLevel.Information;
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

            string tempFile = Path.GetTempFileName() + ModelAccess.GetExtension();
            File.Copy(model, tempFile, true);
            ModelAccess.ConfigureAccess(tempFile);

            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'ModelDocument'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'ExtDoc'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'Baseline'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_document where DocType = 'DIAGRAMIMAGEMAP'");
            ModelAccess.RunSQLnonQuery("Delete FROM t_snapshot");
            ModelAccess.RunSQLnonQuery("Delete FROM t_image");

            string tempFileCompact = Path.GetTempFileName() + ModelAccess.GetExtension();

            ModelAccess.ConfigureAccess(model);

            if (ModelAccess.Compact(tempFile, tempFileCompact))
            {
                long modelCompactSize = new FileInfo(tempFileCompact).Length;

                var ratio = (double)modelCompactSize / (double)modelSize;
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                if (ratio < 0.75)
                {
                    result.Level = IssueLevel.Information;
                }
                else
                {
                    result.Level = IssueLevel.Passed;
                }
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
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) FROM t_document where DocType = 'ModelDocument'");

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



        internal static Issue CheckAuditLogs(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) from t_snapshot");

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

        internal static Issue CheckJournal(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("Select Count(*) from t_document where t_document.DocType = \"JEntry\"");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No Journal entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Detail = $"Journal entries are not merged by LemonTree.";
                result.Title = $"Model has {retVal} Journal Entires";
            }

            return result;
        }

        internal static Issue CheckAuditEnabled(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar($"SELECT Count(*) FROM t_genopt where AppliesTo =\"auditing\" and Option like \"{ModelAccess.GetWildcard()}enabled=1;{ModelAccess.GetWildcard()}\"");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "Auditing is disabled in the model";
            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Detail = $"Auditing is not helpful or required if you manage a model inside a VCS with LemonTree.";
                result.Title = $"Auditing is enabled.";
            }

            return result;
        }


        internal static Issue CheckResourceAllocation(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) from t_objectresource");

            Issue result = new Issue();
            if (retVal == 0)
            {
                result.Level = IssueLevel.Passed;
                result.Title = "No Resource Allocation entries in the model";
            }
            else
            {
                result.Level = IssueLevel.Error;
                result.Detail = $"Resource Allocations are not supported when using LemonTree.";
                result.Title = $"Model has {retVal} Resource Allocation Entires";
            }

            return result;
        }

        internal static Issue CheckUserSecurity(string model)
        {
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT Count(*) from  t_secpolicies where t_secpolicies.Property = 'UserSecurity' and t_secpolicies.Value = 'Enabled'");

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
            long retVal = ModelAccess.RunSQLQueryScalar("SELECT count(*) FROM t_package WHERE IsControlled = True");

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


        /// <summary>
        /// get EA's project statistic view
        /// </summary>
        /// <param name="model"></param>

        /// <returns></returns>

        internal static Issue CheckProjectStatitics(string model)
        {
            #region get result table

            const string statisticSql = @"select count(t_package.Package_ID) as ['Count'], 'Total Packages' as Measure from t_package
                                            UNION
                                            select count(t_package.Package_ID) as ['Count'], 'Total Root Packages' as Measure from t_package where t_package.Parent_ID = 0
                                            UNION
                                            select count(t_diagram.Diagram_ID) as ['Count''], 'Total Diagrams' as Measure from t_diagram
                                            UNION
                                            select count(t_object.Object_ID) as ['Count'], 'Total Elements' as Measure from t_object
                                            UNION
                                            select count(t_connector.Connector_ID) as ['Count'], 'Total Connectors' as Measure from t_connector
                                            UNION
                                            select count(t_diagramobjects.Instance_ID) as ['Count'], 'Elements in Diagrams' as Measure from t_diagramobjects
                                            UNION
                                            select count(t_attribute.ID) as ['Count'], 'Attributes in Elements' as Measure from t_attribute
                                            UNION
                                            select count(t_operation.OperationID) as ['Count'], 'Opertions in Elements' as Measure from t_operation
                                            UNION
                                            select count(t_operationparams.ea_guid) as ['Count'], 'Parameters in Operations' as Measure from t_operationparams
                                            UNION
                                            select count(t_objecttests.Test) as ['Count'], 'Element Testing' as Measure from t_objecttests
                                            UNION
                                            select count(t_objectconstraint.ConstraintType) as ['Count'], 'Constraints on Elements' as Measure from t_objectconstraint
                                            UNION
                                            select count(t_objecteffort.Effort) as ['Count'], 'Efforts on Elements' as Measure from t_objecteffort
                                            UNION
                                            select count(t_objectfiles.FileName) as ['Count'], 'File on Elements' as Measure from t_objectfiles
                                            UNION
                                            select count(t_objectmetrics.Metric) as ['Count'], 'Metrics on Elements' as Measure from t_objectmetrics
                                            UNION
                                            select count(t_objectrequires.ReqID) as ['Count'], 'Requirements in Elements' as Measure from t_objectrequires
                                            UNION
                                            select count(t_objectresource.Resource) as ['Count'], 'Resources Allocated to Elements' as Measure from t_objectresource
                                            UNION
                                            select count(t_objectrisks.Risk) as ['Count'], 'Risks on Elements' as Measure from t_objectrisks
                                            UNION
                                            select count(t_object.Object_Type) as ['Count'], t_object.Object_Type as ['Measure'] from t_object
                                            Group by t_object.Object_Type";




            var resultTable = ModelAccess.RunSql(statisticSql);
            resultTable.DefaultView.Sort = "Measure";


            Debug.WriteLine(ToMD(resultTable, header: true));

            #endregion


            #region process result table and calculate Issue number




            #endregion

            #region set Issue Level

            Issue result = new Issue();

            result.Level = IssueLevel.Information;
            result.Title = "Project Statistics";
            result.Detail = "Executed Project Statistics on Model"; 


            result.Markdown = ToMD(resultTable.DefaultView.ToTable(), header: true);


            #endregion

            return result;
        }

        internal static Issue CheckTableSize(string model)
        {
            #region get result table

            const string statisticSql = @"SELECT name ,SUM(pgsize)/1024 table_size  FROM 'dbstat' GROUP BY name HAVING table_size > 32 ORDER BY table_size desc ; ";

            var resultTable = ModelAccess.RunSql(statisticSql);
            //resultTable.DefaultView.Sort = "table_size";
            resultTable.Columns[1].ColumnName = "table_size (bytes)";

            #endregion

            #region process result table and calculate Issue number


            #endregion

            #region set Issue Level

            Issue result = new Issue();

            result.Level = IssueLevel.Information;
            result.Title = "Table Statistics (all >32)";
            result.Detail = $"Found {resultTable.Rows.Count} Tables bigger 32";


            result.Markdown = ToMD(resultTable.DefaultView.ToTable(), header: true);


            #endregion

            return result;
        }

        internal static Issue CheckModelOrphans(string model)
        {
            #region get result table

            const string statisticSql = @"
            SELECT ea_guid AS CLASSGUID, Object_Type AS CLASSTYPE, t_object.* 
            FROM t_object 
            WHERE t_object.Object_Type <> 'Package'  
            AND t_object.Object_ID NOT IN (SELECT t_diagramobjects.Object_ID FROM t_diagramobjects)  
            AND t_object.Object_ID NOT IN (SELECT t_object.Classifier FROM t_object WHERE t_object.Classifier <> 0) 
            AND t_object.Object_ID NOT IN (SELECT a.Object_ID FROM t_object AS a JOIN t_object AS b ON b.PDATA1 = a.ea_guid) 
            AND t_object.Object_ID NOT IN (SELECT CAST(t_attribute.Classifier AS INTEGER) FROM t_attribute WHERE t_attribute.Classifier <> '0' AND t_attribute.Classifier <> '') 
            AND t_object.Object_ID NOT IN (SELECT CAST(t_operation.Classifier AS INTEGER) FROM t_operation WHERE t_operation.Classifier <> '0' AND t_operation.Classifier <> '') 
            AND t_object.Object_ID NOT IN (SELECT CAST(t_operationparams.Classifier AS INTEGER) FROM t_operationparams WHERE t_operationparams.Classifier <> '0' AND t_operationparams.Classifier <> '') 
            AND t_object.Object_ID NOT IN (SELECT t_connector.Start_Object_ID FROM t_connector) 
            AND t_object.Object_ID NOT IN (SELECT t_connector.End_Object_ID FROM t_connector) 
            AND t_object.Object_ID NOT IN (SELECT t_object.ParentID FROM t_object) 
            AND t_object.Object_ID NOT IN (SELECT t_object.Object_ID  FROM t_xref  JOIN t_object ON t_xref.Description LIKE '%' || t_object.ea_guid || '%' WHERE t_xref.Name = 'MOFProps') 
            AND t_object.ea_guid NOT IN (SELECT t_operation.Behaviour FROM t_operation WHERE t_operation.Behaviour <> '') 
            AND t_object.Object_ID NOT IN (SELECT CAST(t_connector.PDATA1 AS INTEGER)  FROM t_connector  WHERE t_connector.Connector_Type = 'Association' AND t_connector.SubType = 'Class');
            ";

            var resultTable = ModelAccess.RunSql(statisticSql);

            #endregion

            #region process result table and calculate Issue number


            #endregion

            #region set Issue Level

            Issue result = new Issue();

            result.Level = IssueLevel.Information;
            result.Title = "Model Orphans Statistics";


            if (resultTable.Rows.Count > 0)
            {
                result.Markdown = ToMD(resultTable.DefaultView.ToTable(), header: true);
                result.Detail = $"Found {resultTable.Rows.Count} Orphans in Model";
            }
            else
            {
                result.Detail = "No Orphans found in Model!";
            }


            #endregion

            return result;
        }
        private static string ToMD(DataTable t, bool header)
        {

            //dd syntay might be better down the road then <BR>
            //|< dl >< dt > Beast of Bodmin</ dt >< dd > A large feline inhabiting Bodmin Moor.</ dd >< dt > Morgawr </ dt >< dd > A sea serpent</ dd >< dt > Owlman </ dt >< dd > A giant owl-like creature.</ dd ></ dl
            StringBuilder sb = new StringBuilder();
            int maxColIdx = 0;

            if (t?.Columns?.Count > 0)
            {
                maxColIdx = t.Columns.Count - 1;
                if (header)
                {
                    foreach (DataColumn c in t.Columns)
                    {

                        sb.Append("|");
                        sb.Append(c.ColumnName.Replace("'", ""));

                    }
                    sb.Append("|");
                    sb.Append(Environment.NewLine);
                    foreach (DataColumn c in t.Columns)
                    {
                        sb.Append("|");
                        sb.Append("-------");

                    }
                    sb.Append("|");
                    sb.Append(Environment.NewLine);

                }

                if (t?.Rows?.Count > 0)
                {
                    foreach (DataRow r in t.Rows)
                    {
                        foreach (var item in r.ItemArray)
                        {

                            sb.Append("|");
                            sb.Append(item);

                        }
                        sb.Append("|");
                        sb.Append(Environment.NewLine);

                    }
                }
            }
            return sb.ToString();

        }

    }
}

