using System;
using System.Data.OleDb;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class JetDatabase : IEADatabase
    {
        private  OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();
        public bool Compact(string source, string destination)
        {
            try
            {
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


                DBE.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, DBE, oParams);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(DBE);
                DBE = null;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Compact failed: {ex.Message}");
                return false;
            }
        }

        public string GetExtension()
        {
            return ".eapx";
        }

        public string GetWildcard()
        {
            return "*";
        }

        public int RunSQLnonQuery(string sql)
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

        public long RunSQLQueryScalar(string sql)
        {
            long RecordCount = 0;

            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    RecordCount = Convert.ToInt64(cmd.ExecuteScalar());

                }
            }

            return RecordCount;
        }

        public void SetModel(string model)
        {
            _builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            _builder.DataSource = model;
        }
    }
}
