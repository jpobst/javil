using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Javil.Tests;

public class FindImplementedMethodTests
{
    private static string test_jar_directory;
    private static ContainerDefinition test_container;

    static FindImplementedMethodTests ()
    {
        var this_dir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        test_jar_directory = Path.Combine (this_dir!, "java_test_jars");

        var test_jar = Path.Combine (test_jar_directory, "test.jar");
        test_container = ContainerDefinition.ReadContainer (test_jar);
        test_container.AddType (new TypeDefinition ("java.lang", "Object", null, test_container, null));
    }

    [Test]
    public void BasicImplementedMethod ()
    {
        // InterfaceTypeA implements interface method from InterfaceA
        var type = test_container.FindType ("com.example.InterfaceTypeA") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "InterfaceA");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "add");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.InterfaceTypeA.add (int param1, int param2)", FullyQualifyMethod (implemented_method));
    }

    [Test]
    public void BaseClassImplementedMethod ()
    {
        // InterfaceTypeB's base class InterfaceTypeA implements interface method from InterfaceB
        var type = test_container.FindType ("com.example.InterfaceTypeB") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "InterfaceB");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "subtract");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.InterfaceTypeA.subtract (int param1, int param2)", FullyQualifyMethod (implemented_method));
    }

    [Test]
    public void AbstractClassMissingImplementedMethod ()
    {
        // InterfaceTypeC doesn't implement interface, but is abstract so it isn't required
        var type = test_container.FindType ("com.example.InterfaceTypeC") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "InterfaceA");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "add");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNull (implemented_method);
    }

    [Test]
    public void MethodImplementedAsDIMOnAnotherExplicitlyDeclaredInterface ()
    {
        // InterfaceC provides a default implementation of InterfaceA.add
        var type = test_container.FindType ("com.example.InterfaceTypeD") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "InterfaceA");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "add");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.InterfaceC.add (int param1, int param2)", FullyQualifyMethod (implemented_method));
    }

    [Test]
    public void MethodImplementedAsDIMOnAnotherExplicitlyDeclaredInterfaceParent ()
    {
        // InterfaceC provides a default implementation of InterfaceA.add
        var type = test_container.FindType ("com.example.InterfaceTypeE") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "InterfaceA");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "add");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.InterfaceC.add (int param1, int param2)", FullyQualifyMethod (implemented_method));
    }

    [Test]
    public void BasicClosedGenericImplementedMethod ()
    {
        // InterfaceTypeE implements Comparable<InterfaceTypeE>.compareTo
        var type = test_container.FindType ("com.example.InterfaceTypeE") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "MyComparable");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "compareTo");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.InterfaceTypeE.compareTo (com.example.InterfaceTypeE p0)", FullyQualifyMethod (implemented_method));
    }

    [Test]
    public void BasicOpenGenericImplementedMethod ()
    {
        // GenericInterfaceTypeA<T> implements MyComparable<T>.compareTo
        var type = test_container.FindType ("com.example.GenericInterfaceTypeA") ?? throw new ArgumentException ("missing type");
        var iface = type.ImplementedInterfaces.Single (i => i.InterfaceType.Name == "MyComparable");
        var iface_resolved = iface.InterfaceType.Resolve () ?? throw new ArgumentException ("missing interface");
        var method = iface_resolved.Methods.Single (m => m.Name == "compareTo");

        var implemented_method = type.FindImplementedInterfaceOrDefault (iface, method);

        Assert.IsNotNull (implemented_method);
        Assert.AreEqual ("int com.example.GenericInterfaceTypeA<S>.compareTo (S p0)", FullyQualifyMethod (implemented_method));
    }

    static string FullyQualifyMethod (MethodDefinition? method)
    {
        if (method is null)
            return string.Empty;

        return $"{method.ReturnType.FullName} {method.DeclaringType?.FullName}.{method.FullName} ({string.Join (", ", method.Parameters.Select (p => $"{p.ParameterType.FullName} {p.Name}"))})";
    }
}
