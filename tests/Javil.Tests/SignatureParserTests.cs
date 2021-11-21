using NUnit.Framework;
using Javil.Internal;

namespace Javil.Tests;

public class SignatureParserTests
{
    [Test]
    public void ConsumeType ()
    {
        var jni = "Landroid/os/MyClass<Ljava/lang/Object;>$AsyncTask<Ljava/lang/Void$NestedVoid;Ljava/lang/Void;Ljava/lang/Void;>$MyNestedClass;";

        Assert.AreEqual ("I", SignatureParser.ConsumeType ("I"));
        Assert.AreEqual ("I", SignatureParser.ConsumeType ("II"));
        Assert.AreEqual ("[I", SignatureParser.ConsumeType ("[I"));
        Assert.AreEqual ("[I", SignatureParser.ConsumeType ("[II"));
        Assert.AreEqual ("[[I", SignatureParser.ConsumeType ("[[I"));
        Assert.AreEqual ("[[I", SignatureParser.ConsumeType ("[[II"));
        Assert.AreEqual ("I", SignatureParser.ConsumeType ("ILjava/lang/Object;"));
        Assert.AreEqual ("Landroid/os/MyClass;", SignatureParser.ConsumeType ("Landroid/os/MyClass;"));
        Assert.AreEqual ("[Landroid/os/MyClass;", SignatureParser.ConsumeType ("[Landroid/os/MyClass;I"));
        Assert.AreEqual ("Landroid/os/MyClass$NestedClass;", SignatureParser.ConsumeType ("Landroid/os/MyClass$NestedClass;I"));
        Assert.AreEqual ("[Landroid/os/MyClass$NestedClass;", SignatureParser.ConsumeType ("[Landroid/os/MyClass$NestedClass;I"));
        Assert.AreEqual ("Landroid/os/MyClass<Ljava/lang/Object;>;", SignatureParser.ConsumeType ("Landroid/os/MyClass<Ljava/lang/Object;>;I"));
        Assert.AreEqual ("[Landroid/os/MyClass<Ljava/lang/Object;>;", SignatureParser.ConsumeType ("[Landroid/os/MyClass<Ljava/lang/Object;>;I"));
        Assert.AreEqual ("Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass;", SignatureParser.ConsumeType ("Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass;I"));
        Assert.AreEqual ("[Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass;", SignatureParser.ConsumeType ("[Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass;I"));
        Assert.AreEqual ("Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass<I>;", SignatureParser.ConsumeType ("Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass<I>;I"));
        Assert.AreEqual ("[Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass<[I>;", SignatureParser.ConsumeType ("[Landroid/os/MyClass<Ljava/lang/Object;>$NestedClass<[I>;I"));
        Assert.AreEqual (jni, SignatureParser.ConsumeType (jni));
    }

    [Test]
    public void ParseSignature ()
    {
        CheckSignature ("Ljava/util/stream/Collector<TT;*Ljava/util/Map<TK;TU;>;>;");
        CheckSignature ("-[B");
        CheckSignature ("[Landroid/renderscript/RenderScript$Priority;");
        CheckSignature ("*");
        CheckSignature ("+Landroid/app/appsearch/GenericDocument;");
        CheckSignature ("**");
        CheckSignature ("*Ljava/lang/Integer;");
        CheckSignature ("*TC;");
        CheckSignature ("Ljava/util/stream/Collector<TT;*TR;>;");
        CheckSignature ("V");
        CheckSignature ("I");
        CheckSignature ("LOnExitAnimationListener;");
        CheckSignature ("Landroid/window/SplashScreen$OnExitAnimationListener;");
        CheckSignature ("[Landroid/window/SplashScreen;");
        CheckSignature ("Landroid/os/Parcelable$Creator<Landroid/bluetooth/le/AdvertiseSettings;>;");
        CheckSignature ("[B");
        CheckSignature ("Landroid/util/SparseArray<[B>;");
        CheckSignature ("Ljava/util/concurrent/BlockingQueue<TE;>;");
        CheckSignature ("[TT;");
        CheckSignature ("[[Ljava/lang/String;");
        CheckSignature ("Ljava/util/Map<**>;");
        CheckSignature ("Landroid/net/DnsResolver$Callback<-[B>;");
        CheckSignature ("Landroid/net/DnsResolver$Callback<-Ljava/util/List<Ljava/net/InetAddress;>;>;");
        CheckSignature ("Ljava/util/Collection<+Ljava/security/cert/Certificate;>;");
        CheckSignature ("Ljava/util/function/Function<-TT;+TU;>;");
        CheckSignature ("Ljava/util/stream/Collector<TT;*Ljava/lang/Integer;>;");
    }

    private static void CheckSignature (string s)
    {
        var sig = TypeSignature.Parse (s);
        var output = sig.ToString ();

        Assert.AreEqual (s, output);
    }
}
