using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MarkX.Core
{
	public static class StringExtensions
	{
		public static string StripLastNewline(this string text)
		{
			if (text.LastOrDefault() == '\n')
			{
				return text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public static IEnumerable<string> AddParentheses(this IEnumerable<string> lines)
		{
			return lines.Select(x => "(" + x + ")");
		}

		public static string AddParentheses(this string line)
		{
			return "(" + line + ")";
		}

		public static string HashToVerbatim(this string text)
		{
			return ResourceStrings.VerbatimDirectory + text.Hash().ToLower() + ResourceStrings.Verbatim;
		}

		public static string Hash(this string input)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				return Convert.ToHexString(hashBytes);
			}
		}

		public static string ReplaceNewlines(this string text)
		{
			return text.Replace("\n", " ");
		}

		public static IEnumerable<string> Escape(this string text, MappingSpecification? markup, SpecialCharacterType specialCharacterType, bool includeText,
												 bool parenthesise, bool isInputPercentEncoded)
		{
			var specialCharacters = markup?.SpecialCharacters?
				.Where(x => x.SpecialCharacterTypes.Contains(specialCharacterType));
			var mapped = new List<string>();
			var all = new List<string>();

			if (isInputPercentEncoded)
			{
				text = System.Uri.UnescapeDataString(text);
			}

			foreach (var character in text)
			{
				var mappedCharacter = specialCharacters?.Where(x => x.MarkupName == character).FirstOrDefault()?.MarkdownName;
				if (mappedCharacter != null)
				{
					mapped.Add(parenthesise ? StringExtensions.AddParentheses(mappedCharacter) : mappedCharacter);
					all.Add(parenthesise ? StringExtensions.AddParentheses(mappedCharacter) : mappedCharacter);
				}
				else
				{
					all.Add(character.ToString());
				}
			}
			if (includeText)
			{
				return all;
			}
			return mapped;
		}
	}
}