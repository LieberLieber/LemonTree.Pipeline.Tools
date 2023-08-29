using System.Data;

namespace LemonTree.Pipeline.Tools.Database
{
    public interface IEADatabase
    {
        void SetModel(string model);

        bool Compact(string source, string destination);

		/// <summary>
		/// run query
		/// </summary>
		/// <param name="sql">when using parameters, the sql query must contain placeholders</param>
		/// <param name="parameters">optional parameters</param>
		/// <returns>number affected rows</returns>
		int RunSqlNonQuery(string sql, params IEAParameter[] parameters);

		/// <summary>
		/// run query
		/// </summary>
		/// <param name="sql">when using parameters, the sql query must contain placeholders</param>
		/// <param name="parameters">optional parameters</param>
		/// <returns>value of first column in result</returns>
		object RunSqlQueryScalar(string sql, params IEAParameter[] parameters);

		/// <summary>
		/// run SQL and return result table
		/// </summary>
		/// <param name="sql">when using parameters, the sql query must contain placeholders</param>
		/// <param name="parameters">optional parameters</param>
		/// <returns>dataTable with result table or null</returns>
		DataTable RunSql(string sql, params IEAParameter[] parameters);

		/// <summary>
		/// Db File Extension. e.g. eapx, qeax
		/// </summary>
        string DbFileExtension { get; }

		/// <summary>
		/// Wildcard character used in SQL queries
		/// </summary>
        string WildcardCharacter { get; }

		/// <summary>
		/// Placeholder charcter used for DbParameters
		/// </summary>
		string ParameterPlaceholder { get; }

		/// <summary>
		/// Prefix used on escaping strings
		/// </summary>
		string EscapePrefix { get; }

		/// <summary>
		/// Postfix used on escaping string
		/// </summary>
		string EscapePostfix { get; }
	}
}
