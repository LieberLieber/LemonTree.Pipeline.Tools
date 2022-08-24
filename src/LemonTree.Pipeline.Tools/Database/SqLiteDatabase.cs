﻿using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace LemonTree.Pipeline.Tools.Database
{
    internal class SqLiteDatabase : IEADatabase
    {
        private static SQLiteConnectionStringBuilder _builder = new SQLiteConnectionStringBuilder();

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
        }
    }
}
