#nullable enable
namespace System;

/// <summary>
/// The LikeOperator class is used to compare two strings using the * and ? wildcards.
/// </summary>
public class LikeOperator
{
	/// <summary>
	/// Compares two strings using the * and ? wildcards.
	/// </summary>
	public static bool LikeString(string? content, string? pattern, bool ignoreCase = true, bool useInvariantCulture = true)
	{
		if (content == null && pattern == null)
			return true;
		if (content == null || pattern == null)
			return false;

		var patternSpan = pattern.AsSpan();
		var contentSpan = content.AsSpan();

		return LikeString(contentSpan, patternSpan, ignoreCase, useInvariantCulture);
	}

	/// <summary>
	/// Compares two spans using the * and ? wildcards.
	/// </summary>
	public static bool LikeString(ReadOnlySpan<char> contentSpan, ReadOnlySpan<char> patternSpan, bool ignoreCase = true, bool useInvariantCulture = true)
	{
		var zeroOrMoreChars = '*';
		var oneChar = '?';

		if (patternSpan.Length == 1)
		{
			ref readonly char patternItem = ref patternSpan[0];
			if (patternItem == zeroOrMoreChars)
			{
				return true;
			}
		}

		if (contentSpan.Length == 1)
		{
			ref readonly var patternItem = ref patternSpan[0];
			if (patternItem == oneChar)
			{
				return true;
			}
		}

		var zeroOrMorePatternCount = 0;
		var onePatternCount = 0;
		foreach (var @char in patternSpan)
		{
			ref readonly char patternItem = ref @char;
			if (patternItem == zeroOrMoreChars)
			{
				zeroOrMorePatternCount++;
			}
			else if (patternItem == oneChar)
			{
				onePatternCount++;
			}
		}

		if (zeroOrMorePatternCount + onePatternCount == patternSpan.Length)
		{
			if (zeroOrMorePatternCount > 0)
			{
				return true;
			}

			if (patternSpan.Length == contentSpan.Length)
			{
				return true;
			}
		}

		EqualsCharDelegate equalsChar;
		if (ignoreCase)
		{
			if (useInvariantCulture)
			{
				equalsChar = EqualsCharInvariantCultureIgnoreCase;
			}
			else
			{
				equalsChar = EqualsCharCurrentCultureIgnoreCase;
			}
		}
		else
		{
			equalsChar = EqualsChar;
		}

		return LikeStringCore(contentSpan, patternSpan, in zeroOrMoreChars, in oneChar, equalsChar);
	}

	private static bool LikeStringCore(ReadOnlySpan<char> contentSpan, ReadOnlySpan<char> patternSpan, in char zeroOrMoreChars, in char oneChar, EqualsCharDelegate equalsChar)
	{
		var contentIndex = 0;
		var patternIndex = 0;
		while (contentIndex < contentSpan.Length && patternIndex < patternSpan.Length)
		{
			ref readonly var patternItem = ref patternSpan[patternIndex];
			if (patternItem == zeroOrMoreChars)
			{
				while (true)
				{
					if (patternIndex < patternSpan.Length)
					{
						ref readonly char nextPatternItem = ref patternSpan[patternIndex];
						if (nextPatternItem == zeroOrMoreChars)
						{
							patternIndex++;
							continue;
						}
					}

					break;
				}
				
				if (patternIndex == patternSpan.Length)
				{
					return true;
				}
				
				while (contentIndex < contentSpan.Length)
				{
					if (LikeStringCore(contentSpan[contentIndex..], patternSpan[patternIndex..], in zeroOrMoreChars, in oneChar, equalsChar))
					{
						return true;
					}

					contentIndex++;
				}

				return false;
			}

			if (patternItem == oneChar)
			{
				contentIndex++;
				patternIndex++;
			}
			else
			{
				if (contentIndex >= contentSpan.Length)
				{
					return false;
				}

				ref readonly var contentItem = ref contentSpan[contentIndex];
				if (!equalsChar(in contentItem, in patternItem))
				{
					return false;
				}

				contentIndex++;
				patternIndex++;
			}
		}
		
		if (contentIndex == contentSpan.Length)
		{
			while (true)
			{
				if (patternIndex < patternSpan.Length)
				{
					ref readonly char nextPatternItem = ref patternSpan[patternIndex];
					if (nextPatternItem == zeroOrMoreChars)
					{
						patternIndex++;
						continue;
					}
				}

				break;
			}

			return patternIndex == patternSpan.Length;
		}

		return false;
	}

	private static bool EqualsChar(in char contentItem, in char patternItem)
	{
		return contentItem == patternItem;
	}

	private static bool EqualsCharCurrentCultureIgnoreCase(in char contentItem, in char patternItem)
	{
		return char.ToUpper(contentItem) == char.ToUpper(patternItem);
	}

	private static bool EqualsCharInvariantCultureIgnoreCase(in char contentItem, in char patternItem)
	{
		return char.ToUpperInvariant(contentItem) == char.ToUpperInvariant(patternItem);
	}

	private delegate bool EqualsCharDelegate(in char contentItem, in char patternItem);
}