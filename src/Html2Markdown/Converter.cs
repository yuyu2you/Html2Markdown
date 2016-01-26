using System.Collections.Generic;
using System.IO;
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
						"blockquote", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceBlockquote
							}
					},
					{
						"b", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceStrong
							}
					},
					{
						"strong", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceStrong
							}
					},
					{
						"br", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceBreak
							}
					},
					{
						"em", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceEmphasis
							}
					},
					{
						"i", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceEmphasis
							}
					},
					{
						"h1", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"h2", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"h3", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"h4", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"h5", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"h6", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceHeading
							}
					},
					{
						"hr", new CustomReplacer
							{
								CustomAction = HtmlParser.HorizontalRule
							}
					},
					{
						"a", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceAnchor
							}
					},
					{
						"img", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceImg
							}
					},
					{
						"p", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceParagraph
							}
					},
					{
						"body", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceBody
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
			var document = new HtmlDocument();
			document.LoadHtml(html);
			var newDocument = new HtmlDocument();

			foreach (var node in document.DocumentNode.ChildNodes)
			{
				newDocument.DocumentNode.AppendChild(HtmlNode.CreateNode(ParseElement(node)));
			}

			return CleanWhiteSpace(newDocument.DocumentNode.InnerHtml);
		}

		private string ParseElement(HtmlNode element)
		{
			var tagName = element.Name;
			
			if (HasTagReplacer(tagName))
			{
				var replacer = GetTagReplacer(tagName);
				return replacer.Replace(element);
			}
			
			return element.OuterHtml;
		}

		private IReplacer GetTagReplacer(string tagName)
		{
			return _newReplacers[tagName];
		}

		private bool HasTagReplacer(string tagName)
		{
			return _newReplacers.ContainsKey(tagName);
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
