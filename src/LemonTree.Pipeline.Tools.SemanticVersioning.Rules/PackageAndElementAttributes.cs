using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	public class PackageAndElementAttributes : RuleBase, ISemanticVersioningRule
	{
		public string Name => "PackageAndElementAttributes";

		public string Description { get; }

		public ChangeLevel Change => ChangeLevel.Major;
		
		public ChangeLevel Apply(XElement item, ChangeLevel currentChangeLevel)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

			string elementGuid = GetGuid(item);

#if (DEBUG)
			if (elementGuid == "{50E48D93-6512-4627-AC5A-9DBCB1FFD792}")    //MyPackage3_Alias
			{
				item.ToString();
			}
#endif

			var elementNode = GetElementNode(item);
			if (null != elementNode)
			{
				var diffState = GetDiffState(elementNode);

				if (diffState == DiffStates.MODIFIED)
				{
					var changedProperties = elementNode.Elements().Elements();

					foreach (var changedProperty in changedProperties)
					{
						string propertyName = GetName(changedProperty);

						switch (propertyName)
						{
							case "Name":
								localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
								break;
							case "EA Specifics 1.0::Version":
								//do nothing, this is covered in rule VersionDowngrade
								break;

							default:
								localChangeLevel = SetChangeLevel(ChangeLevel.Minor, localChangeLevel);
								break;
						}
					}
				}
			}

			return localChangeLevel;
		}

	}
}