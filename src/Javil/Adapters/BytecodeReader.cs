using Javil.Extensions;
using Xamarin.Android.Tools.Bytecode;

namespace Javil.Adapters;

public static class BytecodeReader
{
    public static ContainerDefinition Read (string fileName, ReaderParameters? parameters = null)
    {
        var container = new ContainerDefinition (fileName, parameters?.Resolver ?? new BaseContainerResolver ());

        var cp = new ClassPath (fileName);
        var flattened_types = new Collection<TypeDefinition> ();

        foreach (var pkg in cp.GetPackages ())
            foreach (var type in pkg.Value.OrderBy (t => t.ThisClass.Name.Value.Count (c => c == '$')))
                AddType (container, type);

        return container;
    }

    private static void AddType (ContainerDefinition container, ClassFile classFile)
    {
        var type_name = classFile.ThisClass.Name.Value.LastSubset ('/');

        if (!type_name.Contains ('$')) {
            container.Types.Add (CreateType (classFile.PackageName, type_name, null, classFile, container));
            return;
        }

        var top_declaring = type_name.FirstSubset ('$');
        var top_declaring_type = container.Types.FirstOrDefault (t => t.FullName == classFile.PackageName + "." + top_declaring);

        if (top_declaring_type is null)
            throw new InvalidOperationException ($"Could not find declaring type for '{classFile.ThisClass.Name.Value}'.");

        AddNestedType (top_declaring_type, classFile, container);
    }

    private static void AddNestedType (TypeDefinition declaring, ClassFile classFile, ContainerDefinition container)
    {
        var remaining_name = classFile.ThisClass.Name.Value.Substring (declaring.FullName.Length + 1);

        if (!remaining_name.Contains ('$')) {
            declaring.NestedTypes.Add (CreateType (classFile.PackageName, remaining_name, declaring, classFile, container));
            return;
        }

        var d = remaining_name.FirstSubset ('$');
        var d_type = declaring.NestedTypes.FirstOrDefault (t => t.Name == d);

        if (d_type is null)
            throw new InvalidOperationException ($"Could not find declaring type for '{classFile.ThisClass.Name.Value}'.");

        AddNestedType (d_type, classFile, container);
    }

    private static TypeDefinition CreateType (string @namespace, string name, TypeReference? declaringType, ClassFile classFile, ContainerDefinition container)
    {
        var signature = classFile.GetSignature ();
        var base_type = (TypeReference?)null;

        // Base type
        if (signature?.SuperclassSignature is string super_sig) {
            base_type = TypeReference.CreateFromSignature (super_sig, container);
        } else if (classFile.SuperClass is not null)
            base_type = TypeReference.CreateFromFullName (classFile.SuperClass.Name.Value, container);

        var visibility_flags = classFile.InnerClass?.InnerClassAccessFlags ?? classFile.AccessFlags;

        var td = new TypeDefinition (@namespace, name, declaringType, container, base_type) {
            IsInterface = classFile.AccessFlags.HasFlag (ClassAccessFlags.Interface),
            IsStatic = classFile.InnerClass?.InnerClassAccessFlags.HasFlag (ClassAccessFlags.Static) ?? false,
            IsEnum = classFile.AccessFlags.HasFlag (ClassAccessFlags.Enum),
            IsPublic = visibility_flags.HasFlag (ClassAccessFlags.Public),
            IsProtected = visibility_flags.HasFlag (ClassAccessFlags.Protected),
            IsPrivate = visibility_flags.HasFlag (ClassAccessFlags.Private),
            IsAbstract = classFile.AccessFlags.HasFlag (ClassAccessFlags.Abstract),
            IsFinal = classFile.AccessFlags.HasFlag (ClassAccessFlags.Final)
        };

        // Type parameters
        if (signature?.TypeParameters.Count > 0)
            foreach (var tp in signature.TypeParameters)
                AddGenericTypeParameter (td, tp, container);

        // Implemented interfaces
        for (var i = 0; i < classFile.Interfaces.Count; i++) {
            if (signature?.SuperinterfaceSignatures.Any () == true)
                td.ImplementedInterfaces.Add (new ImplementedInterface (TypeReference.CreateFromSignature (signature.SuperinterfaceSignatures[i], container)));
            else
                td.ImplementedInterfaces.Add (new ImplementedInterface (TypeReference.CreateFromFullName (classFile.Interfaces[i].Name.Value, container)));
        }

        // Fields
        foreach (var field in classFile.Fields)
            AddField (td, field);

        // Methods
        foreach (var method in classFile.Methods)
            AddMethod (td, method);

        // Attributes
        AddAttributes (td, classFile.Attributes);

        td.IsDeprecated = td.Attributes.OfType<Attributes.DeprecatedAttribute> ().Any ();
        td.SourceFileName = td.Attributes.OfType<Attributes.SourceFileAttribute> ().FirstOrDefault ()?.FileName;

        return td;
    }

