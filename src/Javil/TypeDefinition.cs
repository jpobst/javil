using System.Diagnostics.CodeAnalysis;
using Javil.Attributes;
using Javil.Extensions;

namespace Javil;

public class TypeDefinition : TypeReference, IMemberDefinition, IAttributeProvider
{
    private Collection<BytecodeAttribute>? attributes;

    public Collection<BytecodeAttribute> Attributes => attributes ??= new Collection<BytecodeAttribute> ();
    public Collection<TypeDefinition> NestedTypes { get; } = new Collection<TypeDefinition> ();
    public Collection<ImplementedInterface> ImplementedInterfaces { get; } = new Collection<ImplementedInterface> ();
    public Collection<FieldDefinition> Fields { get; } = new Collection<FieldDefinition> ();
    public Collection<MethodDefinition> Methods { get; } = new Collection<MethodDefinition> ();
    public string? SourceFileName { get; set; }

    public TypeReference? BaseType { get; }

    internal TypeDefinition (string name, ContainerDefinition container) : base (string.Empty, name, container)
    {
    }

    public TypeDefinition (string @namespace, string name, TypeReference? declaringType, ContainerDefinition container, TypeReference? baseType) : base (@namespace, name, container, declaringType)
    {
        BaseType = baseType;
    }

    public bool IsInterface { get; set; }

    public bool IsClass => !IsInterface;

    public bool IsStatic { get; set; }

    public bool IsEnum { get; set; }

    public bool IsPublic { get; set; }

    public bool IsProtected { get; set; }

    public bool IsPrivate { get; set; }

    public bool IsAbstract { get; set; }

    public bool IsFinal { get; set; }

    public bool IsNested => DeclaringType is not null;

    public bool IsDeprecated { get; set; }

    public bool HasAttributes => attributes?.Any () == true;

    /// <summary>
    /// Finds the concrete method that implements the requested interface method declaration.
    /// Example: Given the hierarchy 'MyClass : Clonable<MyClass>', returns 'MyClass.Clone ()'
    ///          for the implemented interface 'Clonable<MyClass>' and method 'Clone ()'.
    /// </summary>
    public MethodDefinition? FindImplementedInterfaceOrDefault (ImplementedInterface iface, MethodDefinition method)
    {
        // Static methods cannot be implemented
        if (method.IsStatic)
            return null;

        var mapping = new GenericParameterMapping ();
        mapping.AddMappingFromTypeReference (iface.InterfaceType);

        return FindImplementedMethodInClass (this, method, mapping);
    }

    private MethodDefinition? FindImplementedMethodInClass (TypeDefinition? type, MethodDefinition method, GenericParameterMapping mapping)
    {
        if (type is null)
            return null;

        var candidates = type.Methods.OfType<MethodDefinition> ().Where (m => m.Name == method.Name && m.Parameters.Count == method.Parameters.Count);

        foreach (var candidate in candidates)
            if (TypeExtensions.AreMethodsCompatible (candidate, method, mapping))
                return candidate;

        // Check if any base types have implemented the method
        if (type.BaseType is TypeReference base_type) {
            // Make a clone so we can have a clean mapping later
            var base_mapping = mapping.Clone ();
            base_mapping.AddMappingFromTypeReference (base_type);

            if (FindImplementedMethodInClass (base_type.Resolve (), method, base_mapping) is MethodDefinition md)
                return md;
        }

        if (method.DeclaringType?.Resolve () is TypeDefinition declaring_interface) {
            // Check if any other implemented interfaces implement this one and provides a default method of it
            foreach (var ii in type.ImplementedInterfaces) {
                var iface_mapping = mapping.Clone ();
                iface_mapping.AddMappingFromTypeReference (ii.InterfaceType);

                if (ii.InterfaceType.Resolve () is TypeDefinition iface && iface.ImplementsInterface (declaring_interface)) {
                    if (FindImplementedMethodInInterface (iface, method, iface_mapping) is MethodDefinition md)
                        return md;
                }
            }

        }

        return null;
    }

    private MethodDefinition? FindImplementedMethodInInterface (TypeDefinition? type, MethodDefinition method, GenericParameterMapping mapping)
    {
        if (type is null)
            return null;

        var candidates = type.Methods.OfType<MethodDefinition> ().Where (m => !m.IsAbstract && m.Name == method.Name && m.Parameters.Count == method.Parameters.Count);

        foreach (var candidate in candidates)
            if (TypeExtensions.AreMethodsCompatible (candidate, method, mapping))
                return candidate;

        foreach (var iface in type.ImplementedInterfaces)
            if (FindImplementedMethodInInterface (iface.InterfaceType.Resolve (), method, mapping) is MethodDefinition md)
                return md;

        return null;
    }

    /// <summary>
    /// If this interface method is a default method, it may be implementing a method declaration
    /// from an "implemented interface".
    /// Example:
    ///   interface Describable { String Describe (); }
    ///   interface Animal implements Describable { String Describe () { return "Animal"; } }
    ///   'Animal.Describe' would return 'Describable.Describe' since it provides an implementation for the method declaration.
    /// </summary>    
    public bool TryFindDeclarationMethodIsProvidingImplementationFor (MethodDefinition method, [NotNullWhen (true)] out ImplementedInterface? implementedInterface, [NotNullWhen (true)] out MethodDefinition? implementedMethod)
    {
        // TODO: Needs unit tests
        implementedInterface = null;
        implementedMethod = null;

        // If the method isn't providing an implementation of anything, return null
        if (method.IsAbstract)
            return false;

        // TODO: Ensure this method is part of this interface

        // See if there is an abstract version of this method on this interface
        foreach (var ii in ImplementedInterfaces) {
            if (ii.InterfaceType.Resolve () is TypeDefinition type) {
                var mapping = new GenericParameterMapping ();
                mapping.AddMappingFromTypeReference (ii.InterfaceType);

                if (TryFindDeclarationMethodIsProvidingImplementationForCore (ii, method, mapping, out var foundIface, out var md)) {
                    implementedInterface = foundIface;
                    implementedMethod = md;
                    return true;
                }
            }
        }

        return false;
    }

    bool TryFindDeclarationMethodIsProvidingImplementationForCore (ImplementedInterface iface, MethodDefinition method, GenericParameterMapping mapping, [NotNullWhen (true)] out ImplementedInterface? implementedInterface, [NotNullWhen (true)] out MethodDefinition? implementedMethod)
    {
        implementedInterface = null;
        implementedMethod = null;

        // See if there is an abstract version of this method on this interface
        if (iface.InterfaceType.Resolve () is TypeDefinition type) {

            // Make a clone so don't affect parent usage
            var new_mapping = mapping.Clone ();
            new_mapping.AddMappingFromTypeReference (iface.InterfaceType);

            if (type.Name == "Temporal" && method.Name == "isSupported")
                Console.WriteLine ();

            // Look for method
            var candidates = type.Methods.OfType<MethodDefinition> ().Where (m => m.IsAbstract && m.Name == method.Name && m.Parameters.Count == method.Parameters.Count);

            foreach (var candidate in candidates)
                if (TypeExtensions.AreMethodsCompatible (method, candidate, mapping)) {
                    implementedInterface = iface;
                    implementedMethod = candidate;
                    return true;
                }

            // Recurse into implemented interfaces
            foreach (var i in type.ImplementedInterfaces)
                if (TryFindDeclarationMethodIsProvidingImplementationForCore (i, method, mapping, out var ii, out var md)) {
                    implementedInterface = ii;
                    implementedMethod = md;
                    return true;
                }
        }

        return false;
    }
}
