using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;


namespace LemonTree.Pipeline.Tools
{
    public static class ModelAccess
    {
        private static OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();

        public static bool CompactAndRepairAccessDB(string source, string destination)
        {
            try
            {
                Console.WriteLine($"Starting compact of model.");
                var oParams = new object[]
                 {
                        source, destination
                 };

                string comName = "DAO.DBEngine.120";

                object DBE = Activator.CreateInstance(Type.GetTypeFromProgID(comName));
                
                if (DBE == null)
                {
                    Console.WriteLine($"Compact failed couldn't get the {comName} Com object");

                    comName = "DAO.DBEngine.36";
                    DBE = Activator.CreateInstance(Type.GetTypeFromProgID(comName));
                    if (DBE == null)
                    {
                        Console.WriteLine($"Compact failed couldn't get the {comName} Com object");
                        return false;
                    }
                }

                Console.WriteLine($"Running with {comName} Com object");
                DBE.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, DBE, oParams);



                // JetEngine engine = new JetEngine();
                //engine.CompactDatabase(string.Format("Data Source={0};Provider=Microsoft.Jet.OLEDB.4.0;", source),
                //string.Format("Data Source={0};Provider=Microsoft.Jet.OLEDB.4.0;", destination));
                System.Runtime.InteropServices.Marshal.ReleaseComObject(DBE);
                DBE = null;
                Console.WriteLine($"Finished compact of model.");
                return true;
            }
            catch (Exception ex)    
            {
                Debug.WriteLine($"Compact failed: {ex.Message}");
                Console.WriteLine($"Compact failed: {ex.Message}");
                return false;
            }
        }

        public static bool CompactAndRepairJetDB(string source, string destination)
        {
            try
            {
                Console.WriteLine($"Starting compact of JET model.");
                var oParams = new object[]
                 {
                        string.Format("Data Source={0};Provider=Microsoft.Jet.OLEDB.4.0;", source), string.Format("Data Source={0};Provider=Microsoft.Jet.OLEDB.4.0;", destination)
                 };

                string comName = "JRO.JetEngine";

                object DBE = Activator.CreateInstance(Type.GetTypeFromProgID(comName));

                if (DBE == null)
                {
                        Console.WriteLine($"Compact failed couldn't get the {comName} Com object");
                        return false;
                }

                Console.WriteLine($"Running with {comName} Com object");
                DBE.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, DBE, oParams);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(DBE);
                DBE = null;
                Console.WriteLine($"Finished compact of model.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Compact failed: {ex.Message}");
                Console.WriteLine($"Compact failed: {ex.Message}");
                return false;
            }
        }

        public static int RunSQLnonQuery(string sql)
        {
            int RecordCount = -1;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    RecordCount = cmd.ExecuteNonQuery();
                }
            }

            return RecordCount;
        }

        public static void ConfigureAccess(string provider, string model)
        {
            _builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            _builder.DataSource = model;
        }

        public static int RunSQLQueryScalar(string sql)
        {
            int RecordCount = 0;

            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    RecordCount = (Int32)cmd.ExecuteScalar(); 
                  
                }
            }

            return RecordCount;
        }
    }
}
