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

	public SemanticVersioning(IEnumerable<ISemanticVersioningRule> rules, bool tryRun)
	{
		_rules = rules.OrderBy(r => r.Sequence);
		_tryRun = tryRun;
	}

	public List<string> Exceptions = new List<string>();

	private bool _tryRun;

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

	private void UpdateVersion(string guid, string newVersion, bool tryRun)
	{
		try
		{
			if (!tryRun)
			{
				string placeholder = ModelAccess.ParameterPlaceholder();

				ModelAccess.RunSql($"UPDATE t_object SET version = {placeholder} WHERE ea_guid = {placeholder}",
					new EAParameter("newVersion", newVersion),
					new EAParameter("guid", guid));
			}
		}
		catch (Exception ex)
		{
			Exceptions.Add($"Error update version on item {guid}: {ex.Message}");
		}
	}

	private string GetVersionInfoFromElement(string guid)
	{
		try
		{
			string placeholder = ModelAccess.ParameterPlaceholder();

			return ModelAccess.RunSQLQueryScalarAsString($"SELECT DISTINCT t_object.version FROM t_object WHERE t_object.ea_guid = {placeholder}",
				new EAParameter("guid", guid));
		}
		catch (Exception ex)
		{
			Exceptions.Add($"Error get version on item {guid}: {ex.Message}");
		}

		return null;
	}

	public SemanticVersionStatistics VersioningStatistics { get; private set; } = new SemanticVersionStatistics();
 
	/// <summary>
	/// Runs the semantic versioning for a set of changes 
	/// </summary>
	/// <param name="file">xml file with all changes</param>
	public void Run(string file)
	{
		VersioningStatistics.Reset();
		Exceptions.Clear();

		var doc = XDocument.Parse(File.ReadAllText(file));

		var packages = doc.Root.Descendants().Where(item => item.Name.LocalName == "package");

		foreach (var package in packages)
		{
			RunRulesOnNode(package, packages);

			var elements = package.Descendants().Where(item => item.Name.LocalName == "classifier");
			foreach (XElement element in elements)
			{
				RunRulesOnNode(element, elements);
			}
		}
	}

	private void RunRulesOnNode(XElement node, IEnumerable<XElement> elements)
	{
		string guid = node.Attribute("guid").Value.ToString();
		string version = GetVersionInfoFromElement(guid);

		if (string.IsNullOrEmpty(version))
		{
			Console.WriteLine($"{guid} - {version} ==> not set");
			return;
		}

		var overallChangeLevel = ChangeLevel.None;
		foreach (var rule in _rules)
		{
			//we need to find the highest ranked change off all changes
			var changeLevelOfCurrentRule = rule.Apply(node, overallChangeLevel);
			if (changeLevelOfCurrentRule > overallChangeLevel)
			{
				overallChangeLevel = changeLevelOfCurrentRule;
			}

			VersioningStatistics.Add(rule, changeLevelOfCurrentRule, guid);
		}

		if (overallChangeLevel == ChangeLevel.Downgrade)
		{
			//downgrade version number to old value in diff file
			var oldVersionItem = GetVersionNode(node);

			if (null != oldVersionItem)
			{
				string oldVersion = oldVersionItem.Attribute("oldValue").Value;
				Console.WriteLine($"{guid} - {version} ==> {oldVersion}");
				UpdateVersion(guid, oldVersion, _tryRun);
			}
		}
		else if (overallChangeLevel != ChangeLevel.None)
		{
			string newVersion = CreateNewVersion(version, overallChangeLevel);
			Console.WriteLine($"{guid} - {version} ==> {newVersion}");
			UpdateVersion(guid, newVersion, _tryRun);
		}
		else
		{
			Console.WriteLine($"{guid} - {version} ==> {version}");
		}
	}

	private XElement GetVersionNode(XElement node)
	{
		return node.Elements().Elements().Elements().FirstOrDefault(i => i.FirstAttribute.Value.Equals("EA Specifics 1.0::Version"));
	}
}