using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonTree.Pipeline.Tools.SetFilterInSessionFile.CommandLineOptions
{
	public enum Exitcode
	{
		Error = -2,
		ErrorCmdParameter = -1, //Parsing error of command line parameter
		Success = 0, // Successful merge
	}
}
