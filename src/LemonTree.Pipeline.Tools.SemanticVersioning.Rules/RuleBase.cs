using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	public class RuleBase
	{
		public virtual int Sequence => 0;

		protected string GetDiffState(XElement element)
		{
			string retVal = "";

			if (element.Name.LocalName == "classifier")
			{
				var subElement = GetElementNode(element);

				retVal = subElement?.Attribute("diffState")?.Value;
			}
			else if (element.Name.LocalName == "diagram")
			{
				var subElement = GetElementNode(element);

				retVal = subElement?.Attribute("diffState")?.Value;
			}
			else
			{
				retVal = element.Attribute("diffState")?.Value;
			}
			
			return retVal ?? "";
		}

		protected string GetGuid(XElement element)
		{
			return element.Attribute("guid")?.Value;
		}

		protected string GetName(XElement element)
		{
			return element.Attribute("name")?.Value;
		}

		protected ChangeLevel SetChangeLevel(ChangeLevel newChangeLevel, ChangeLevel localChangeLevel)
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

		protected XElement GetElementNode(XElement item)
		{
			return item.Elements().FirstOrDefault(i => i.Name.LocalName.EndsWith("element"));
		}

		protected IEnumerable<XElement> GetChildNodes(XElement item)
		{
			if (item.Name.LocalName == "package")
			{
				return item.Elements().Where(i => !i.Name.LocalName.Equals("element"));
			}
			else
			{
				return item.Elements().Skip(1);
			}
		}

		protected bool NodeHasElement(XElement item)
		{
			return item.Elements().FirstOrDefault(i => i.Name.LocalName.EndsWith("element")) != null;
		}

		protected bool NodeIsPackage(XElement item)
		{
			return item.Name.LocalName.Equals("package", System.StringComparison.InvariantCultureIgnoreCase);
		}

		protected bool NodeIsElement(XElement item)
		{
			return item.Name.LocalName.Equals("classifier", System.StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
