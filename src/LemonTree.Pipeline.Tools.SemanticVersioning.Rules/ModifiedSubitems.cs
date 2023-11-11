using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal class ModifiedSubitems : RuleBase, ISemanticVersioningRule
	{
		string ISemanticVersioningRule.Name => "ModifiedSubitems";

		string ISemanticVersioningRule.Description { get; }

		ChangeLevel ISemanticVersioningRule.Change => ChangeLevel.Major;

		ChangeLevel ISemanticVersioningRule.Apply(XElement item, ChangeLevel currentChangeLevel)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

#if (DEBUG)
			string elementGuid = GetGuid(item);
			if (elementGuid == "{F149650A-C3DA-431c-88C6-A6DC24B24768}")    //ElementOfRenamedSubitems
			{
				item.ToString();
			}
#endif

			var childNodes = GetChildNodes(item);
			foreach (var childNode in childNodes)
			{
				string changedProperty = GetName(childNode);
				string diffState = GetDiffState(childNode) ?? "";

				if (diffState == DiffStates.MODIFIED)
				{
					localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
					break;
				}
			}

			return localChangeLevel;
		}
	}
}
