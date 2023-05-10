using NUnit.Framework;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace JUST.UnitTests;

[TestFixture, Category("CustomFunctions")]
public class ExternalAssemblyBugTests
{
	private JUSTContext _context;
	[SetUp]
	public void Setup()
	{
		_context = new JUSTContext();
	}

	/// <summary>
	/// <para>
	/// We want to load external assembly to load context, use it and then unload it.
	/// This assembly can be located anywhere (not only in bin directory)
	/// </para>
	/// <para>
	/// NOTE: THIS IS DOTNET CORE 3.1 TEST CODE
	/// THIS PROJECT MUST NOT REFERENCE ExternalMethods to avoid having its assembly in binaries directory.
	/// </para>
	/// </summary>
	[Test]
	public void ExternalStaticMethodPreloadedFromAssembly()
	{
		string currentDirectory = Path.GetDirectoryName(typeof(ExternalAssemblyBugTests).Assembly.Location);
		string searchPath = Path.GetFullPath(currentDirectory + @"\..\..\..\..\");

		string assemblyFilePath = Directory.GetFiles(
					searchPath, "ExternalMethods.dll", SearchOption.AllDirectories).First();

		AssemblyLoadContext loadContext = new(name: Guid.NewGuid().ToString(), isCollectible: true);

		{
			// read the file and release it immediately (do not keep handle)
			FileStream stream = new(assemblyFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			// alternatively can be used the line below:
			//using var stream = new MemoryStream(File.ReadAllBytes(assemblyFilePath));
			Assembly loadedAssembly = loadContext.LoadFromStream(stream);

			Assert.AreEqual("<Unknown>", loadedAssembly.ManifestModule.FullyQualifiedName);
			Assert.AreEqual("<Unknown>", loadedAssembly.ManifestModule.Name);
			Assert.AreEqual("ExternalMethods.dll", loadedAssembly.ManifestModule.ScopeName);
			// it seems that ScopeName is only reliable information in this case
		}

		const string input = "{ }";
		const string transformer = "{ \"result\": \"#StaticMethod()\" }";

		_context.RegisterCustomFunction("ExternalMethods", "ExternalMethods.ExternalClass", "StaticMethod");
		string result = new JsonTransformer(_context).Transform(transformer, input);

		Assert.AreEqual("{\"result\":\"External Static\"}", result);

		_context.ClearCustomFunctionRegistrations();
		loadContext.Unload();
	}
}
