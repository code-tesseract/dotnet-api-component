using System.Security.Cryptography;
using System.Text;

namespace Component.Helpers;

public static class SecurityHelper
{
	public static string HashPassword(this string password)
	{
		var salt = new byte[16];
		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(salt);
		}

		var hash = GenerateHash(Encoding.UTF8.GetBytes(password), salt, 1000, 32);

		// Combine the salt and hash into a single string
		var hashBytes = new byte[49];
		hashBytes[0] = 0x01; // PBKDF2 with HMAC-SHA1
		Buffer.BlockCopy(salt, 0, hashBytes, 1, 16);
		Buffer.BlockCopy(hash, 0, hashBytes, 17, 32);
		return Convert.ToBase64String(hashBytes);
	}

	public static bool VerifyPassword(this string password, string hashedPassword)
	{
		var hashBytes = Convert.FromBase64String(hashedPassword);

		// Verify the hash algorithm and version
		if (hashBytes[0] != 0x01)
		{
			return false; // Unsupported algorithm or version
		}

		var salt = new byte[16];
		Buffer.BlockCopy(hashBytes, 1, salt, 0, 16);
		var hash = GenerateHash(Encoding.UTF8.GetBytes(password), salt, 1000, 32);

		// Compare the hash with the stored hash
		for (var i = 0; i < 32; i++)
		{
			if (hashBytes[i + 17] != hash[i])
			{
				return false; // Hash mismatch
			}
		}

		return true;
	}

	private static byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length)
	{
		using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
		return pbkdf2.GetBytes(length);
	}
}