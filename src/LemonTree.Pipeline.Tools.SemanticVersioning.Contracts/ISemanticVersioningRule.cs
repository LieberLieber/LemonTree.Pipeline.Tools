using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Contracts
{
	public interface ISemanticVersioningRule
	{
		/// <summary>
		/// Name of the rule
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Description of the rule
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Change caused by this rule
		/// </summary>
		ChangeLevel Change { get; }
		
		/// <summary>
		/// Applies the rule
		/// </summary>
		ChangeLevel Apply(XElement modifiedElement);
	}
}
