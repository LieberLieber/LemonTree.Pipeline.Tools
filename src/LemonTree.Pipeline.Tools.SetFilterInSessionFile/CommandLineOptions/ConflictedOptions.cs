using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace LemonTree.Pipeline.Tools.SetFilterInSessionFile.CommandLineOptions
{
	[Verb("Conflicted", HelpText = "Sets the Standard Filters for \"Conflicted\" state in the Single Session File.")]
	internal class ConflictedOptions:BaseOptions
	{
		public override string ToString()
		{
			return $"SingleSessionFile: {SingleFileSession};";
		}
		

		[Usage(ApplicationAlias = "LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				yield return new Example("Conflicted scenario", new ConflictedOptions
				{
					SingleFileSession = "LemonTreeSessionFileFromAutomation.ltsfs"
				});
			}
		}
	}
}
