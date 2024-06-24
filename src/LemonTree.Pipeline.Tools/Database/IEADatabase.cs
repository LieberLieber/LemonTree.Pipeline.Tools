using System.Data;

namespace LemonTree.Pipeline.Tools.Database
{
    public interface IEADatabase
    {
        void SetModel(string model); //mm

        bool Compact(string source, string destination);

        int RunSQLnonQuery(string sql);

        int RunSQLQueryScalar(string sql);

        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        DataTable RunSql(string sqlQuery);

        string GetExtension();
        string GetWildcard();
    }
}
