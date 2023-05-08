using System.Reflection;
using System.Security;
using LemonTree.Pipeline.Tools.SemanticVersioning.Contracts;

namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner;

public class PluginLoader
{
	internal IEnumerable<ISemanticVersioningRule> Rules { get; private set; } = new List<ISemanticVersioningRule>();

	private Assembly LoadPlugin(string relativePath)
	{
		string root = Path.GetDirectoryName(typeof(Program).Assembly.Location);

		string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
		Console.WriteLine($"Loading rules from: {pluginLocation}");
		PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
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
		string[] pluginPaths = new[]
		{
			// Paths to plugins to load.
			@"..\..\..\..\LemonTree.Pipeline.Tools.SemanticVersioning.Rules\bin\Debug\netstandard2.0\LemonTree.Pipeline.Tools.SemanticVersioning.Rules.dll",
			//@"..\..\..\..\LemonTree.Pipeline.Tools.SemanticVersioning.Rules.Examples\bin\Debug\net472\LemonTree.Pipeline.Tools.SemanticVersioning.Rules.Examples.dll",
		};

		Rules = pluginPaths.SelectMany(pluginPath =>
		{
			var pluginAssembly = LoadPlugin(pluginPath);
			return CreateRules(pluginAssembly);
		}).ToList();

		foreach (ISemanticVersioningRule rule in Rules)
		{
			Console.WriteLine($"{rule.Name}\t - {rule.Description}");
		}
	}
}