    private static void AddField (TypeDefinition declaringType, FieldInfo field)
    {
        var name = field.Name;
        var signature = field.GetSignature ();

        var field_type = TypeReference.CreateFromSignature (signature ?? field.Descriptor, declaringType.Container);

        var fd = new FieldDefinition (name, field_type, declaringType) {
            IsStatic = field.AccessFlags.HasFlag (FieldAccessFlags.Static),
            IsPublic = field.AccessFlags.HasFlag (FieldAccessFlags.Public),
            IsFinal = field.AccessFlags.HasFlag (FieldAccessFlags.Final),
            IsProtected = field.AccessFlags.HasFlag (FieldAccessFlags.Protected),
            IsSythetic = field.AccessFlags.HasFlag (FieldAccessFlags.Synthetic),
            IsTransient = field.AccessFlags.HasFlag (FieldAccessFlags.Transient),
            IsPrivate = field.AccessFlags.HasFlag (FieldAccessFlags.Private),
            IsVolatile = field.AccessFlags.HasFlag (FieldAccessFlags.Volatile),

            IsDeprecated = field.Attributes.Get<DeprecatedAttribute> () is not null,
            Nullability = field.GetNullability (),

            Value = GetFieldValue (field)
        };

        AddAttributes (fd, field.Attributes);

        declaringType.Fields.Add (fd);
    }

    private static void AddMethod (TypeDefinition declaringType, MethodInfo method)
    {
        var name = method.Name;
        var signature = method.GetSignature ();

        var method_type = TypeReference.CreateFromSignature (signature?.ReturnTypeSignature ?? method.Descriptor.LastSubset (')'), declaringType.Container);

        var md = new MethodDefinition (name, method_type, declaringType) {
            IsStatic = method.AccessFlags.HasFlag (MethodAccessFlags.Static),
            IsFinal = method.AccessFlags.HasFlag (MethodAccessFlags.Final),
            IsPublic = method.AccessFlags.HasFlag (MethodAccessFlags.Public),
            IsProtected = method.AccessFlags.HasFlag (MethodAccessFlags.Protected),
            IsPrivate = method.AccessFlags.HasFlag (MethodAccessFlags.Private),
            IsAbstract = method.AccessFlags.HasFlag (MethodAccessFlags.Abstract),
            IsSynthetic = method.AccessFlags.HasFlag (MethodAccessFlags.Synthetic),
            IsBridge = method.AccessFlags.HasFlag (MethodAccessFlags.Bridge),
            IsVarargs = method.AccessFlags.HasFlag (MethodAccessFlags.Varargs),
            IsSynchronized = method.AccessFlags.HasFlag (MethodAccessFlags.Synchronized),
            IsNative = method.AccessFlags.HasFlag (MethodAccessFlags.Native),
            IsStrict = method.AccessFlags.HasFlag (MethodAccessFlags.Strict),
            IsDeprecated = method.Attributes.Get<DeprecatedAttribute> () is not null,
            ReturnTypeNullability = method.GetReturnTypeNullability ()
        };

        // Type parameters
        if (signature?.TypeParameters.Count > 0)
            foreach (var tp in signature.TypeParameters)
                AddGenericTypeParameter (md, tp, declaringType.Container);

        // Method parameters
        if (signature?.Parameters.Any () == true) {
            var method_params = method.GetParameters (); ;

            if (signature.Parameters.Count != method_params.Length)
                throw new InvalidOperationException ();

            for (var i = 0; i < method_params.Length; i++)
                md.Parameters.Add (new ParameterDefinition (md, method_params[i].Name, TypeReference.CreateFromSignature (signature.Parameters[i], declaringType.Container), i, method.GetParameterNullability (i)));
        } else
            for (var i = 0; i < method.GetParameters ().Length; i++)
                md.Parameters.Add (new ParameterDefinition (md, method.GetParameters ()[i].Name, TypeReference.CreateFromSignature (method.GetParameters ()[i].Type.TypeSignature!, declaringType.Container), i, method.GetParameterNullability (i)));

        // Checked exceptions
        foreach (var t in method.GetThrows ())
            md.CheckedExceptions.Add (new CheckedException (md, t));

        declaringType.Methods.Add (md);
    }

