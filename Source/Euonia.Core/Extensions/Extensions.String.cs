using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public static partial class Extensions
{
    /// <summary>
    /// Adds a char to end of given string if it does not ends with the char.
    /// </summary>
    public static string EnsureEndsWith(this string source, char c, StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.EnsureNotNull(source, nameof(source));

        if (source.EndsWith(c.ToString(), comparisonType))
        {
            return source;
        }

        return source + c;
    }

    /// <summary>
    /// Adds a char to beginning of given string if it does not starts with the char.
    /// </summary>
    public static string EnsureStartsWith(this string source, char c, StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.EnsureNotNull(source, nameof(source));

        if (source.StartsWith(c.ToString(), comparisonType))
        {
            return source;
        }

        return c + source;
    }

    /// <summary>
    /// Indicates whether this string is null or an System.String.Empty string.
    /// </summary>
    public static bool IsNullOrEmpty(this string source)
    {
        return string.IsNullOrEmpty(source);
    }

    /// <summary>
    /// indicates whether this string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="length"/> is bigger that string's length</exception>
    public static string Left(this string source, int length)
    {
        Check.EnsureNotNull(source, nameof(source));

        if (source.Length < length)
        {
            throw new ArgumentException("length argument can not be bigger than given string's length!");
        }

        return source[..length];
    }

    /// <summary>
    /// Gets a substring of a string from end of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="length"/> is bigger that string's length</exception>
    public static string Right(this string source, int length)
    {
        Check.EnsureNotNull(source, nameof(source));

        if (source.Length < length)
        {
            throw new ArgumentException("length argument can not be bigger than given string's length!");
        }

        return source.Substring(source.Length - length, length);
    }

    /// <summary>
    /// Converts line endings in the string to <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string NormalizeLineEndings(this string source)
    {
        return source.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
    }

    /// <summary>
    /// Gets index of nth occurrence of a char in a string.
    /// </summary>
    /// <param name="source">source string to be searched</param>
    /// <param name="c">Char to search in source string.</param>
    /// <param name="n">Count of the occurrence</param>
    public static int NthIndexOf(this string source, char c, int n)
    {
        Check.EnsureNotNull(source, nameof(source));

        var count = 0;
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] != c)
            {
                continue;
            }

            if ((++count) == n)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes first occurrence of the given postfixes from end of the given string.
    /// </summary>
    /// <param name="source">The string.</param>
    /// <param name="postFixes">one or more postfix.</param>
    /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
    public static string RemovePostFix(this string source, params string[] postFixes)
    {
        return source.RemovePostFix(StringComparison.Ordinal, postFixes);
    }

    /// <summary>
    /// Removes first occurrence of the given postfixes from end of the given string.
    /// </summary>
    /// <param name="source">The string.</param>
    /// <param name="comparisonType">String comparison type</param>
    /// <param name="postFixes">one or more postfix.</param>
    /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
    public static string RemovePostFix(this string source, StringComparison comparisonType, params string[] postFixes)
    {
        if (source.IsNullOrEmpty())
        {
            return null;
        }

        if (postFixes.IsNullOrEmpty())
        {
            return source;
        }

        foreach (var postFix in postFixes)
        {
            if (source.EndsWith(postFix, comparisonType))
            {
                return source.Left(source.Length - postFix.Length);
            }
        }

        return source;
    }

    /// <summary>
    /// Removes first occurrence of the given prefixes from beginning of the given string.
    /// </summary>
    /// <param name="source">The string.</param>
    /// <param name="preFixes">one or more prefix.</param>
    /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
    public static string RemovePreFix(this string source, params string[] preFixes)
    {
        return source.RemovePreFix(StringComparison.Ordinal, preFixes);
    }

    /// <summary>
    /// Removes first occurrence of the given prefixes from beginning of the given string.
    /// </summary>
    /// <param name="source">The string.</param>
    /// <param name="comparisonType">String comparison type</param>
    /// <param name="preFixes">one or more prefix.</param>
    /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
    public static string RemovePreFix(this string source, StringComparison comparisonType, params string[] preFixes)
    {
        if (source.IsNullOrEmpty())
        {
            return null;
        }

        if (preFixes.IsNullOrEmpty())
        {
            return source;
        }

        foreach (var preFix in preFixes)
        {
            if (source.StartsWith(preFix, comparisonType))
            {
                return source.Right(source.Length - preFix.Length);
            }
        }

        return source;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="search"></param>
    /// <param name="replace"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    public static string ReplaceFirst(this string source, string search, string replace, StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.EnsureNotNull(source, nameof(source));

        var pos = source.IndexOf(search, comparisonType);
        if (pos < 0)
        {
            return source;
        }

        return source.Substring(0, pos) + replace + source.Substring(pos + search.Length);
    }

    /// <summary>
    /// Uses string.Split method to split given string by given separator.
    /// </summary>
    public static string[] Split(this string source, string separator)
    {
        return source.Split(new[] { separator }, StringSplitOptions.None);
    }

    /// <summary>
    /// Uses string.Split method to split given string by given separator.
    /// </summary>
    public static string[] Split(this string source, string separator, StringSplitOptions options)
    {
        return source.Split(new[] { separator }, options);
    }

    /// <summary>
    /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string source)
    {
        return source.Split(Environment.NewLine);
    }

    /// <summary>
    /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string source, StringSplitOptions options)
    {
        return source.Split(Environment.NewLine, options);
    }

    /// <summary>
    /// Converts PascalCase string to camelCase string.
    /// </summary>
    /// <param name="source">String to convert</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    /// <returns>camelCase of the string</returns>
    public static string ToCamelCase(this string source, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        if (source.Length == 1)
        {
            return useCurrentCulture ? source.ToLower() : source.ToLowerInvariant();
        }

        return (useCurrentCulture ? char.ToLower(source[0]) : char.ToLowerInvariant(source[0])) + source[1..];
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
    /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
    /// </summary>
    /// <param name="source">String to convert.</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    public static string ToSentenceCase(this string source, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        return useCurrentCulture
            ? Regex.Replace(source, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]))
            : Regex.Replace(source, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to kebab-case.
    /// </summary>
    /// <param name="source">String to convert.</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    public static string ToKebabCase(this string source, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        source = source.ToCamelCase();

        return useCurrentCulture
            ? Regex.Replace(source, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLower(m.Value[1]))
            : Regex.Replace(source, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1]));
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to snake case.
    /// Example: "ThisIsSampleSentence" is converted to "this_is_a_sample_sentence".
    /// https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NameTranslation/NpgsqlSnakeCaseNameTranslator.cs#L51
    /// </summary>
    /// <param name="source">String to convert.</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        var builder = new StringBuilder(source.Length + Math.Min(2, source.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var currentIndex = 0; currentIndex < source.Length; currentIndex++)
        {
            var currentChar = source[currentIndex];
            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber &&
                        previousCategory != null &&
                        currentIndex > 0 &&
                        currentIndex + 1 < source.Length &&
                        char.IsLower(source[currentIndex + 1]))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }

                    break;

                default:
                    if (previousCategory != null)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }

                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts string to enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">String value to convert</param>
    /// <returns>Returns enum object</returns>
    public static T ToEnum<T>(this string value)
        where T : struct
    {
        Check.EnsureNotNull(value, nameof(value));
        return (T)Enum.Parse(typeof(T), value);
    }

    /// <summary>
    /// Converts string to enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">String value to convert</param>
    /// <param name="ignoreCase">Ignore case</param>
    /// <returns>Returns enum object</returns>
    public static T ToEnum<T>(this string value, bool ignoreCase)
        where T : struct
    {
        Check.EnsureNotNull(value, nameof(value));
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToMd5(this string value)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Converts camelCase string to PascalCase string.
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    /// <returns>PascalCase of the string</returns>
    public static string ToPascalCase(this string value, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (value.Length == 1)
        {
            return useCurrentCulture ? value.ToUpper() : value.ToUpperInvariant();
        }

        return (useCurrentCulture ? char.ToUpper(value[0]) : char.ToUpperInvariant(value[0])) + value.Substring(1);
    }

    // /// <summary>
    // /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    // /// </summary>
    // public static string Truncate(this string source, int maxLength)
    // {
    //     if (source == null)
    //     {
    //         return null;
    //     }
    //
    //     if (source.Length <= maxLength)
    //     {
    //         return source;
    //     }
    //
    //     return source.Left(maxLength);
    // }

    /// <summary>
    /// Gets a substring of a string from Ending of the string if it exceeds maximum length.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
    public static string TruncateFromBeginning(this string source, int maxLength)
    {
        if (source == null)
        {
            return null;
        }

        if (source.Length <= maxLength)
        {
            return source;
        }

        return source.Right(maxLength);
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    /// It adds a "..." postfix to end of the string if it's truncated.
    /// Returning string can not be longer than maxLength.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
    public static string TruncateWithPostfix(this string source, int maxLength)
    {
        // ReSharper disable once IntroduceOptionalParameters.Global
        return TruncateWithPostfix(source, maxLength, "...");
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    /// It adds given <paramref name="postfix"/> to end of the string if it's truncated.
    /// Returning string can not be longer than maxLength.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
    public static string TruncateWithPostfix(this string source, int maxLength, string postfix)
    {
        if (source == null)
        {
            return null;
        }

        if (source == string.Empty || maxLength == 0)
        {
            return string.Empty;
        }

        if (source.Length <= maxLength)
        {
            return source;
        }

        if (maxLength <= postfix.Length)
        {
            return postfix.Left(maxLength);
        }

        return source.Left(maxLength - postfix.Length) + postfix;
    }

    /// <summary>
    /// Converts given string to a byte array using <see cref="Encoding.UTF8"/> encoding.
    /// </summary>
    public static byte[] GetBytes(this string source)
    {
        return source.GetBytes(Encoding.UTF8);
    }

    /// <summary>
    /// Converts given string to a byte array using the given <paramref name="encoding"/>
    /// </summary>
    public static byte[] GetBytes([NotNull] this string source, [NotNull] Encoding encoding)
    {
        Check.EnsureNotNull(source, nameof(source));
        Check.EnsureNotNull(encoding, nameof(encoding));

        return encoding.GetBytes(source);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string SafeSubstring(this string source, int index, int length = 0)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (index < 0 || index > source.Length - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length == 0)
        {
            return source[index..];
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (length > (source.Length - index))
        {
            return source[index..];
        }

        return source.Substring(index, length);
    }

    /// <summary>
    /// Regular expression for matching a phone number.
    /// </summary>
    internal const string PhoneNumberRegex = @"^[+]?(\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

    /// <summary>
    /// Regular expression for matching a string that contains only letters.
    /// </summary>
    internal const string CharactersRegex = "^[A-Za-z]+$";

    /// <summary>
    /// Regular expression for matching an email address.
    /// </summary>
    /// <remarks>General Email Regex (RFC 5322 Official Standard) from https://emailregex.com.</remarks>
    internal const string EmailRegex = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";

    /// <summary>
    /// Regular expression of HTML tags to remove.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private const string REMOVE_HTML_TAGS_REGEX = @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>";

    /// <summary>
    /// Regular expression for removing comments from HTML.
    /// </summary>
    private static readonly Regex _removeHtmlCommentsRegex = new("<!--.*?-->", RegexOptions.Singleline);

    /// <summary>
    /// Regular expression for removing scripts from HTML.
    /// </summary>
    private static readonly Regex _removeHtmlScriptsRegex = new(@"(?s)<script.*?(/>|</script>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    /// <summary>
    /// Regular expression for removing styles from HTML.
    /// </summary>
    private static readonly Regex _removeHtmlStylesRegex = new(@"(?s)<style.*?(/>|</style>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    /// <summary>
    /// Determines whether a string is a valid email address.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns><c>true</c> for a valid email address; otherwise, <c>false</c>.</returns>
    public static bool IsEmail(this string source) => Regex.IsMatch(source, EmailRegex);

    /// <summary>
    /// Determines whether a string is a valid decimal number.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns><c>true</c> for a valid decimal number; otherwise, <c>false</c>.</returns>
    public static bool IsDecimal(this string source) => decimal.TryParse(source, NumberStyles.Number, CultureInfo.InvariantCulture, out _);

    /// <summary>
    /// Determines whether a string is a valid integer.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns><c>true</c> for a valid integer; otherwise, <c>false</c>.</returns>
    public static bool IsNumeric(this string source) => int.TryParse(source, out _);

    /// <summary>
    /// Determines whether a string is a valid phone number.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns><c>true</c> for a valid phone number; otherwise, <c>false</c>.</returns>
    public static bool IsPhoneNumber(this string source) => Regex.IsMatch(source, PhoneNumberRegex);

    /// <summary>
    /// Determines whether a string contains only letters.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns><c>true</c> if the string contains only letters; otherwise, <c>false</c>.</returns>
    public static bool IsCharacterString(this string source) => Regex.IsMatch(source, CharactersRegex);

    /// <summary>
    /// Returns a string with HTML comments, scripts, styles, and tags removed.
    /// </summary>
    /// <param name="htmlText">HTML string.</param>
    /// <returns>Decoded HTML string.</returns>
    public static string DecodeHtml(this string htmlText)
    {
        if (htmlText == null)
        {
            return null;
        }

        var ret = htmlText.FixHtml();

        // Remove html tags
        ret = new Regex(REMOVE_HTML_TAGS_REGEX).Replace(ret, string.Empty);

        return WebUtility.HtmlDecode(ret);
    }

    /// <summary>
    /// Returns a string with HTML comments, scripts, and styles removed.
    /// </summary>
    /// <param name="html">HTML string to fix.</param>
    /// <returns>Fixed HTML string.</returns>
    public static string FixHtml(this string html)
    {
        // Remove comments
        var withoutComments = _removeHtmlCommentsRegex.Replace(html, string.Empty);

        // Remove scripts
        var withoutScripts = _removeHtmlScriptsRegex.Replace(withoutComments, string.Empty);

        // Remove styles
        var withoutStyles = _removeHtmlStylesRegex.Replace(withoutScripts, string.Empty);

        return withoutStyles;
    }

    /// <summary>
    /// Truncates a string to the specified length.
    /// </summary>
    /// <param name="value">The string to be truncated.</param>
    /// <param name="length">The maximum length.</param>
    /// <returns>Truncated string.</returns>
    public static string Truncate(this string value, int length) => Truncate(value, length, false);

    /// <summary>
    /// Provide better linking for resourced strings.
    /// </summary>
    /// <param name="format">The format of the string being linked.</param>
    /// <param name="args">The object which will receive the linked String.</param>
    /// <returns>Truncated string.</returns>
    public static string AsFormat(this string format, params object[] args) => string.Format(format, args);

    /// <summary>
    /// Truncates a string to the specified length.
    /// </summary>
    /// <param name="value">The string to be truncated.</param>
    /// <param name="length">The maximum length.</param>
    /// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
    /// <returns>Truncated string.</returns>
    public static string Truncate(this string value, int length, bool ellipsis)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Trim();
            if (value.Length > length)
            {
                if (ellipsis)
                {
                    return value[..length] + "...";
                }

                return value[..length];
            }
        }

        return value ?? string.Empty;
    }
}
