using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

// ReSharper disable MemberCanBePrivate.Global

public static partial class Extensions
{
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
	private const string REMOVE_HTML_TAGS_REGEX = """(?></?\w+)(?>(?:[^>'"]+|'[^']*'|"[^"]*")*)>""";

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

	/// <param name="source">source string to be searched</param>
	extension(string source)
	{
		/// <summary>
		/// Adds a char to end of given string if it does not end with the char.
		/// </summary>
		public string EnsureEndsWith(char c, StringComparison comparisonType = StringComparison.Ordinal)
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
		public string EnsureStartsWith(char c, StringComparison comparisonType = StringComparison.Ordinal)
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
		public bool IsNullOrEmpty()
		{
			return string.IsNullOrEmpty(source);
		}

		/// <summary>
		/// indicates whether this string is null, empty, or consists only of white-space characters.
		/// </summary>
		public bool IsNullOrWhiteSpace()
		{
			return string.IsNullOrWhiteSpace(source);
		}

		/// <summary>
		/// Gets a substring of a string from beginning of the string.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="length"/> is bigger that string's length</exception>
		public string Left(int length)
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
		public string Right(int length)
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
		public string NormalizeLineEndings()
		{
			return source.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
		}

		/// <summary>
		/// Gets index of nth occurrence of a char in a string.
		/// </summary>
		/// <param name="c">Char to search in source string.</param>
		/// <param name="n">Count of the occurrence</param>
		public int NthIndexOf(char c, int n)
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
		/// <param name="postFixes">one or more postfix.</param>
		/// <returns>Modified string or the same string if it has not any of given postfixes</returns>
		public string RemovePostFix(params string[] postFixes)
		{
			return source.RemovePostFix(StringComparison.Ordinal, postFixes);
		}

		/// <summary>
		/// Removes first occurrence of the given postfixes from end of the given string.
		/// </summary>
		/// <param name="comparisonType">String comparison type</param>
		/// <param name="postFixes">one or more postfix.</param>
		/// <returns>Modified string or the same string if it has not any of given postfixes</returns>
		public string RemovePostFix(StringComparison comparisonType, params string[] postFixes)
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
		/// <param name="preFixes">one or more prefix.</param>
		/// <returns>Modified string or the same string if it has not any of given prefixes</returns>
		public string RemovePreFix(params string[] preFixes)
		{
			return source.RemovePreFix(StringComparison.Ordinal, preFixes);
		}

		/// <summary>
		/// Removes first occurrence of the given prefixes from beginning of the given string.
		/// </summary>
		/// <param name="comparisonType">String comparison type</param>
		/// <param name="preFixes">one or more prefix.</param>
		/// <returns>Modified string or the same string if it has not any of given prefixes</returns>
		public string RemovePreFix(StringComparison comparisonType, params string[] preFixes)
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
		/// <param name="search"></param>
		/// <param name="replace"></param>
		/// <param name="comparisonType"></param>
		/// <returns></returns>
		public string ReplaceFirst(string search, string replace, StringComparison comparisonType = StringComparison.Ordinal)
		{
			Check.EnsureNotNull(source, nameof(source));

			var pos = source.IndexOf(search, comparisonType);
			if (pos < 0)
			{
				return source;
			}

			return source[..pos] + replace + source[(pos + search.Length)..];
		}

		/// <summary>
		/// Uses string.Split method to split given string by given separator.
		/// </summary>
		public string[] Split(string separator)
		{
			return source.Split([separator], StringSplitOptions.None);
		}

		/// <summary>
		/// Uses string.Split method to split given string by given separator.
		/// </summary>
		public string[] Split(string separator, StringSplitOptions options)
		{
			return source.Split([separator], options);
		}

		/// <summary>
		/// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
		/// </summary>
		public string[] SplitToLines()
		{
			return source.Split(Environment.NewLine);
		}

		/// <summary>
		/// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
		/// </summary>
		public string[] SplitToLines(StringSplitOptions options)
		{
			return source.Split(Environment.NewLine, options);
		}

