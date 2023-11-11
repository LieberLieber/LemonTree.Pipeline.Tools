using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal class DeletedSubitems : RuleBase, ISemanticVersioningRule
	{
		string ISemanticVersioningRule.Name => "DeletedSubitems";

		string ISemanticVersioningRule.Description { get; }

		ChangeLevel ISemanticVersioningRule.Change => ChangeLevel.Major;

		ChangeLevel ISemanticVersioningRule.Apply(XElement item, ChangeLevel currentChangeLevel)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

#if (DEBUG)
			string elementGuid = GetGuid(item);
			if (elementGuid == "{41F7CA6C-7DE6-48ca-87FF-EE9287C8C462}")		//ElementOfDeletedSubitems
			{
				item.ToString();
			}
#endif

			var childNodes = GetChildNodes(item);
			foreach (var childNode in childNodes)
			{
				string changedProperty = GetName(childNode);
				string diffState = GetDiffState(childNode) ?? "";

				if (diffState == DiffStates.REMOVED)
				{
					localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
					break;
				}
			}

			return localChangeLevel;
		}
	}
}
