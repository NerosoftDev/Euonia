using System.Security.Cryptography;

namespace System;

/// <summary>
/// The ULID (Universally Unique Lexicographically Sortable Identifier) generator.
/// </summary>
/// <remarks>
/// ULID is a 128-bit universally unique identifier that is lexicographically sortable and URL-safe.
/// </remarks>
internal static class UlidGenerator
{
	/// <summary>
	/// ULID uses a specific 32-character encoding known as Crockford's Base32, which includes digits and upper-case letters but excludes letters like "I", "L", "O" to avoid confusion with digits.
	/// </summary>
	private const string CROCKFORD_BASE32 = "0123456789ABCDEFGHJKMNPQRSTVWXYZ"; // Base32 alphabet used by ULID
	private static readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

	public static string Generate()
	{
		var timestamp = GetTimestamp(); // 6 bytes timestamp (48 bits)
		var randomBytes = GetRandomBytes(); // 10 bytes random data (80 bits)

		return Encode(timestamp, randomBytes);
	}

	private static byte[] GetTimestamp()
	{
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Get UTC time in milliseconds
		var timestampBytes = BitConverter.GetBytes(timestamp); // Convert to bytes

		if (BitConverter.IsLittleEndian)
			Array.Reverse(timestampBytes); // Ensure big-endian order

		var result = new byte[6]; // We only need 6 bytes for ULID's 48-bit timestamp
		Array.Copy(timestampBytes, 2, result, 0, 6); // Extract the last 6 bytes

		return result;
	}

	private static byte[] GetRandomBytes()
	{
		var randomBytes = new byte[10]; // ULID requires 10 bytes of random data (80 bits)
		_randomNumberGenerator.GetBytes(randomBytes);
		return randomBytes;
	}

	private static string Encode(byte[] timestamp, byte[] randomBytes)
	{
		var ulid = new StringBuilder(26);

		// Convert 48-bit timestamp (6 bytes) into Base32
		var ulidBytes = new byte[16]; // ULID is 16 bytes total
		Array.Copy(timestamp, 0, ulidBytes, 0, 6);
		Array.Copy(randomBytes, 0, ulidBytes, 6, 10);

		foreach (int value in ulidBytes)
		{
			ulid.Append(CROCKFORD_BASE32[(value >> 3) & 0x1F]);
			ulid.Append(CROCKFORD_BASE32[value & 0x1F]);
		}

		return ulid.ToString();
	}
}