		/// <summary>
		/// Converts PascalCase string to camelCase string.
		/// </summary>
		/// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
		/// <returns>camelCase of the string</returns>
		public string ToCamelCase(bool useCurrentCulture = false)
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
		/// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
		public string ToSentenceCase(bool useCurrentCulture = false)
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
		/// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
		public string ToKebabCase(bool useCurrentCulture = false)
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
		/// <returns></returns>
		public string ToSnakeCase()
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
		/// <returns>Returns enum object</returns>
		public T ToEnum<T>()
			where T : struct
		{
			Check.EnsureNotNull(source, nameof(source));
			return (T)Enum.Parse(typeof(T), source);
		}

		/// <summary>
		/// Converts string to enum value.
		/// </summary>
		/// <typeparam name="T">Type of enum</typeparam>
		/// <param name="ignoreCase">Ignore case</param>
		/// <returns>Returns enum object</returns>
		public T ToEnum<T>(bool ignoreCase)
			where T : struct
		{
			Check.EnsureNotNull(source, nameof(source));
			return (T)Enum.Parse(typeof(T), source, ignoreCase);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string ToMd5()
		{
			using var md5 = MD5.Create();
			var inputBytes = Encoding.UTF8.GetBytes(source);
			var hashBytes = md5.ComputeHash(inputBytes);

			var sb = new StringBuilder();
			foreach (var hashByte in hashBytes)
			{
				sb.Append(hashByte.ToString("X2"));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts camelCase string to PascalCase string.
		/// </summary>
		/// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
		/// <returns>PascalCase of the string</returns>
		public string ToPascalCase(bool useCurrentCulture = false)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return source;
			}

			if (source.Length == 1)
			{
				return useCurrentCulture ? source.ToUpper() : source.ToUpperInvariant();
			}

			return (useCurrentCulture ? char.ToUpper(source[0]) : char.ToUpperInvariant(source[0])) + source[1..];
		}

		/// <summary>
		/// Gets a substring of a string from Ending of the string if it exceeds maximum length.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
		public string TruncateFromBeginning(int maxLength)
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
		public string TruncateWithPostfix(int maxLength)
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
		public string TruncateWithPostfix(int maxLength, string postfix)
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
		public byte[] GetBytes()
		{
			return source.GetBytes(Encoding.UTF8);
		}

		/// <summary>
		/// Determines whether a string is a valid email address.
		/// </summary>
		/// <returns><c>true</c> for a valid email address; otherwise, <c>false</c>.</returns>
		public bool IsEmail() => Regex.IsMatch(source, EmailRegex);

		/// <summary>
		/// Determines whether a string is a valid decimal number.
		/// </summary>
		/// <returns><c>true</c> for a valid decimal number; otherwise, <c>false</c>.</returns>
		public bool IsDecimal() => decimal.TryParse(source, NumberStyles.Number, CultureInfo.InvariantCulture, out _);

		/// <summary>
		/// Determines whether a string is a valid integer.
		/// </summary>
		/// <returns><c>true</c> for a valid integer; otherwise, <c>false</c>.</returns>
		public bool IsNumeric() => int.TryParse(source, out _);

		/// <summary>
		/// Determines whether a string is a valid phone number.
		/// </summary>
		/// <returns><c>true</c> for a valid phone number; otherwise, <c>false</c>.</returns>
		public bool IsPhoneNumber() => Regex.IsMatch(source, PhoneNumberRegex);

		/// <summary>
		/// Determines whether a string contains only letters.
		/// </summary>
		/// <returns><c>true</c> if the string contains only letters; otherwise, <c>false</c>.</returns>
		public bool IsCharacterString() => Regex.IsMatch(source, CharactersRegex);

		/// <summary>
		/// Returns a string with HTML comments, scripts, styles, and tags removed.
		/// </summary>
		/// <returns>Decoded HTML string.</returns>
		public string DecodeHtml()
		{
			if (source == null)
			{
				return null;
			}

			var ret = source.FixHtml();

			// Remove html tags
			ret = new Regex(REMOVE_HTML_TAGS_REGEX).Replace(ret, string.Empty);

			return WebUtility.HtmlDecode(ret);
		}

		/// <summary>
		/// Returns a string with HTML comments, scripts, and styles removed.
		/// </summary>
		/// <returns>Fixed HTML string.</returns>
		public string FixHtml()
		{
			// Remove comments
			var withoutComments = _removeHtmlCommentsRegex.Replace(source, string.Empty);

			// Remove scripts
			var withoutScripts = _removeHtmlScriptsRegex.Replace(withoutComments, string.Empty);

			// Remove styles
			var withoutStyles = _removeHtmlStylesRegex.Replace(withoutScripts, string.Empty);

			return withoutStyles;
		}

		/// <summary>
		/// Truncates a string to the specified length.
		/// </summary>
		/// <param name="length">The maximum length.</param>
		/// <returns>Truncated string.</returns>
		public string Truncate(int length) => Truncate(source, length, false);

		/// <summary>
		/// Provide better linking for resourced strings.
		/// </summary>
		/// <param name="args">The object which will receive the linked String.</param>
		/// <returns>Truncated string.</returns>
		public string AsFormat(params object[] args) => string.Format(source, args);

		/// <summary>
		/// Truncates a string to the specified length.
		/// </summary>
		/// <param name="length">The maximum length.</param>
		/// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
		/// <returns>Truncated string.</returns>
		public string Truncate(int length, bool ellipsis)
		{
			if (!string.IsNullOrEmpty(source))
			{
				source = source.Trim();
				if (source.Length > length)
				{
					if (ellipsis)
					{
						return source[..length] + "...";
					}

					return source[..length];
				}
			}

			return source ?? string.Empty;
		}

		/// <summary>
		/// Trim text and return new string.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public string Trim(TextTrimType type)
		{
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}

			return type switch
			{
				TextTrimType.Head => source.TrimStart(),
				TextTrimType.Tail => source.TrimEnd(),
				TextTrimType.Both => source.Trim(),
				TextTrimType.All => Regex.Replace(source, @"\s+", string.Empty),
				TextTrimType.None => source,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		/// <summary>
		/// Normalize text and return new string.
		/// </summary>
		/// <param name="caseType"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public string Normalize(TextCaseType caseType)
		{
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}

			var text = CultureInfo.CurrentCulture.TextInfo;
			return caseType switch
			{
				TextCaseType.Upper => text.ToUpper(source),
				TextCaseType.Lower => text.ToLower(source),
				TextCaseType.Title => text.ToTitleCase(source),
				TextCaseType.None => source,
				_ => throw new ArgumentOutOfRangeException(nameof(caseType), caseType, null)
			};
		}

		/// <summary>
		/// Mask text and return new string.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="maskChar"></param>
		/// <returns></returns>
		public string Mask(int start, int length, char maskChar = '*')
		{
			var end = start + length;
			if (source.Length <= start)
			{
				return string.Empty;
			}

			if (source.Length < end)
			{
				return source[..start] + "".PadLeft(source.Length - start, maskChar);
			}

			return source[..start] + "".PadLeft(length, maskChar) + source[end..];
		}

		/// <summary>
		/// Returns a value if the string is null or white space.
		/// </summary>
		/// <param name="default">The string to return if source string is null or white space.</param>
		/// <returns>The <paramref name="source"/> or <paramref name="default"/>.</returns>
		public string DefaultIfNullOrWhiteSpace([NotNull] string @default)
		{
			return string.IsNullOrWhiteSpace(source) ? @default : source;
		}

		/// <summary>
		/// Encoding the given string using Base64 and returns a new string.
		/// </summary>
		/// <returns></returns>
		public string ToBase64()
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return string.Empty;
			}

			var plainTextBytes = Encoding.UTF8.GetBytes(source);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public string SafeSubstring(int index, int length = 0)
		{
			if (length < 0 || index < 0 || index > source.Length - 1)
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
		/// Converts given string to a byte array using the given <paramref name="encoding"/>
		/// </summary>
		public byte[] GetBytes([NotNull] Encoding encoding)
		{
			Check.EnsureNotNull(source, nameof(source));
			Check.EnsureNotNull(encoding, nameof(encoding));

			return encoding.GetBytes(source);
		}
	}

	/// <summary>
	/// Decoding the given Base64 string and returns a new string.
	/// </summary>
	extension(string)
	{
		/// <summary>
		/// Collapses the given string parts into a single string by returning the first non-null and non-empty string.
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		public static string Collapse(params string[] parts)
		{
			foreach (var part in parts)
			{
				if (!string.IsNullOrEmpty(part))
				{
					return part;
				}
			}

			return string.Empty;
		}
	}
}