    private static void AddGenericTypeParameter (IGenericParameterProvider provider, TypeParameterInfo tp, ContainerDefinition container)
    {
        var tr = new GenericParameter (tp.Identifier, container);

        if (!string.IsNullOrEmpty (tp.ClassBound))
            tr.ClassBounds = TypeReference.CreateFromSignature (tp.ClassBound, container);

        foreach (var iface in tp.InterfaceBounds)
            tr.InterfaceBounds.Add (TypeReference.CreateFromSignature (iface, container));

        provider.GenericParameters.Add (tr);
    }

    private static void AddAttributes (IAttributeProvider provider, AttributeCollection attributes)
    {
        foreach (var attr in attributes)
            if (CreateAttribute (attr) is Attributes.BytecodeAttribute aa)
                provider.Attributes.Add (aa);
    }

    private static Attributes.BytecodeAttribute? CreateAttribute (AttributeInfo attr)
    {
        if (attr is CodeAttribute code) {
            var c = new Attributes.CodeAttribute (code.Name, code.MaxStack, code.MaxLocals, code.ByteCode);

            foreach (var ex in code.ExceptionTable)
                c.ExceptionTable.Add (new Attributes.CodeExceptionTableEntry (ex.StartPC, ex.EndPC, ex.HandlerPC, ex.CatchType.Name.Value));

            return c;
        }

        if (attr is ConstantValueAttribute cvt)
            return new Attributes.ConstantValueAttribute (cvt.Name, CreateConstant (cvt.Constant));

        if (attr is DeprecatedAttribute dep)
            return new Attributes.DeprecatedAttribute (dep.Name);

        if (attr is EnclosingMethodAttribute enc) {
            var m = enc.Method is not null ? new Attributes.ConstantNameAndTypeItem (enc.Method.Name.Value, enc.Method.Descriptor.Value) : null;
            return new Attributes.EnclosingMethodAttribute (enc.Name, enc.Class.Name.Value, m);
        }

        if (attr is InnerClassesAttribute inner) {
            var a = new Attributes.InnerClassesAttribute (inner.Name);

            foreach (var c in inner.Classes)
                a.InnerClasses.Add (new Attributes.InnerClassInfo (c.InnerName, c.OuterClassName));

            return a;
        }

        if (attr is LocalVariableTableAttribute lvt) {
            var a = new Attributes.LocalVariableTableAttribute (lvt.Name);

            foreach (var p in lvt.LocalVariables)
                a.LocalVariables.Add (new Attributes.LocalVariableTableEntry (p.StartPC, p.Length, p.Index, p.Name, p.Descriptor));

            return a;
        }

        if (attr is MethodParametersAttribute mpa) {
            var a = new Attributes.MethodParameterAttribute (mpa.Name);

            foreach (var p in mpa.ParameterInfo)
                a.Parameters.Add (new Attributes.MethodParameterInfo (p.Name, (Attributes.MethodParameterAccessFlags)p.AccessFlags));

            return a;
        }

        if (attr is RuntimeVisibleAnnotationsAttribute rva) {
            var a = new Attributes.RuntimeVisibleAnnotationsAttribute (rva.Name);

            foreach (var ann in rva.Annotations)
                a.Annotations.Add (CreateAnnotation (ann));

            return a;
        }

        if (attr is RuntimeInvisibleAnnotationsAttribute ria) {
            var a = new Attributes.RuntimeInvisibleAnnotationsAttribute (ria.Name);

            foreach (var ann in ria.Annotations)
                a.Annotations.Add (CreateAnnotation (ann));

            return a;
        }

        if (attr is RuntimeInvisibleParameterAnnotationsAttribute rip) {
            var a = new Attributes.RuntimeInvisibleParameterAnnotationsAttribute (rip.Name);

            foreach (var ann in rip.Annotations)
                a.Annotations.Add (CreateParameterAnnotation (ann));

            return a;
        }

        if (attr is SignatureAttribute sig)
            return new Attributes.SignatureAttribute (sig.Name, sig.Value);

        if (attr is SourceFileAttribute sfa)
            return new Attributes.SourceFileAttribute (sfa.Name, sfa.FileName);

        if (attr is StackMapTableAttribute smt)
            return new Attributes.StackMapTableAttribute (smt.Name, smt.Data);

        if (attr is UnknownAttribute unk)
            return new Attributes.UnknownAttribute (unk.Name, unk.Data);

        throw new NotSupportedException ($"Unknown attribute info type {attr.GetType ().FullName}");
    }

