using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal class NewSubitems : RuleBase, ISemanticVersioningRule
	{
		string ISemanticVersioningRule.Name => "NewSubitems";

		string ISemanticVersioningRule.Description { get; }

		ChangeLevel ISemanticVersioningRule.Change => ChangeLevel.Major;

		ChangeLevel ISemanticVersioningRule.Apply(XElement item, ChangeLevel currentChangeLevel)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

#if(DEBUG)
			string elementGuid = GetGuid(item);
			if (elementGuid == "{5605CA7D-2EFB-4a60-9463-A1502FE7EE7B}")       //RM___Risikomanagement (new diagram)
			{
				item.ToString();
			}
#endif

			var childNodes = GetChildNodes(item);
			foreach (var childNode in childNodes)
			{
				string changedProperty = GetName(childNode);
				string diffState = GetDiffState(childNode) ?? "";

				if (diffState == DiffStates.NEW)
				{
					localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
					break;
				}
			}

			return localChangeLevel;
		}
	}
}
