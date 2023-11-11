using System;
using System.Collections.Generic;
using System.Text;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Rules
{
	internal static class DiffStates
	{
		internal const string MODIFIED = "Modified";
		internal const string NEW = "New";
		internal const string SUB_ELEMENT_MODIFIED = "SubElementModified";
		internal const string REMOVED = "Removed";
	}
}
