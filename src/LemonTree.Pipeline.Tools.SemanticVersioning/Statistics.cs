using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning
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


	public class Statistics
	{
		private List<StatisticsItem> _appliedRules { get; set; } = new List<StatisticsItem>();

		public void Reset()
		{
			_appliedRules.Clear();
		}

		public void Update(ISemanticVersioningRule rule, ChangeLevel changeLevel)
		{
			_appliedRules.Add(new StatisticsItem(rule, changeLevel));
		}

		public Dictionary<string, int> NumberOfAppliedByRuleName()
		{
			var statistics = new Dictionary<string, int>();

			var rulesWithChangeLevel = _appliedRules.Where(r => r.ChangeLevel != ChangeLevel.None).ToList();
			var ruleNames = rulesWithChangeLevel.Select(r => r.Rule.Name).Distinct();
			foreach (string ruleName in ruleNames)
			{
				var numberApplied = _appliedRules.Count(i => i.Rule.Name.Equals(ruleName));
				statistics.Add(ruleName, numberApplied);
			}

			return statistics;
		}

		public Dictionary<string, int> NumberOfAppliedByChangeLevel()
		{
			var statistics = new Dictionary<string, int>();

			var rulesWithChangeLevel = _appliedRules.Where(r => r.ChangeLevel != ChangeLevel.None).ToList();
			var changeLevels = rulesWithChangeLevel.Select(r => r.ChangeLevel).Distinct();

			foreach (ChangeLevel level in changeLevels)
			{
				var numberApplied = _appliedRules.Count(i => i.ChangeLevel == level);
				statistics.Add(level.ToString(), numberApplied);
			}

			return statistics;
		}
	}
}
