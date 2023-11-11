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
			if ((elementGuid == "{301253EB-D53D-4460-8E4A-F1857D4DD357}")       //ElementOfNewSubitems
				|| (elementGuid == "{50E48D93-6512-4627-AC5A-9DBCB1FFD792}"))   //MyPackage3_Alias
			{
				item.ToString();
			}
#endif

			XElement elementNode = GetElementNode(item);
			if (null != elementNode)
			{
				string elementState = GetDiffState(elementNode);
				if (elementState.Equals(DiffStates.SUB_ELEMENT_MODIFIED) 
					|| elementState.Equals(DiffStates.MODIFIED))
				{
					var childNodes = GetChildNodes(item);
					foreach (var childNode in childNodes)
					{
						string changedProperty = GetName(childNode);
						string diffState = GetDiffState(childNode) ?? "";

						//we need to find the highest ranked change off all the attributes that have changed.
						if (diffState == DiffStates.NEW)
						{
							localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
							break;
						}
					}
				}
			}

			return localChangeLevel;
		}
	}
}
