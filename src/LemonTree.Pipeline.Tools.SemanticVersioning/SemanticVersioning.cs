using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LemonTree.Pipeline.Tools.Database;
using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

namespace LemonTree.Pipeline.Tools.SemanticVersioning;

public class SemanticVersioning
{
	private readonly IEnumerable<ISemanticVersioningRule> _rules;

	public SemanticVersioning(IEnumerable<ISemanticVersioningRule> rules)
	{
		_rules = rules;
	}

	private static string CreateNewVersion(string version, ChangeLevel changeLevel)
	{
		// We should detect if the version number supplied fits the standard pattern
		// we should use System.Version for this? 
		// should add 3 rd Level for proper semantic version.
		try
		{
			string[] versionDetails = version.Split('.');
			int major = versionDetails.Length > 0 ? Convert.ToInt32(versionDetails[0]) : 0;
			int minor = versionDetails.Length > 1 ? Convert.ToInt32(versionDetails[1]) : 0;
			int patch = versionDetails.Length > 2 ? Convert.ToInt32(versionDetails[2]) : 0;

			switch (changeLevel)
			{
				case ChangeLevel.None:
					// do nothing
					break;
				case ChangeLevel.Patch:
					patch++;
					break;
				case ChangeLevel.Minor:
					minor++;
					patch = 0;
					break;
				case ChangeLevel.Major:
					major++;
					minor = 0;
					patch = 0;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(changeLevel), changeLevel, null);
			}

			return $"{major}.{minor}.{patch}";
		}
		catch (Exception ex)
		{

			Console.WriteLine($"{version} doesn't seem to fit the pattern major.minor.patch e.g. 1.1.0");
			Console.WriteLine(ex.Message);
		}

		//We could not modify it - doesn't fit the logic
		return version;
	}

	private static void UpdateVersion(string guid, string newVersion)
	{
		try
		{
			string placeholder = ModelAccess.ParameterPlaceholder();
			ModelAccess.RunSql($"UPDATE t_object SET version = {placeholder} WHERE ea_guid = {placeholder}",
				new EAParameter("newVersion", newVersion),
				new EAParameter("guid", guid));
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error UpdateVersion: {ex.Message}");
		}
	}

	private static string GetVersionInfoFormElement(string guid)
	{
		string placeholder = ModelAccess.ParameterPlaceholder();

		return ModelAccess.RunSQLQueryScalarAsString($"SELECT DISTINCT t_object.version FROM t_object WHERE t_object.ea_guid = {placeholder}",
			new EAParameter("guid", guid));
	}

	public Statistics RunStatistics { get; private set; } = new Statistics();
 
	/// <summary>
	/// Runs the semantic versioning for a set of changes 
	/// </summary>
	/// <param name="file">xml file with all changes</param>
	public void Run(string file)
	{
		RunStatistics.Reset();

		var doc = XDocument.Parse(File.ReadAllText(file));
		var elements = doc.Root.Descendants().Where(item => item.Name.LocalName == "element");
		var modified = elements.Where(item =>
			item.Attributes().Any(attribute => attribute.Name.LocalName == "diffState" && attribute.Value == "Modified"));

		foreach (var modifiedElement in modified)
		{
			string guid = modifiedElement.Attribute("guid").Value.ToString();
			string version = GetVersionInfoFormElement(guid);

			var overallChange = ChangeLevel.None;
			foreach (var rule in _rules)
			{
				//we need to find the highest ranked change off all changes
				var singleChange = rule.Apply(modifiedElement);
				if (singleChange > overallChange)
				{
					overallChange = singleChange;
				}

				RunStatistics.Update(rule, singleChange);
			}

			if (overallChange != ChangeLevel.None)
			{
				string newVersion = CreateNewVersion(version, overallChange);
				Console.WriteLine($"{guid} - {version} ==> {newVersion}");
				UpdateVersion(guid, newVersion);
			}
			else
			{
				Console.WriteLine($"{guid} - {version} ==> {version}");
			}
		}
	}
}