using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Javil.Extensions;

static class StringExtensions
{
    /// <summary>
    /// Shortcut for !string.IsNullOrWhiteSpace (s)
    /// </summary>
    public static bool HasValue ([NotNullWhen (true)] this string? s) => !string.IsNullOrWhiteSpace (s);

    /// <summary>
    /// Removes the first subset of a delimited string. ("127.0.0.1" -> "0.0.1")
    /// </summary>
    [return: NotNullIfNotNull ("s")]
    public static string? ChompFirst (this string? s, char separator)
    {
        if (!s.HasValue ())
            return s;

        var index = s.IndexOf (separator);

        if (index < 0)
            return string.Empty;

        return s.Substring (index + 1);
    }

    /// <summary>
    /// Removes the final subset of a delimited string. ("127.0.0.1" -> "127.0.0")
    /// </summary>
    [return: NotNullIfNotNull ("s")]
    public static string? ChompLast (this string? s, char separator)
    {
        if (!s.HasValue ())
            return s;

        var index = s.LastIndexOf (separator);

        if (index < 0)
            return string.Empty;

        return s.Substring (0, index);
    }

    /// <summary>
    /// Returns the first subset of a delimited string. ("127.0.0.1" -> "127")
    /// </summary>
    [return: NotNullIfNotNull ("s")]
    public static string? FirstSubset (this string? s, char separator)
    {
        if (!s.HasValue ())
            return s;

        var index = s.IndexOf (separator);

        if (index < 0)
            return s;

        return s.Substring (0, index);
    }

    /// <summary>
    /// Returns the final subset of a delimited string. ("127.0.0.1" -> "1")
    /// </summary>
    [return: NotNullIfNotNull ("s")]
    public static string? LastSubset (this string? s, char separator)
    {
        if (!s.HasValue ())
            return s;

        var index = s.LastIndexOf (separator);

        if (index < 0)
            return s;

        return s.Substring (index + 1);
    }

    [return: NotNullIfNotNull ("s")]
    public static string? StripJni (this string? s)
    {
        if (string.IsNullOrWhiteSpace (s))
            return s;

        if (!s.StartsWith ('L') || !s.EndsWith (';'))
            throw new ArgumentException ($"Invalid Jni: '{s}'");

        return s.Substring (1, s.Length - 2);
    }

    public static bool In<T> (this T enumeration, params T[] values)
    {
        if (enumeration is null)
            return false;

        foreach (var en in values)
            if (enumeration.Equals (en))
                return true;

        return false;
    }

    public static string Repeat (this string s, int count)
    {
        if (count == 0)
            return s;

        var sb = new StringBuilder ();

        for (var i = 0; i < count; i++)
            sb.Append (s);

        return sb.ToString ();
    }
}
