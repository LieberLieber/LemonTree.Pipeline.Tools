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
		/// Sequence of rules, some may execute at the end only
		/// </summary>
		int Sequence { get; }

		/// <summary>
		/// Change caused by this rule
		/// </summary>
		ChangeLevel Change { get; }

		/// <summary>
		/// Applies the rule
		/// </summary>
		/// <param name="classifier">classifier node to check</param>
		/// <param name="currentChangeLevel">some rules are only to be executed for certain changeLevels</param>
		/// <returns></returns>
		ChangeLevel Apply(XElement classifier, ChangeLevel currentChangeLevel);
	}
}
