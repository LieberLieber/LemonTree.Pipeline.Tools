
namespace LemonTree.Pipeline.Tools.Database
{
	public class EAParameter : IEAParameter
	{
		public string Column { get; set; }
		public object Value { get; set; }
		public System.Data.DbType DbType { get; set; }

		public EAParameter()
		{ }
		public EAParameter(string column, object value, System.Data.DbType dbType = System.Data.DbType.String)
		{
			Column = column;
			Value = value;
			DbType = dbType;
		}
	}
}
