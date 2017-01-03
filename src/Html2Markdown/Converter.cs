using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Html2Markdown.Replacement;
using HtmlAgilityPack;

namespace Html2Markdown
{
	/// <summary>
	/// An Html to Markdown converter.
	/// </summary>
	public class Converter
	{
		private readonly IDictionary<string, IReplacer> _newReplacers = new Dictionary<string, IReplacer>
			{
				{
					"strong", new CustomReplacer
						{
							CustomAction = HtmlParser.ReplaceStrong
						}
				}
			};

		/// <summary>
		/// Converts Html contained in a file to a Markdown string
		/// </summary>
		/// <param name="path">The path to the file which is being converted</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string ConvertFile(string path)
		{
			using (var reader = new StreamReader(path))
			{
				var html = reader.ReadToEnd();
				html = StandardiseWhitespace(html);
				return Convert(html);
			}
		}

		private static string StandardiseWhitespace(string html)
		{
			return Regex.Replace(html, @"([^\r])\n", "$1\r\n");
		}

		/// <summary>
		/// Converts an Html string to a Markdown string
		/// </summary>
		/// <param name="html">The Html string you wish to convert</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string Convert(string html)
		{
			var initialDocument = new HtmlDocument();
			initialDocument.LoadHtml(html);
			var initialDocNode = initialDocument.DocumentNode;
			var parsedDocument = new HtmlDocument();

			ParseChildren(initialDocNode, parsedDocument.DocumentNode, 0);

			return parsedDocument.DocumentNode.OuterHtml;
		}

		private HtmlNodeCollection ParseChildren(HtmlNode node, HtmlNode parsedDocument, int index)
		{
			var parentNode = new HtmlNode(node.NodeType, parsedDocument.ParentNode, index);
			var nodeCollection = new HtmlNodeCollection(parentNode);
			if (node.HasChildNodes)
			{
				foreach (var child in node.ChildNodes)
				{
					var parsedChildren = ParseChildren(child, parsedDocument, 0);
				}
			}
			else
			{
				var parsedNode = ParseNode(node, parsedDocument);
			}

			return nodeCollection;
		}

		private HtmlNode ParseNode(HtmlNode node, HtmlDocument parsedDocument)
		{
			return null;
		}

		private static string CleanWhiteSpace(string markdown)
		{
			var cleaned = Regex.Replace(markdown, @"\r\n\s+\r\n", "\r\n\r\n");
			cleaned = Regex.Replace(cleaned, @"(\r\n){3,}", "\r\n\r\n");
			cleaned = Regex.Replace(cleaned, @"(> \r\n){2,}", "> \r\n");
			cleaned = Regex.Replace(cleaned, @"^(\r\n)+", "");
			cleaned = Regex.Replace(cleaned, @"(\r\n)+$", "");
			return cleaned;
		}
	}
}
