using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal class TaggedValue : RuleBase, ISemanticVersioningRule
	{
		string ISemanticVersioningRule.Name => "TaggedValue";

		string ISemanticVersioningRule.Description { get; }

		ChangeLevel ISemanticVersioningRule.Change => ChangeLevel.Minor;

		ChangeLevel ISemanticVersioningRule.Apply(XElement modifiedElement, ChangeLevel currentChangeLevel)
		{
			//{CA244F4F-9793-42dd-9CB5-95A4D80403CE}	ElementOfRenamedTags (changed value)
			//{D3A08BF6-829B-4a61-8AC2-F5D763DEBD52}	ElementOfDeletedTag

			ChangeLevel localChangeLevel = ChangeLevel.None;

			/* Unable to find out, the changedProperty is a taggedValue:
			 * 
			 * this is taken from the DiffReport.xml file:
			 
				<element name="ElementOfRenamedTags" pathToClassifier="" qualifiedName="Model.AdditionalThings.ElementOfRenamedTags" ltName="ElementOfRenamedTags:Class" ltQualifiedName="Model.AdditionalThings.ElementOfRenamedTags:Class" qualifiedNameOld="Model.AdditionalThings.ElementOfRenamedTags" guid="{CA244F4F-9793-42dd-9CB5-95A4D80403CE}" umlType="Class" eaUmlType="Class" diffState="Modified">
					<changedProperties>
						<property name="Zwei" namespace="" oldValue="DeleteMe" newValue="MeWasValueChanged" diffState="Modified" />
					</changedProperties>
				</element>
			 
			 *
			 *
			// there's no way to see this is a taggedValue

			var changedProperties = modifiedElement.Elements().Elements();
			foreach (var element in changedProperties)
			{
				string changedProperty = element.Attribute("name").Value;
				string diffState = element.Attribute("diffstate").Value;

				//we need to find the highest ranked change off all the attributes that have changed.
				switch (diffState)
				{
					case "Modified":
						localChangeLevel = SetChangeLevel(ChangeLevel.Minor, localChangeLevel);
						break;
				}

			}
			*/

			return localChangeLevel;
		}
	}
}