    private static Attributes.ParameterAnnotation CreateParameterAnnotation (ParameterAnnotation ann)
    {
        var a = new Attributes.ParameterAnnotation (ann.ParameterIndex);

        foreach (var annotation in ann.Annotations)
            a.Annotations.Add (CreateAnnotation (annotation));

        return a;
    }

    private static Attributes.AnnotationItem CreateAnnotation (Annotation ann)
    {
        var a = new Attributes.AnnotationItem (ann.Type);

        foreach (var value in ann.Values)
            if (CreateAnnotationElement (value) is KeyValuePair<string, Attributes.AnnotationElementValue> v)
                a.Values.Add (v);

        return a;
    }

    private static KeyValuePair<string, Attributes.AnnotationElementValue>? CreateAnnotationElement (KeyValuePair<string, AnnotationElementValue?> pair)
    {
        if (pair.Value is null)
            return null;

        return new KeyValuePair<string, Attributes.AnnotationElementValue> (pair.Key, CreateAnnotationElementValue (pair.Value));
    }

    private static Attributes.AnnotationElementValue CreateAnnotationElementValue (AnnotationElementValue value)
    {
        if (value is AnnotationElementEnum aee)
            return new Attributes.AnnotationElementEnum (aee.TypeName, aee.ConstantName);
        if (value is AnnotationElementClassInfo aec)
            return new Attributes.AnnotationElementClassInfo (aec.ClassInfo);
        if (value is AnnotationElementAnnotation aea)
            return new Attributes.AnnotationElementAnnotation (CreateAnnotation (aea.Annotation));
        if (value is AnnotationElementArray arr)
            return new Attributes.AnnotationElementArray (arr.Values.Select (a => CreateAnnotationElementValue (a)).ToArray ());
        if (value is AnnotationElementConstant aes)
            return new Attributes.AnnotationElementConstant (aes.Value);

        throw new NotSupportedException ($"Unknown attribute element value type {value.GetType ().FullName}");
    }

