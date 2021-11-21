namespace Javil.Attributes;

public class ConstantValueAttribute : BytecodeAttribute
{
    public ConstantValueItem Value { get; set; }

    public ConstantValueAttribute (string name, ConstantValueItem value) : base (name)
    {
        Value = value;
    }
}

public abstract class ConstantValueItem
{
    public abstract ConstantPoolItemType Type { get; }
}

public class ConstantClassItem : ConstantValueItem
{
    public string Name { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Class;

    public ConstantClassItem (string name)
    {
        Name = name;
    }
}

public abstract class ConstantMemberRefItem : ConstantValueItem
{
    public ConstantClassItem Class { get; set; }
    public ConstantNameAndTypeItem NameAndType { get; set; }

    public ConstantMemberRefItem (ConstantClassItem @class, ConstantNameAndTypeItem nameAndType)
    {
        Class = @class;
        NameAndType = nameAndType;
    }
}

public class ConstantFieldRefItem : ConstantMemberRefItem
{
    public override ConstantPoolItemType Type => ConstantPoolItemType.FieldRef;

    public ConstantFieldRefItem (ConstantClassItem @class, ConstantNameAndTypeItem nameAndType) : base (@class, nameAndType) 
    {
    }
}

public class ConstantMethodRefItem : ConstantMemberRefItem
{
    public override ConstantPoolItemType Type => ConstantPoolItemType.MethodRef;

    public ConstantMethodRefItem (ConstantClassItem @class, ConstantNameAndTypeItem nameAndType) : base (@class, nameAndType)
    {
    }
}

public class ConstantInterfaceMethodRefItem : ConstantMemberRefItem
{
    public override ConstantPoolItemType Type => ConstantPoolItemType.InterfaceMethodRef;

    public ConstantInterfaceMethodRefItem (ConstantClassItem @class, ConstantNameAndTypeItem nameAndType) : base (@class, nameAndType)
    {
    }
}

public class ConstantStringItem : ConstantValueItem
{
    public string Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.String;

    public ConstantStringItem (string value)
    {
        Value = value;
    }
}

public class ConstantIntegerItem : ConstantValueItem
{
    public int Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Integer;

    public ConstantIntegerItem (int value)
    {
        Value = value;
    }
}

public class ConstantFloatItem : ConstantValueItem
{
    public float Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Float;

    public ConstantFloatItem (float value)
    {
        Value = value;
    }
}

public class ConstantLongItem : ConstantValueItem
{
    public long Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Long;

    public ConstantLongItem (long value)
    {
        Value = value;
    }
}

public class ConstantDoubleItem : ConstantValueItem
{
    public double Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Double;

    public ConstantDoubleItem (double value)
    {
        Value = value;
    }
}

public class ConstantNameAndTypeItem : ConstantValueItem
{
    public string Name { get; set; }
    public string Descriptor { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.NameAndType;

    public ConstantNameAndTypeItem (string name, string descriptor)
    {
        Name = name;
        Descriptor = descriptor;
    }
}

public class ConstantUtf8Item : ConstantValueItem
{
    public string Value { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Utf8;

    public ConstantUtf8Item (string value)
    {
        Value = value;
    }
}

public class ConstantMethodHandleItem : ConstantValueItem
{
    public byte ReferenceKind { get; set; }
    public ushort ReferenceIndex { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.MethodHandle;

    public ConstantMethodHandleItem (byte referenceKind, ushort referenceIndex)
    {
        ReferenceKind = referenceKind;
        ReferenceIndex = referenceIndex;
    }
}

public class ConstantMethodTypeItem : ConstantValueItem
{
    public string Descriptor { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.MethodType;

    public ConstantMethodTypeItem (string descriptor)
    {
        Descriptor = descriptor;
    }
}

public class ConstantDynamicItem : ConstantValueItem
{
    public ushort BootstrapMethodAttrIndex { get; set; }
    public ConstantNameAndTypeItem NameAndType { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Dynamic;

    public ConstantDynamicItem (ushort bootstrapMethodAttrIndex, ConstantNameAndTypeItem nameAndType)
    {
        BootstrapMethodAttrIndex = bootstrapMethodAttrIndex;
        NameAndType = nameAndType;
    }
}

public class ConstantInvokeDynamicItem : ConstantValueItem
{
    public ushort BootstrapMethodAttrIndex { get; set; }
    public ConstantNameAndTypeItem NameAndType { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.InvokeDynamic;

    public ConstantInvokeDynamicItem (ushort bootstrapMethodAttrIndex, ConstantNameAndTypeItem nameAndType)
    {
        BootstrapMethodAttrIndex = bootstrapMethodAttrIndex;
        NameAndType = nameAndType;
    }
}

public class ConstantModuleItem : ConstantValueItem
{
    public string Name { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Module;

    public ConstantModuleItem (string name)
    {
        Name = name;
    }
}

public class ConstantPackageItem : ConstantValueItem
{
    public string Name { get; set; }

    public override ConstantPoolItemType Type => ConstantPoolItemType.Package;

    public ConstantPackageItem (string name)
    {
        Name = name;
    }
}

public enum ConstantPoolItemType
{
    Utf8 = 1,
    Integer = 3,
    Float = 4,
    Long = 5,
    Double = 6,
    Class = 7,
    String = 8,
    FieldRef = 9,
    MethodRef = 10,
    InterfaceMethodRef = 11,
    NameAndType = 12,
    MethodHandle = 15,
    MethodType = 16,
    Dynamic = 17,
    InvokeDynamic = 18,
    Module = 19,
    Package = 20,
}
