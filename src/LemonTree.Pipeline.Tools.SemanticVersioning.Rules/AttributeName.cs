using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	public class AttributeName : ISemanticVersioningRule
	{
		public string Name => "AttributeName";

		public string Description { get; }

		public ChangeLevel Change => ChangeLevel.Major;
		
		public ChangeLevel Apply(XElement modifiedElement)
		{
			ChangeLevel localChangeLevel = ChangeLevel.None;

			bool versionHasChanged = false;
			bool otherPropertyHasChanged = false;

			var changedProperties = modifiedElement.Elements().Elements();
			foreach (var element in changedProperties)
			{
				string changedProperty = element.Attribute("name").Value;
				
				//we need to find the highest ranked change off all the attributes that have changed.
				switch (changedProperty)
				{
					case "Name":
						otherPropertyHasChanged = true;
						localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
						break;
					case "EA Specifics 1.0::Version":
						versionHasChanged = true;
						break;

					default:
						otherPropertyHasChanged = true;
						localChangeLevel = SetChangeLevel(ChangeLevel.Minor, localChangeLevel);
						break;
				}
			}

			//no changes at all but version number
			if (versionHasChanged && !otherPropertyHasChanged)
			{
				localChangeLevel = ChangeLevel.Downgrade;
			}

			return localChangeLevel;
		}

		private static ChangeLevel SetChangeLevel(ChangeLevel newChangeLevel, ChangeLevel localChangeLevel)
		{
			// Change Level can only increase not decrease if we detect a minor change after a major it still shall be a major.
			if (localChangeLevel < newChangeLevel)
			{
				return newChangeLevel;
			}
			else
			{
				return localChangeLevel;
			}
		}
	}
}