    private static Attributes.ConstantValueItem CreateConstant (ConstantPoolItem item)
    {
        if (item is ConstantPoolUtf8Item utf)
            return new Attributes.ConstantUtf8Item (utf.Value);
        if (item is ConstantPoolIntegerItem itg)
            return new Attributes.ConstantIntegerItem (itg.Value);
        if (item is ConstantPoolFloatItem flt)
            return new Attributes.ConstantFloatItem (flt.Value);
        if (item is ConstantPoolLongItem lng)
            return new Attributes.ConstantLongItem (lng.Value);
        if (item is ConstantPoolDoubleItem dbl)
            return new Attributes.ConstantDoubleItem (dbl.Value);
        if (item is ConstantPoolClassItem cls)
            return new Attributes.ConstantClassItem (cls.Name.Value);
        if (item is ConstantPoolStringItem str)
            return new Attributes.ConstantStringItem (str.StringData.Value);
        if (item is ConstantPoolFieldrefItem fld)
            return new Attributes.ConstantFieldRefItem ((Attributes.ConstantClassItem)CreateConstant (fld.Class), (Attributes.ConstantNameAndTypeItem)CreateConstant (fld.NameAndType));
        if (item is ConstantPoolMethodrefItem mth)
            return new Attributes.ConstantMethodRefItem ((Attributes.ConstantClassItem)CreateConstant (mth.Class), (Attributes.ConstantNameAndTypeItem)CreateConstant (mth.NameAndType));
        if (item is ConstantPoolInterfaceMethodrefItem im)
            return new Attributes.ConstantInterfaceMethodRefItem ((Attributes.ConstantClassItem)CreateConstant (im.Class), (Attributes.ConstantNameAndTypeItem)CreateConstant (im.NameAndType));
        if (item is ConstantPoolNameAndTypeItem nat)
            return new Attributes.ConstantNameAndTypeItem (nat.Name.Value, nat.Descriptor.Value);
        if (item is ConstantPoolMethodHandleItem mh)
            return new Attributes.ConstantMethodHandleItem (mh.referenceKind, mh.referenceIndex);
        if (item is ConstantPoolMethodTypeItem mt)
            return new Attributes.ConstantMethodTypeItem (mt.Descriptor.Value);
        if (item is ConstantPoolDynamicItem dyn)
            return new Attributes.ConstantDynamicItem (dyn.boostrapMethodAttrIndex, (Attributes.ConstantNameAndTypeItem)CreateConstant (dyn.NameAndType));
        if (item is ConstantPoolInvokeDynamicItem id)
            return new Attributes.ConstantInvokeDynamicItem (id.boostrapMethodAttrIndex, (Attributes.ConstantNameAndTypeItem)CreateConstant (id.NameAndType));
        if (item is ConstantPoolModuleItem mod)
            return new Attributes.ConstantModuleItem (mod.Name.Value);
        if (item is ConstantPoolPackageItem pkg)
            return new Attributes.ConstantPackageItem (pkg.Name.Value);

        throw new NotSupportedException ($"Unknown constant item type {item.GetType ().FullName}");
    }

    private static object? GetFieldValue (FieldInfo field)
    {
        if (field.Attributes.FirstOrDefault (a => a.Name == "ConstantValue") is not ConstantValueAttribute constantValue)
            return null;

        var constant = constantValue.Constant;

        switch (constant.Type) {
            case ConstantPoolItemType.Double:
                return ((ConstantPoolDoubleItem)constant).Value;
            case ConstantPoolItemType.Float:
                return ((ConstantPoolFloatItem)constant).Value;
            case ConstantPoolItemType.Long:
                return ((ConstantPoolLongItem)constant).Value;
            case ConstantPoolItemType.Integer:
                if (field.Descriptor == "Z")
                    return ((ConstantPoolIntegerItem)constant).Value == 1 ? true : false;
                else if (field.Descriptor == "C")
                    return (char)((ConstantPoolIntegerItem)constant).Value;
                else
                    return ((ConstantPoolIntegerItem)constant).Value;
            case ConstantPoolItemType.String:
                return ((ConstantPoolStringItem)constant).StringData.Value;
            default:
                throw new InvalidOperationException ("Unable to get value for: " + constant);
        }
    }
}
