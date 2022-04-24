using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace LemonTree.Pipeline.Tools.SetFilterInSessionFile.CommandLineOptions
{
	[Verb("Custom", HelpText = "Sets Customs Filters  in the Single Session File.")]
	internal class CustomOptions : BaseOptions
	{
		public override string ToString()
		{
			return $"SingleSessionFile: {SingleFileSession}; ImpactedElements: {ImpactedElements}; ImpactedDiagrams: {ImpactedDiagrams};" ;
		}

		[Option("ImpactedElements", Required = true, HelpText = "Filter string for 'Impacted Elements'.")]
		public string ImpactedElements { get; set; }

		[Option("ImpactedDiagrams", Required = true, HelpText = "Filter string for 'Impacted Diagrams'.")]
		public string ImpactedDiagrams { get; set; }

		[Usage(ApplicationAlias = "LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				yield return new Example("Conflicted scenario", new CustomOptions
				{
					SingleFileSession = "LemonTreeSessionFileFromAutomation.ltsfs",
					ImpactedElements = "#Moved",
					ImpactedDiagrams = "$HideGraphicalChanges"
				});
			}
		}
	}
}
