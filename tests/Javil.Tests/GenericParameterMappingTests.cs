using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Javil.Tests;

public class GenericParameterMappingTests
{
    private static string test_jar_directory;
    private static ContainerDefinition test_container;

    static GenericParameterMappingTests ()
    {
        var this_dir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        test_jar_directory = Path.Combine (this_dir!, "java_test_jars");

        var test_jar = Path.Combine (test_jar_directory, "test.jar");
        test_container = ContainerDefinition.ReadContainer (test_jar);
        test_container.AddType (new TypeDefinition ("java.lang", "Object", null, test_container, null));
    }

    [Test]
    public void BasicGenericBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericDerivedClass") ?? throw new ArgumentException ("missing type");
        var base_type = type.BaseType ?? throw new ArgumentException ("missing base type");
        var method = type.Methods.Single (m => m.Name == "doThing" && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault () ?? throw new ArgumentException ("missing base method");

        var mapping = new GenericParameterMapping ();
        mapping.AddMappingFromTypeReference (base_type);

        Assert.AreEqual ("T", method.ReturnType.ToString ());
        Assert.AreEqual ("T", mapping.GetMappedReference (base_method.ReturnType).ToString ());
    }

    [Test]
    public void BasicGenericConcreteBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var base_type = type.BaseType ?? throw new ArgumentException ("missing base type");
        var method = type.Methods.Single (m => m.Name == "doThing" && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault () ?? throw new ArgumentException ("missing base method");

        var mapping = new GenericParameterMapping ();
        mapping.AddMappingFromTypeReference (base_type);

        Assert.AreEqual ("T", base_method.ReturnType.ToString ());
        Assert.AreEqual ("java.lang.Object", mapping.GetMappedReference (base_method.ReturnType).ToString ());
    }

    [Test]
    public void BaseMethodGenericRederivedClass ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericRederivedClass") ?? throw new ArgumentException ("missing type");
        var base_type = type.BaseType ?? throw new ArgumentException ("missing base type");
        var method = type.Methods.Single (m => m.Name == "doThing5" && m.Parameters.Count == 1);
        var base_method = method.FindBaseMethodOrDefault () ?? throw new ArgumentException ("missing base method");

        var mapping = new GenericParameterMapping ();
        mapping.AddMappingFromTypeReference (base_type);

        Assert.AreEqual ("T", base_method.ReturnType.ToString ());
        Assert.AreEqual ("K", mapping.GetMappedReference (base_method.ReturnType).ToString ());
    }

    [Test]
    public void BasicGenericListConcreteBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var base_type = type.BaseType ?? throw new ArgumentException ("missing base type");
        var method = type.Methods.Single (m => m.Name == "doThing7");
        var base_method = method.FindBaseMethodOrDefault () ?? throw new ArgumentException ("missing base method");

        var mapping = new GenericParameterMapping ();
        mapping.AddMappingFromTypeReference (base_type);

        Assert.AreEqual ("java.util.List<T>", base_method.Parameters[0].ParameterType.ToString ());
        Assert.AreEqual ("java.util.List<java.lang.Object>", mapping.GetMappedReference (base_method.Parameters[0].ParameterType).ToString ());
    }

    static string FullyQualifyMethod (MethodDefinition? method)
    {
        if (method is null)
            return string.Empty;

        return $"{method.ReturnType.FullName} {method.DeclaringType?.FullName}.{method.FullName} ({string.Join (", ", method.Parameters.Select (p => $"{p.ParameterType.FullName} {p.Name}"))})";
    }
}
