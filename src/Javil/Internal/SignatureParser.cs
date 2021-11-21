using Javil.Extensions;

namespace Javil.Internal;

// Internal for unit tests
internal class SignatureParser
{
    private string text;
    private int total;
    private int curr = 0;
    private char[] primitive_types = new[] { 'B', 'C', 'D', 'F', 'I', 'J', 'S', 'V', 'Z' };

    private SignatureParser (string text)
    {
        this.text = text;
        total = text.Length;
    }

    public static TypeSignature Parse (string s)
    {
        var parser = new SignatureParser (s);
        var sig = parser.ParseTypeSignature ();

        System.Diagnostics.Debug.Assert (sig.ToString () == s, $"Could not round trip TypeSignature '{s}' - '{sig}'");

        return sig;
    }

    // [[Landroid/os/MyClass<Ljava/lang/Object;>$AsyncTask<Ljava/lang/Void$NestedVoid;Ljava/lang/Void;Ljava/lang/Void;>$MyNestedClass;
    // - built-in type
    // - array of built in type
    // - complex type
    // - array of complex type
    private TypeSignature ParseTypeSignature ()
    {
        var array_rank = 0;
        var is_generic_parameter = false;
        var wildcard_bounds = string.Empty;
        var wildcard_indicator = string.Empty;

        if (AtWildcardBounds)
            wildcard_bounds = ConsumeChar ().ToString ();

        while (AtArrayStart) {
            array_rank++;
            Advance ();
        }

        if (text == "*" || text == "**")
            return new TypeSignature (string.Empty, text);

        if (AtPrimitiveType)
            return new TypeSignature (string.Empty, ConsumePrimitiveType ()) {
                ArrayRank = array_rank,
                IsPrimitiveType = true,
                WildcardBounds = wildcard_bounds
            };

        if (AtWildcardIndicator)
            wildcard_indicator = ConsumeChar ().ToString ();

        // L or T
        if (ConsumeChar () == 'T')
            is_generic_parameter = true;

        // Strip trailing semicolon
        total--;

        var temp_start = curr;
        var types = new List<string> ();
        var namespaces = new List<string> ();

        while (true) {
            if (AtNamespaceSeparator) {
                namespaces.Add (text.Substring (temp_start, curr - temp_start));
                Advance ();
                temp_start = curr;
                continue;
            }

            if (AtNestedTypeSeparator) {
                types.Add (text.Substring (temp_start, curr - temp_start));
                Advance ();
                temp_start = curr;
                continue;
            }

            if (AtGenericStart) {
                ConsumeGenericTypes ();
                continue;
            }

            if (AtEOF) {
                types.Add (text.Substring (temp_start, curr - temp_start));
                Advance ();
                temp_start = curr;
                break;
            }

            Advance ();
        }

        var name = types.First ();
        var generics = string.Empty;

        if (name.Contains ('<')) {
            generics = name.Substring (name.IndexOf ('<'));
            name = name.Substring (0, name.IndexOf ('<'));
        }

        var signature = new TypeSignature (namespaces.Any () ? string.Join ('.', namespaces) : string.Empty, name) {
            IsGenericParameter = is_generic_parameter,
            WildcardBounds = wildcard_bounds,
            WildcardIndicator = wildcard_indicator
        };

        if (generics.HasValue ())
            foreach (var g in ParseGenericList (generics))
                signature.GenericArguments.Add (TypeSignature.Parse (g));

        var parent = signature;

        foreach (var nested in types.Skip (1)) {
            var nested_sig = TypeSignature.Parse ("L" + nested + ";");
            parent.NestedType = nested_sig;
            parent = nested_sig;
        }

        // If there's a nested type, that's what's actually the array
        if (array_rank > 0)
            signature.GetMostNestedType ().ArrayRank = array_rank;

        return signature;
    }

    private string ConsumePrimitiveType ()
    {
        var type = CurrentChar;
        Advance ();

        return type.ToString ();
    }

    // Internal for testing
    internal static string ConsumeType (string s)
    {
        var parser = new SignatureParser (s);
        return parser.ConsumeType ();
    }

    private static Collection<string> ParseGenericList (string s)
    {
        var parser = new SignatureParser (s);
        return parser.ParseGenericList ();
    }

    private string ConsumeGenericTypes ()
    {
        var start = curr;
        var depth = 0;

        while (true) {
            if (CurrentChar == '<') {
                depth++;
                Advance ();
                continue;
            }

            if (CurrentChar == '>') {
                depth--;
                Advance ();

                if (depth == 0)
                    return text.Substring (start, curr - start);

                continue;
            }

            Advance ();
        }
    }

    private void Advance (int count = 1) => curr += count;

    private bool AtArrayStart => CurrentChar == '[';

    private bool AtPrimitiveType => primitive_types.Contains (CurrentChar);

    private bool AtObjectType => CurrentChar.In ('L', 'T');

    private bool AtGenericStart => CurrentChar == '<';

    private bool AtGenericEnd => CurrentChar == '>';

    private bool AtWildcardBounds => CurrentChar.In ('+', '-');

    private bool AtWildcardIndicator => CurrentChar.In ('*');

    private bool AtEOF => curr == total;

    private char CurrentChar => text[curr];

    private char PeekChar => AtEOF ? '\0' : text[curr + 1];

    private char ConsumeChar () => text[curr++];

    private Collection<string> ParseGenericList ()
    {
        // <
        Advance ();

        // Trailing >
        total--;

        var result = new Collection<string> ();

        while (!AtEOF)
            result.Add (ConsumeType ());

        return result;
    }

    private string ConsumeObjectType ()
    {
        var start = curr;

        while (AtWildcardIndicator)
            ConsumeChar ();

        // L or T
        if (AtObjectType)
            ConsumeChar ();

        while (!AtEOF) {

            if (AtGenericStart) {
                ConsumeGenericTypes ();
                continue;
            }

            if (CurrentChar == ';') {
                Advance ();
                break;
            }

            Advance ();
        }

        return text.Substring (start, curr - start);
    }

    private string ConsumeType ()
    {
        var start = curr;

        if (AtWildcardIndicator) {
            ConsumeChar ();

            if (CurrentChar == '*')
                ConsumeChar ();

            return text.Substring (start, curr - start);
        }

        //if (AtWildcardIndicator)
        //    ConsumeChar ();

        while (AtArrayStart)
            Advance ();

        if (AtPrimitiveType)
            ConsumePrimitiveType ();
        else
            ConsumeObjectType ();

        return text.Substring (start, curr - start);
    }

    private bool AtNamespaceSeparator => CurrentChar == '/';

    private bool AtNestedTypeSeparator => CurrentChar == '$';
}
