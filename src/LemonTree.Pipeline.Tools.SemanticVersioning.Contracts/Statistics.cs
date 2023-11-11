using System.Collections.Generic;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Contracts
{
	public class Statistics
	{
		protected List<StatisticsItem> _appliedRules { get; set; } = new List<StatisticsItem>();

		public void Reset()
		{
			_appliedRules.Clear();
		}

		public void Add(ISemanticVersioningRule rule, ChangeLevel changeLevel, string elementGuid)
		{
			_appliedRules.Add(new StatisticsItem(rule, changeLevel, elementGuid));
		}
	}
}
