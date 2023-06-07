using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Javil.Tests;

public class FindBaseMethodTests
{
    private static string test_jar_directory;
    private static ContainerDefinition test_container;

    static FindBaseMethodTests ()
    {
        var this_dir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        test_jar_directory = Path.Combine (this_dir!, "java_test_jars");

        var test_jar = Path.Combine (test_jar_directory, "test.jar");
        test_container = ContainerDefinition.ReadContainer (test_jar);
        test_container.AddType (new TypeDefinition ("java.lang", "Object", null, test_container, null));
    }

    [Test]
    public void BasicBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("java.lang.Object com.example.BaseMethodBaseClass.doThing ()", FullyQualifyMethod (base_method));
    }

    [Test]
    public void CovariantBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && !m.IsSynthetic && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "java.lang.Object");
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method despite the covariant type
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("java.lang.Object com.example.BaseMethodBaseClass.doThing (java.lang.Object obj)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GrandchildBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGrandchildClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && !m.IsSynthetic && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "int");
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method on our grandparent type
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("java.lang.Object com.example.BaseMethodBaseClass.doThing (int value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void StaticMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing3" && !m.IsSynthetic && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault ();

        // This should be null because static methods cannot be overridden
        Assert.IsNull (base_method);
    }

    [Test]
    public void BasicGenericBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing ()", FullyQualifyMethod (base_method));
    }

    [Test]
    public void BasicGenericConcreteBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && m.Parameters.Count == 0);
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing ()", FullyQualifyMethod (base_method));
    }

    [Test]
    public void CovariantGenericConcreteBaseMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing" && !m.IsSynthetic && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "java.lang.Object");
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method despite the covariant type
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing (java.lang.Object obj)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void VoidGenericConcreteBaseMethod_GenericParameter ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing4" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from T to Object
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.BaseMethodGenericBaseClass<T>.doThing4 (T value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void VoidGenericConcreteBaseMethod_GenericParameter_GenericList ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing7" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from List<T> to List<Object>
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.BaseMethodGenericBaseClass<T>.doThing7 (java.util.List<T> list)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void VoidGenericConcreteBaseMethod_GenericParameter_GenericListWildcard ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing8" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from List<T> to List<Object>
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.BaseMethodGenericBaseClass<T>.doThing8 (java.util.List<? super T> list)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericConcreteBaseMethod_GenericParameter ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing5" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter and return type is changed from T to Object
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing5 (T value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void CovariantGenericConcreteBaseMethod_GenericParameter ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericConcreteDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing6" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from T to Object and return type is changed from T to a covariant return type
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing6 (T value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericRederivedBaseMethod_GenericParameter ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericRederivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing5" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter and return type is changed from T to K
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing5 (T value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericRederivedGrandchildBaseMethod_GenericParameter ()
    {
        var type = test_container.FindType ("com.example.BaseMethodGenericRederivedGrandchildClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing6" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Handle multiple levels of generic type arguments:
        // BaseMethodGenericRederivedGrandchildClass extends BaseMethodGenericRederivedClass<Object>
        // BaseMethodGenericRederivedClass<K> extends BaseMethodGenericBaseClass<K>
        // BaseMethodGenericBaseClass<T>
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("T com.example.BaseMethodGenericBaseClass<T>.doThing6 (T value)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericMethod ()
    {
        var type = test_container.FindType ("com.example.BaseMethodDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing4" && m.Parameters.Count == 2);
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("A com.example.BaseMethodBaseClass.doThing4 (A value, java.util.List<A> list)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericMethodWithConstraint ()
    {
        var type = test_container.FindType ("com.example.BaseMethodDerivedClass") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing5" && m.Parameters.Count == 2);
        var base_method = method.FindBaseMethodOrDefault ();

        // Ensure we found the base method
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("A com.example.BaseMethodBaseClass.doThing5 (A value, java.util.List<? super A> list)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericDoubleAsterisk ()
    {
        var type = test_container.FindType ("com.example.GenericDerivedNestedType") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing2" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from T to Object
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.GenericNestedType<A, B>.doThing2 (java.util.Map<? extends A, ? extends B> map)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericQuestionMark ()
    {
        var type = test_container.FindType ("com.example.GenericDerivedNestedType") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing3" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from T to Object
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.GenericNestedType<A, B>.doThing3 (A key, java.util.function.Function<? super A, ? extends B> mappingFunction)", FullyQualifyMethod (base_method));
    }

    [Test]
    public void GenericRawClass ()
    {
        var type = test_container.FindType ("com.example.GenericDerivedNestedType") ?? throw new ArgumentException ("missing type");
        var method = type.Methods.Single (m => m.Name == "doThing4" && !m.IsSynthetic);
        var base_method = method.FindBaseMethodOrDefault ();

        // Parameter is changed from T to Object
        Assert.IsNotNull (base_method);
        Assert.AreEqual ("void com.example.GenericNestedType<A, B>.doThing4 (java.lang.Class<?>[] classes)", FullyQualifyMethod (base_method));
    }

    static string FullyQualifyMethod (MethodDefinition? method)
    {
        if (method is null)
            return string.Empty;

        return $"{method.ReturnType.FullName} {method.DeclaringType?.FullName}.{method.FullName} ({string.Join (", ", method.Parameters.Select (p => $"{p.ParameterType.FullName} {p.Name}"))})";
    }
}
