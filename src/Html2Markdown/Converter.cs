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
						"code", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceCode
							}
					},
					{
						"body", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTagLeaveChildren
							}
					},
					{
						"pre", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplacePre
							}
					},
					{
						"li", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceListItem
							}
					},
					{
						"ol", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceList
							}
					},
					{
						"ul", new CustomReplacer
							{
								CustomAction = HtmlParser.ReplaceList
							}
					},
					{
						"meta", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTag
							}
					},
					{
						"link", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTag
							}
					},
					{
						"title", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTag
							}
					},
					{
						"head", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTagLeaveChildren
							}
					},
					{
						"html", new CustomReplacer
							{
								CustomAction = HtmlParser.RemoveTagLeaveChildren
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
			document.LoadHtml(InitialClean(html));
			var newDocument = new HtmlDocument();

			newDocument.DocumentNode.AppendChild(ParseChildren(document.DocumentNode.ChildNodes).Node);

			return PostClean(newDocument.DocumentNode.InnerHtml);
		}

		private static string InitialClean(string html)
		{
			var commentsRemoved = HtmlParser.RemoveComments(html);
			var doctypeRemoved = HtmlParser.RemoveDoctype(commentsRemoved);

			return doctypeRemoved;
		}

		private static string PostClean(string html)
		{
			var entitiesRemoved = HtmlParser.ReplaceEntities(html);
			var whitespaceRemoved = CleanWhiteSpace(entitiesRemoved);

			return whitespaceRemoved;
		}

		private ParsedNode ParseChildren(IEnumerable<HtmlNode> nodeCollection)
		{
			var newDocument = new HtmlDocument();
			var containsList = false;
			foreach (var node in nodeCollection)
			{
				var parsedChildNode = new ParsedNode();
				if (node.Name.Equals("ol") || node.Name.Equals("ul")) containsList = true;
				if (node.HasChildNodes)
				{
					parsedChildNode = ParseChildren(node.ChildNodes);
					var children = parsedChildNode.Node;
					node.InnerHtml = children.OuterHtml;
				}

				var converted = ParseElement(node, parsedChildNode.ContainsList);
				if (converted != null)
				{
					newDocument.DocumentNode.AppendChild(converted);
				}
			}

			return new ParsedNode
				{
					Node = newDocument.DocumentNode,
					ContainsList = containsList
				};
		}

		private class ParsedNode
		{
			public ParsedNode()
			{
				ContainsList = false;
			}

			public HtmlNode Node { get; set; }

			public bool ContainsList { get; set; }
		}

		private HtmlNode ParseElement(HtmlNode element, bool containsList)
		{
			var tagName = element.Name;
			
			if (HasTagReplacer(tagName))
			{
				var replacer = GetTagReplacer(tagName);
				return HtmlNode.CreateNode(replacer.Replace(element, containsList));
			}
			
			return element;
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
