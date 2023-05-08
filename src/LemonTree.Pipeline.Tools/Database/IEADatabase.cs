using System.Data;

namespace LemonTree.Pipeline.Tools.Database
{
    public interface IEADatabase
    {
        void SetModel(string model);

        bool Compact(string source, string destination);

        int RunSqlNonQuery(string sql);

        object RunSqlQueryScalar(string sql);

        /// <summary>
        /// run SQL and return result table
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>dataTable with result table or null</returns>
        DataTable RunSql(string sql);

        string GetExtension();
        string GetWildcard();
    }
}
