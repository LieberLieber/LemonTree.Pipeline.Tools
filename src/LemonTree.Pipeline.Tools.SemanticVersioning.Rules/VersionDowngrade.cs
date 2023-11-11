using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal class VersionDowngrade : RuleBase, ISemanticVersioningRule
	{
		public string Name => "VersionDowngrade";

		public string Description { get; }

		public override int Sequence => 99;

		public ChangeLevel Change => ChangeLevel.Major;

		public ChangeLevel Apply(XElement item, ChangeLevel currentChangeLevel)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

#if (DEBUG)
			string elementGuid = GetGuid(item);
			if (elementGuid == "{01D18B2D-4409-48ef-B14D-0C53DA9C4002}")    //MyPackage1_DowngradeTo200
			{
				item.ToString();
			}
#endif

			if (currentChangeLevel == ChangeLevel.None)
			{
				var elementNode = GetElementNode(item);
				if (null != elementNode)
				{
					var changedProperties = elementNode.Elements().Elements();
					if (changedProperties?.Count() == 1)
					{
						var propertyName = GetName(changedProperties.First());
						if (propertyName.Equals("EA Specifics 1.0::Version"))
						{
							localChangeLevel = ChangeLevel.Downgrade;
						}
					}
				}
			}

			return localChangeLevel;
		}
	}
}
