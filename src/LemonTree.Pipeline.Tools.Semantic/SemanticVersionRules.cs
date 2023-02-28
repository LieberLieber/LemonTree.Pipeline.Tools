using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LemonTree.Pipeline.Tools.Semantic
{
    class SemanticVersionRules
    {
        internal static ChangeLevel DetectChangeLevel(XElement modifedElement)
        {
            ChangeLevel localChangeLevel = ChangeLevel.None;
            foreach (var element in modifedElement.Elements().Elements())
            {
                string changedProperty = element.Attribute("name").Value;
                //we need to find the highest rankend change - off all the attributes that have changed.
                switch (changedProperty)
                {
                    case "Name":
                        localChangeLevel = SetChangeLevel(ChangeLevel.Major, localChangeLevel);
                        break;

                    default:
                        localChangeLevel = SetChangeLevel(ChangeLevel.Minor, localChangeLevel);
                        break;
                }



            }
            return localChangeLevel;
        }

        private static ChangeLevel SetChangeLevel(ChangeLevel newChangeLevel, ChangeLevel localChangeLevel)
        {
            //Change Level can only increase not decrease if we detect a minor change after a major it still shall be a major.
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
