
namespace LemonTree.Pipeline.Tools.Database
{
	public interface IEAParameter
	{
		string Column { get; set; }
		object Value { get; set; }
		System.Data.DbType DbType { get; set; }
	}
}
