using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules.Examples
{
	public class DummyRulePatch : ISemanticVersioningRule
	{
		public string Name => "DummyRulePatch";

		public string Description => "Dummy rule with patch change";

		public ChangeLevel Change => ChangeLevel.Patch;

		public ChangeLevel Apply(XElement modifiedElement)
		{
			return ChangeLevel.Patch;
		}
	}
}
