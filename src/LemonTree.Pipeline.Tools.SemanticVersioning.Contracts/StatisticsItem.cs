namespace LemonTree.Pipeline.Tools.SemanticVersioning.Contracts
{
	public class StatisticsItem
	{
		public ISemanticVersioningRule Rule { get; set; }
		public ChangeLevel ChangeLevel { get; set; }

		public StatisticsItem(ISemanticVersioningRule rule, ChangeLevel changeLevel)
		{
			Rule = rule;
			ChangeLevel = changeLevel;
		}
	}
}
