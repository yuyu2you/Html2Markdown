using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Html2Markdown.Replacement;
using Html2Markdown.Scheme;

namespace Html2Markdown
{
	/// <summary>
	/// An Html to Markdown converter.
	/// </summary>
	public class Converter
	{
		private readonly IList<IReplacer> _replacers;
		
		/// <summary>
		/// Create a Converter with the standard Markdown conversion scheme
		/// </summary>
		public Converter() {
			_replacers = new Markdown().Replacers();
		}

		/// <summary>
		/// Create a converter with a custom conversion scheme
		/// </summary>
		/// <param name="scheme">Conversion scheme to control conversion</param>
		public Converter(IScheme scheme)
		{
			_replacers = scheme.Replacers();
		}

		/// <summary>
		/// Converts Html contained in a file to a Markdown string
		/// </summary>
		/// <param name="path">The path to the file which is being converted</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string ConvertFile(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open)) {
				using (var reader = new StreamReader(stream))
				{
					var html = reader.ReadToEnd();
					return Convert(html);
				}
			}
		}

		/// <summary>
		/// Converts an Html string to a Markdown string
		/// </summary>
		/// <param name="html">The Html string you wish to convert</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string Convert(string html)
		{
			return CleanWhiteSpace(_replacers.Aggregate(html, (current, element) => element.Replace(current)));
		}

		private static string CleanWhiteSpace(string markdown)
		{
			var oneNewLine = Environment.NewLine;
			var twoNewLines = Environment.NewLine + Environment.NewLine;
			var cleaned = Regex.Replace(markdown, @"\r\n\s+\r\n", twoNewLines);
			cleaned = Regex.Replace(cleaned, @"(\r\n){3,}", twoNewLines);
			cleaned = Regex.Replace(cleaned, @"(> \r\n){2,}", "> " + oneNewLine);
			cleaned = Regex.Replace(cleaned, @"^(\r\n)+", "");
			cleaned = Regex.Replace(cleaned, @"(\r\n)+$", "");
			return cleaned;
		}
	}
}