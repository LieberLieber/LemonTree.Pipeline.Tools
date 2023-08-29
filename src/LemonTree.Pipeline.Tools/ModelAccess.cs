using LemonTree.Pipeline.Tools.Database;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace LemonTree.Pipeline.Tools
{
    public static class ModelAccess
    {
        private static IEADatabase eaDatabase = null;

        public static bool Compact(string source, string destination)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.Compact(source, destination);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        public static int RunSQLnonQuery(string sql)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSQLnonQuery(sql);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        public static long RunSQLQueryScalar(string sql)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSQLQueryScalar(sql);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        public static DataTable RunSql(string sql)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSql(sql);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        public static void ConfigureAccess(string model)
        {
            if (model.EndsWith(".eap") || model.EndsWith(".eapx"))
            {
                eaDatabase = new JetDatabase();
                eaDatabase.SetModel(model);
            }
            else if (model.EndsWith(".qea") || model.EndsWith(".qeax"))
            {
                eaDatabase = new SqLiteDatabase();
                eaDatabase.SetModel(model);
            }
            else
            {
                Console.WriteLine("only .eap, .eapx, .qea and .qeax are suported");
                throw new NotSupportedException("only .eap, .eapx, .qea and .qeax are suported");
            }
        }

 
        public static string GetExtension()
        {
            if (eaDatabase != null)
            {
                return  eaDatabase.GetExtension();
            }
            else
            {
                throw new Exception("Model not set");
            }
            
        }

        public static string GetWildcard()
        {
            if (eaDatabase != null)
            {
                return  eaDatabase.GetWildcard();
            }
            else
            {
                throw new Exception("Model not set");
            }
        }
    }
}
