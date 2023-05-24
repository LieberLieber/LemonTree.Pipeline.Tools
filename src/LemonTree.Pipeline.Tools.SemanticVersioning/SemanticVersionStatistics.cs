
using System.Collections.Generic;
using System.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Contracts
{
	public class SemanticVersionStatistics: Statistics
	{
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
