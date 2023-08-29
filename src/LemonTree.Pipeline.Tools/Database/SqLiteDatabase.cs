using System.Data;
using System.Data.SQLite;
using System.IO;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class SqLiteDatabase : IEADatabase
    {
        private static readonly SQLiteConnectionStringBuilder _builder = new SQLiteConnectionStringBuilder();

		/// <summary>
		/// Db File Extension. e.g. qeax
		/// </summary>
		public string DbFileExtension => ".qeax";

		/// <summary>
		/// Wildcard character used in SQL queries
		/// </summary>
		public string WildcardCharacter => "%";

		/// <summary>
		/// Placeholder charcter used for DbParameters
		/// </summary>
		public string ParameterPlaceholder => "?";

		/// <summary>
		/// Prefix used on escaping strings
		/// </summary>
		public string EscapePrefix => "\"";

		/// <summary>
		/// Postfix used on escaping string
		/// </summary>
		public string EscapePostfix => "\"";

		public SqLiteDatabase()
        {
            //string sqllitefile = "sqllite.dll";
            //Assembly currentAssembly = Assembly.GetExecutingAssembly();

            //string output = Path.Combine(Path.GetDirectoryName(currentAssembly.Location), sqllitefile);

            //using (FileStream fs = File.OpenWrite(sqllitefile))
            //{
            //    using (Stream resourceStream = currentAssembly.GetManifestResourceStream(sqllitefile))
            //    {
            //        const int size = 4096;
            //        byte[] bytes = new byte[4096];
            //        int numBytes;
            //        while ((numBytes = resourceStream.Read(bytes, 0, size)) > 0)
            //        {
            //            fs.Write(bytes, 0, numBytes);
            //        }
            //        fs.Flush();
            //        fs.Close();
            //        resourceStream.Close();
            //    }
            //}
        }

        public bool Compact(string source, string destination)
        {
            File.Copy(source, destination, true);
            SetModel(destination);
            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
                using (SQLiteCommand command = cn.CreateCommand())
                {
                    cn.Open();
                    command.CommandText = "vacuum;";
                    command.ExecuteNonQuery();
                }
            }
            SetModel(source);
            return true;
        }

        
		public int RunSqlNonQuery(string sql, params IEAParameter[] parameters)
        {
            int recordCount = -1;
            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
				cn.Open();

				using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
                {
					AddParameters(cmd, parameters);
					recordCount = cmd.ExecuteNonQuery();
                }
            }

            return recordCount;
        }

        public object RunSqlQueryScalar(string sql, params IEAParameter[] parameters)
        {
            object scalar = 0;

            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
				cn.Open();

				using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
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

            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
				cn.Open();

				using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
                {
					AddParameters(cmd, parameters);

					var dataReader = cmd.ExecuteReader();
                    result = new DataTable();
                    result.Load(dataReader);
                }
            }

            return result;
        }

		private void AddParameters(SQLiteCommand cmd, IEAParameter[] parameters)
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
            //string sqllitefile = "sqllite.dll";
            //Assembly currentAssembly = Assembly.GetExecutingAssembly();

            //string output = Path.Combine(Path.GetDirectoryName(currentAssembly.Location), sqllitefile);

            //using (FileStream fs = File.OpenWrite(sqllitefile))
            //{
            //    using (Stream resourceStream = currentAssembly.GetManifestResourceStream(sqllitefile))
            //    {
            //        const int size = 4096;
            //        byte[] bytes = new byte[4096];
            //        int numBytes;
            //        while ((numBytes = resourceStream.Read(bytes, 0, size)) > 0)
            //        {
            //            fs.Write(bytes, 0, numBytes);
            //        }
            //        fs.Flush();
            //        fs.Close();
            //        resourceStream.Close();
            //    }
            //}

            _builder.DataSource = model;
        }
    }
}
