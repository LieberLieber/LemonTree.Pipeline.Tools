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

        public static int RunSQLnonQuery(string sql, params IEAParameter[] parameters)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSqlNonQuery(sql, parameters);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        public static object RunSQLQueryScalar(string sql, params IEAParameter[] parameters)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSqlQueryScalar(sql, parameters);
            }
            else
            {
                throw new Exception("Model not set");
            }
        }

        public static long RunSQLQueryScalarAsLong(string sql, params IEAParameter[] parameters)
        {
            return Convert.ToInt64(RunSQLQueryScalar(sql, parameters));            
        }

        public static string RunSQLQueryScalarAsString(string sql, params IEAParameter[] parameters)
        {
            return Convert.ToString(RunSQLQueryScalar(sql, parameters));
        }

        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        public static DataTable RunSql(string sql, params IEAParameter[] parameters)
        {
            if (eaDatabase != null)
            {
                return eaDatabase.RunSql(sql, parameters);
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
			if (null == eaDatabase)
			{
				throw new Exception("Model not set");
			}
            
			return eaDatabase.DbFileExtension;
        }

        public static string WildcardCharacter()
        {
			if (null == eaDatabase)
			{
				throw new Exception("Model not set");
			}
			
			return eaDatabase.WildcardCharacter;
        }

		public static string ParameterPlaceholder()
		{
			if (null == eaDatabase)
			{
				throw new Exception("Model not set");
			}
			return eaDatabase.ParameterPlaceholder;
		}
	}
}
