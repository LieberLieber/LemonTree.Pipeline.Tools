using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules.Examples
{
	public class DummyRuleMinor : ISemanticVersioningRule
	{
		public string Name => "DummyRuleMinor";

		public string Description => "Dummy rule with minor change";

		public ChangeLevel Change => ChangeLevel.Minor;

        public int Sequence => 1;

        public ChangeLevel Apply(XElement classifier, ChangeLevel currentChangeLevel)
        {
			return ChangeLevel.Minor;
        }
    }
}
