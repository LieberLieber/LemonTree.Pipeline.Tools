using System;
using System.Data;
using System.Data.OleDb;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class JetDatabase : IEADatabase
    {
        private readonly OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();
        public bool Compact(string source, string destination)
        {
            try
            {
                var oParams = new object[]
                 {
	                 $"Data Source={source};Provider=Microsoft.Jet.OLEDB.4.0;",
	                 $"Data Source={destination};Provider=Microsoft.Jet.OLEDB.4.0;"
                 };

                string comName = "JRO.JetEngine";

                object dbe = Activator.CreateInstance(Type.GetTypeFromProgID(comName));

                if (dbe == null)
                {
                    Console.WriteLine($"Compact failed couldn't get the {comName} Com object");
                    return false;
                }


                dbe.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, dbe, oParams);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(dbe);
                dbe = null;

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

        public int RunSqlNonQuery(string sql)
        {
            int recordCount = -1;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    recordCount = cmd.ExecuteNonQuery();
                }
            }

            return recordCount;
        }

        public object RunSqlQueryScalar(string sql)
        {
            object scalar = null;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    scalar = cmd.ExecuteScalar();
                }
            }

            return scalar;
        }


        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        public DataTable RunSql(string sql)
        {
            DataTable result;

            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    var dataReader = cmd.ExecuteReader();
                    result = new DataTable();
                    result.Load(dataReader);
                }
            }

            return result;
        }

        public void SetModel(string model)
        {
            _builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            _builder.DataSource = model;
        }
    }
}
