using System.IO;
using System.Reflection;
using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner;

public class PluginLoader
{
	internal IEnumerable<ISemanticVersioningRule> Rules { get; private set; } = new List<ISemanticVersioningRule>();

	private Assembly LoadPlugin(string relativePath)
	{
		string root = Path.GetDirectoryName(typeof(Program).Assembly.Location);

		string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
		var loadContext = new PluginLoadContext(pluginLocation);
		return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
	}

	private IEnumerable<ISemanticVersioningRule> CreateRules(Assembly assembly)
	{
		int count = 0;
		foreach (Type type in assembly.GetTypes())
		{
			if (typeof(ISemanticVersioningRule).IsAssignableFrom(type))
			{
				if (Activator.CreateInstance(type) is ISemanticVersioningRule result)
				{
					count++;
					yield return result;
				}
			}
		}

		if (count == 0)
		{
			string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
			throw new ApplicationException(
				$"Can't find any type which implements ISemanticVersioningRule in {assembly} from {assembly.Location}.\n" +
				$"Available types: {availableTypes}");
		}
	}

	internal void Run()
	{
        //default plugin search path is <ExecutingAssembly>/Rules/Debug

        var assemblyConfigurationAttribute = typeof(PluginLoader).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		string pluginPath = Path.Combine(exePath, "Rules", buildConfigurationName);
		
		var rules = new List<ISemanticVersioningRule>();
		int pluginsLoaded = 0;
		foreach (string dll in Directory.GetFiles(pluginPath, "*.dll"))
		{
			try
			{
				var pluginAssembly = LoadPlugin(dll);
				var rulesFromDLL = CreateRules(pluginAssembly);

				Console.WriteLine($"- {pluginAssembly.GetName().Name} ({rulesFromDLL.Count()} rules)");
				rules.AddRange(rulesFromDLL);

				pluginsLoaded++;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WARN: unable to load rules from assembly '{pluginPath}': {ex.Message}");
			}
		}
				
		Rules = rules;

		Console.WriteLine($"Loaded rules in total: {Rules.Count()}");
	}
}