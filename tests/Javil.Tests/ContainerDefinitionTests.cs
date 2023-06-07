using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Javil.Tests;

public class ContainerDefinitionTests
{
    private static string test_jar_directory;

    static ContainerDefinitionTests ()
    {
        var this_dir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        test_jar_directory = Path.Combine (this_dir!, "java_test_jars");
    }


    [Test]
    public void BasicTest ()
    {
        var test_jar = Path.Combine (test_jar_directory, "test.jar");
        var container = ContainerDefinition.ReadContainer (test_jar);
        
        var type = container.FindType ("com.example.ParameterInterface");
        Assert.IsNotNull (type);
    }
}
