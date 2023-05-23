using System;
using System.Data;
using System.Data.OleDb;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class JetDatabase : IEADatabase
    {
        private readonly OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();

		/// <summary>
		/// Db File Extension. e.g. eapx
		/// </summary>
		public string DbFileExtension => ".eapx";

		/// <summary>
		/// Wildcard character used in SQL queries
		/// </summary>
		public string WildcardCharacter => "*";

		/// <summary>
		/// Placeholder charcter used for DbParameters
		/// </summary>
		public string ParameterPlaceholder => "?";

		/// <summary>
		/// Prefix used on escaping strings
		/// </summary>
		public string EscapePrefix => "[";

		/// <summary>
		/// Postfix used on escaping string
		/// </summary>
		public string EscapePostfix => "]";


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

		public int RunSqlNonQuery(string sql, params IEAParameter[] parameters)
        {
            int recordCount = -1;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
			{
				cn.Open();

				using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
					AddParameters(cmd, parameters);
					recordCount = cmd.ExecuteNonQuery();
                }
            }

            return recordCount;
        }

        public object RunSqlQueryScalar(string sql, params IEAParameter[] parameters)
        {
            object scalar = null;
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
				cn.Open();

				using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
					AddParameters(cmd, parameters);
					scalar = cmd.ExecuteScalar();
                }
            }

            return scalar;
        }


		/// <summary>
		/// run SQL and return result table
		/// on using params, the sql query must contain the correct placeholders
		/// </summary>
		/// <param name="sql">sql query to execute.</param>
		/// <param name="parameters">array of parameters</param>
		/// <returns>dataTable with result table or null</returns>
		public DataTable RunSql(string sql, params IEAParameter[] parameters)
		{
            DataTable result;

            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
				cn.Open();

				using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
					AddParameters(cmd, parameters);

                    var dataReader = cmd.ExecuteReader();
                    result = new DataTable();
                    result.Load(dataReader);
                }
            }

            return result;
        }

		private void AddParameters(OleDbCommand cmd, IEAParameter[] parameters)
		{
			if (parameters?.Length > 0)
			{
				foreach (var parameter in parameters)
				{
					var newP = cmd.CreateParameter();
					newP.ParameterName = parameter.Column;
					newP.Value = parameter.Value;

					newP.DbType = parameter.DbType;

					cmd.Parameters.Add(newP);
				}
			}
		}

		public void SetModel(string model)
        {
            _builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            _builder.DataSource = model;
        }
    }
}
