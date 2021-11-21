using NUnit.Framework;

namespace Javil.Tests;

public class TypeReferenceTests
{
    [Test]
    public void BasicType ()
    {
        var type = TypeReference.CreateFromSignature ("Landroid/nfc/NfcAdapter;", DummyContainer);

        Assert.IsInstanceOf<TypeReference> (type);
        Assert.AreEqual ("NfcAdapter", type.Name);
        Assert.AreEqual ("NfcAdapter", type.NestedName);
        Assert.AreEqual ("NfcAdapter", type.JniName);
        Assert.AreEqual ("android.nfc", type.Namespace);
        Assert.AreEqual ("android.nfc.NfcAdapter", type.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter", type.FullNameGenericsErased);
        Assert.AreEqual ("NfcAdapter", type.GenericName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", type.JniFullName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", type.JniFullNameGenericsErased);
        Assert.IsNull (type.DeclaringType);
    }

    [Test]
    public void PrimitiveType ()
    {
        var type = TypeReference.CreateFromSignature ("I", DummyContainer);

        Assert.IsInstanceOf<TypeReference> (type);
        Assert.AreEqual ("int", type.Name);
        Assert.AreEqual ("int", type.NestedName);
        Assert.AreEqual ("I", type.JniName);
        Assert.AreEqual (string.Empty, type.Namespace);
        Assert.AreEqual ("int", type.FullName);
        Assert.AreEqual ("int", type.FullNameGenericsErased);
        Assert.AreEqual ("int", type.GenericName);
        Assert.AreEqual ("I", type.JniFullName);
        Assert.AreEqual ("I", type.JniFullNameGenericsErased);
        Assert.IsNull (type.DeclaringType);
    }

    [Test]
    public void GenericType ()
    {
        var type = TypeReference.CreateFromSignature ("Ljava/util/ArrayList<TT;>;", DummyContainer);

        Assert.AreEqual ("GenericInstanceType", type.GetType ().Name);

        Assert.AreEqual ("ArrayList", type.Name);
        Assert.AreEqual ("ArrayList", type.NestedName);
        Assert.AreEqual ("ArrayList", type.JniName);
        Assert.AreEqual ("java.util", type.Namespace);
        Assert.AreEqual ("java.util.ArrayList<T>", type.FullName);
        Assert.AreEqual ("java.util.ArrayList", type.FullNameGenericsErased);
        Assert.AreEqual ("ArrayList<T>", type.GenericName);
        Assert.AreEqual ("Ljava/util/ArrayList<TT;>;", type.JniFullName);
        Assert.AreEqual ("Ljava/util/ArrayList;", type.JniFullNameGenericsErased);
        Assert.IsNull (type.DeclaringType);

        var gen_inst = (GenericInstanceType)type;

        Assert.AreEqual (1, gen_inst.GenericArguments.Count);

        var ga = gen_inst.GenericArguments[0];

        Assert.AreEqual ("T", ga.Name);
        Assert.AreEqual ("T", ga.NestedName);
        Assert.AreEqual ("T", ga.JniName);
        Assert.AreEqual (string.Empty, ga.Namespace);
        Assert.AreEqual ("T", ga.FullName);
        Assert.AreEqual ("T", ga.FullNameGenericsErased);
        Assert.AreEqual ("T", ga.GenericName);
        Assert.AreEqual ("TT;", ga.JniFullName);
        Assert.AreEqual ("Ljava/lang/Object;", ga.JniFullNameGenericsErased);
        Assert.IsNull (ga.DeclaringType);
    }

    [Test]
    public void NestedType ()
    {
        var type = TypeReference.CreateFromSignature ("Landroid/nfc/NfcAdapter$NestedType;", DummyContainer);

        Assert.IsInstanceOf<TypeReference> (type);
        Assert.AreEqual ("NestedType", type.Name);
        Assert.AreEqual ("NfcAdapter$NestedType", type.NestedName);
        Assert.AreEqual ("NestedType", type.JniName);
        Assert.AreEqual (string.Empty, type.Namespace);
        Assert.AreEqual ("android.nfc.NfcAdapter$NestedType", type.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter$NestedType", type.FullNameGenericsErased);
        Assert.AreEqual ("NestedType", type.GenericName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter$NestedType;", type.JniFullName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter$NestedType;", type.JniFullNameGenericsErased);

        Assert.IsNotNull (type.DeclaringType);
        var declaring = type.DeclaringType!;

        Assert.IsInstanceOf<TypeReference> (declaring);
        Assert.AreEqual ("NfcAdapter", declaring.Name);
        Assert.AreEqual ("NfcAdapter", declaring.NestedName);
        Assert.AreEqual ("NfcAdapter", declaring.JniName);
        Assert.AreEqual ("android.nfc", declaring.Namespace);
        Assert.AreEqual ("android.nfc.NfcAdapter", declaring.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter", declaring.FullNameGenericsErased);
        Assert.AreEqual ("NfcAdapter", declaring.GenericName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", declaring.JniFullName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", declaring.JniFullNameGenericsErased);
        Assert.IsNull (declaring.DeclaringType);
    }

    [Test]
    public void BasicArrayType ()
    {
        var type = TypeReference.CreateFromSignature ("[Landroid/nfc/NfcAdapter;", DummyContainer);

        Assert.IsInstanceOf<ArrayType> (type);
        Assert.AreEqual ("NfcAdapter[]", type.Name);
        Assert.AreEqual ("NfcAdapter[]", type.NestedName);
        Assert.AreEqual ("NfcAdapter", type.JniName);
        Assert.AreEqual ("android.nfc", type.Namespace);
        Assert.AreEqual (1, ((ArrayType)type).Rank);
        Assert.AreEqual ("android.nfc.NfcAdapter[]", type.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter[]", type.FullNameGenericsErased);
        Assert.AreEqual ("NfcAdapter[]", type.GenericName);
        Assert.AreEqual ("[Landroid/nfc/NfcAdapter;", type.JniFullName);
        Assert.AreEqual ("[Landroid/nfc/NfcAdapter;", type.JniFullNameGenericsErased);
        Assert.IsNull (type.DeclaringType);
    }

    [Test]
    public void PrimitiveArrayType ()
    {
        var type = TypeReference.CreateFromSignature ("[I", DummyContainer);

        Assert.IsInstanceOf<TypeReference> (type);
        Assert.AreEqual ("int[]", type.Name);
        Assert.AreEqual ("int[]", type.NestedName);
        Assert.AreEqual ("I", type.JniName);
        Assert.AreEqual (string.Empty, type.Namespace);
        Assert.AreEqual ("int[]", type.FullName);
        Assert.AreEqual ("int[]", type.FullNameGenericsErased);
        Assert.AreEqual ("int[]", type.GenericName);
        Assert.AreEqual ("[I", type.JniFullName);
        Assert.AreEqual ("[I", type.JniFullNameGenericsErased);
        Assert.IsNull (type.DeclaringType);
    }

    [Test]
    public void NestedArrayType ()
    {
        var type = TypeReference.CreateFromSignature ("[Landroid/nfc/NfcAdapter$NestedType;", DummyContainer);

        Assert.IsInstanceOf<TypeReference> (type);
        Assert.AreEqual ("NestedType[]", type.Name);
        Assert.AreEqual ("NfcAdapter$NestedType[]", type.NestedName);
        Assert.AreEqual ("NestedType", type.JniName);
        Assert.AreEqual (string.Empty, type.Namespace);
        Assert.AreEqual (1, ((ArrayType)type).Rank);
        Assert.AreEqual ("android.nfc.NfcAdapter$NestedType[]", type.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter$NestedType[]", type.FullNameGenericsErased);
        Assert.AreEqual ("NestedType[]", type.GenericName);
        Assert.AreEqual ("[Landroid/nfc/NfcAdapter$NestedType;", type.JniFullName);
        Assert.AreEqual ("[Landroid/nfc/NfcAdapter$NestedType;", type.JniFullNameGenericsErased);

        Assert.IsNotNull (type.DeclaringType);
        var declaring = type.DeclaringType!;

        Assert.IsInstanceOf<TypeReference> (declaring);
        Assert.AreEqual ("NfcAdapter", declaring.Name);
        Assert.AreEqual ("NfcAdapter", declaring.NestedName);
        Assert.AreEqual ("NfcAdapter", declaring.JniName);
        Assert.AreEqual ("android.nfc", declaring.Namespace);
        Assert.AreEqual ("android.nfc.NfcAdapter", declaring.FullName);
        Assert.AreEqual ("android.nfc.NfcAdapter", declaring.FullNameGenericsErased);
        Assert.AreEqual ("NfcAdapter", declaring.GenericName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", declaring.JniFullName);
        Assert.AreEqual ("Landroid/nfc/NfcAdapter;", declaring.JniFullNameGenericsErased);
        Assert.IsNull (declaring.DeclaringType);
    }

    [Test]
    public void KitchenSink ()
    {
        var type = TypeReference.CreateFromSignature ("[Landroid/os/AsyncTask<TT;[ILjava/util/ArrayList<TK;>;>$NestedType<Z>;", DummyContainer);

        // Test NestedType[]
        Assert.AreEqual ("ArrayType", type.GetType ().Name);
        var array_type = (ArrayType)type;

        Assert.AreEqual ("NestedType[]", type.Name);
        Assert.AreEqual ("AsyncTask$NestedType[]", type.NestedName);
        Assert.AreEqual ("NestedType", type.JniName);
        Assert.AreEqual (string.Empty, type.Namespace);
        Assert.AreEqual ("android.os.AsyncTask<T, int[], java.util.ArrayList<K>>$NestedType<boolean>[]", type.FullName);
        Assert.AreEqual ("android.os.AsyncTask$NestedType[]", type.FullNameGenericsErased);
        Assert.AreEqual ("NestedType<boolean>[]", type.GenericName);
        Assert.AreEqual ("[Landroid/os/AsyncTask<TT;[ILjava/util/ArrayList<TK;>;>$NestedType<Z>;", type.JniFullName);
        Assert.AreEqual ("[Landroid/os/AsyncTask$NestedType;", type.JniFullNameGenericsErased);

        // Test boolean generic argument in NestedType<Z>[]
        var gen_inst = (GenericInstanceType)array_type.ElementType;
        Assert.AreEqual (1, gen_inst.GenericArguments.Count);

        var ga = gen_inst.GenericArguments[0];

        Assert.AreEqual ("boolean", ga.Name);
        Assert.AreEqual ("boolean", ga.NestedName);
        Assert.AreEqual ("Z", ga.JniName);
        Assert.AreEqual (string.Empty, ga.Namespace);
        Assert.AreEqual ("boolean", ga.FullName);
        Assert.AreEqual ("boolean", ga.FullNameGenericsErased);
        Assert.AreEqual ("boolean", ga.GenericName);
        Assert.AreEqual ("Z", ga.JniFullName);
        Assert.AreEqual ("Z", ga.JniFullNameGenericsErased);
        Assert.IsNull (ga.DeclaringType);

        // Test AsyncTask

        // Test AsyncTask generic parameter 1: 'TT;'

        // Test AsyncTask generic parameter 2: '[I'

        // Test AsyncTask generic parameter 3: 'Ljava/util/ArrayList<TK;>;'

        // Test AsyncTask generic parameter 3's generic parameter: 'TK;'
    }

    [Test]
    public void Hyphen ()
    {
        AssertRoundTrip ("Ljava/lang/Class<+[TT;>;");
        AssertRoundTrip ("Landroid/net/DnsResolver$Callback<-[B>;");
        AssertRoundTrip ("Landroid/net/DnsResolver$Callback<-Ljava/util/List<Ljava/net/InetAddress;>;>;");
        AssertRoundTrip ("Ljava/util/stream/Collector<TT;*Ljava/util/Map<TK;TU;>;>;");
        AssertRoundTrip ("Ljava/util/Collection<+Ljava/security/cert/Certificate;>;");
        AssertRoundTrip ("Ljava/util/Map<**>;");
        AssertRoundTrip ("Ljava/util/function/Function<-TT;+TU;>;");
        AssertRoundTrip ("Ljava/util/stream/Collector<TT;*Ljava/lang/Integer;>;");
    }

    private void AssertRoundTrip (string s)
    {
        var type = TypeReference.CreateFromSignature (s, DummyContainer);
        Assert.AreEqual (s, type.JniFullName);
    }

    private ContainerDefinition DummyContainer { get; } = new ContainerDefinition ("dummy.jar", new BaseContainerResolver ());
}
