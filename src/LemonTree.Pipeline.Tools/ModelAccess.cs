using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonTree.Pipeline.Tools
{
    public static class ModelAccess
    {
        private static OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();

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
    }
}
