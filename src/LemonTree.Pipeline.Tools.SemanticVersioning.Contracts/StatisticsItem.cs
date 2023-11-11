namespace LemonTree.Pipeline.Tools.SemanticVersioning.Contracts
{
	public class StatisticsItem
	{
		public ISemanticVersioningRule Rule { get; set; }
		public ChangeLevel ChangeLevel { get; set; }
		public string ElementGuid { get; set; }

		public StatisticsItem(ISemanticVersioningRule rule, ChangeLevel changeLevel, string elementGuid)
		{
			Rule = rule;
			ChangeLevel = changeLevel;
			ElementGuid = elementGuid;
		}
	}
}
