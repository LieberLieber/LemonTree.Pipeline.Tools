using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class SqLiteDatabase : IEADatabase
    {
        private static SQLiteConnectionStringBuilder _builder = new SQLiteConnectionStringBuilder();
        private bool _allowWrite = false;

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
            
            // Temporarily allow write access for compact operation
            bool previousAllowWrite = _allowWrite;
            _allowWrite = true;
            
            try
            {
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
            finally
            {
                _allowWrite = previousAllowWrite;
            }
        }

        public string GetExtension()
        {
            return ".qeax";
        }

        public string GetWildcard()
        {
            return "%";
        }

        public int RunSQLnonQuery(string sql)
        {
            int RecordCount = -1;
            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
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

            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    RecordCount = (long)cmd.ExecuteScalar();

                }
            }

            return RecordCount;
        }


        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        public DataTable RunSql(string sql)
        {
            DataTable result;

            using (var cn = new SQLiteConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new SQLiteCommand { CommandText = sql, Connection = cn })
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
            // Set ReadOnly to true unless write access is explicitly allowed (e.g., for compact operations)
            _builder.ReadOnly = !_allowWrite;
        }

        public void SetModelWithWriteAccess(string model)
        {
            // NOTE: This method modifies instance state and is not thread-safe.
            // For command-line tools that run one operation at a time, this is acceptable.
            // If concurrent access is needed, consider using a thread-local flag or synchronization.
            _allowWrite = true;
            try
            {
                SetModel(model);
            }
            finally
            {
                _allowWrite = false;
            }
        }
    }
}
