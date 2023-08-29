using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules.Examples
{
	public class DummyRuleMajor : ISemanticVersioningRule
	{
		public string Name => "DummyRuleMajor";

		public string Description => "Dummy rule with major change";

		public ChangeLevel Change => ChangeLevel.Major;

		public ChangeLevel Apply(XElement modifiedElement)
		{
			return ChangeLevel.Major;
		}
	}
}
