using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Html2Markdown.Replacement
{
	internal static class HtmlParser
	{
		private static readonly Regex NoChildren = new Regex(@"<(ul|ol)\b[^>]*>(?:(?!<ul|<ol)[\s\S])*?<\/\1>");

		public static string ReplaceListItem(HtmlNode node)
		{
			var listParent = GetListParent(node);
			var markdown = ParseListItem(listParent, node);

			return MarkdownNode(markdown).OuterHtml;

		}

		private static string ParseListItem(HtmlNode listParent, HtmlNode node)
		{
			var prefix = listParent.Name.Equals("ol") ? "1.  " : "*   ";
			return Environment.NewLine + prefix + node.InnerHtml;
		}

		private static HtmlNode GetListParent(HtmlNode node)
		{
			var parent = node.ParentNode;
			var tagName = parent.Name;
			if (tagName.Equals("ol") || tagName.Equals("ul"))
			{
				return parent;
			}

			return GetListParent(parent);
		}

		public static string ReplaceList(HtmlNode node)
		{
			var tagContents = node.InnerHtml;
			var markdown = Environment.NewLine + tagContents;
			return MarkdownNode(markdown).OuterHtml;
		}

		private static bool HasNoChildLists(string html)
		{
			return NoChildren.Match(html).Success;
		}

		public static string ReplacePre(HtmlNode node)
		{
			
			var tagContents = node.InnerHtml;
			var markdown = ConvertPre(tagContents);

			return MarkdownNode(markdown).OuterHtml;
		}

		private static string ConvertPre(string html)
		{
			var tag = TabsToSpaces(html);
			tag = IndentNewLines(tag);
			return Environment.NewLine + Environment.NewLine + tag + Environment.NewLine;
		}

		private static string IndentNewLines(string tag)
		{
			return tag.Replace(Environment.NewLine, Environment.NewLine + "    ");
		}

		private static string TabsToSpaces(string tag)
		{
			return tag.Replace("\t", "    ");
		}

		internal static string ReplaceImg(HtmlNode node)
		{
			var src = node.Attributes.GetAttributeOrEmpty("src");
			var alt = node.Attributes.GetAttributeOrEmpty("alt");
			var title = node.Attributes.GetAttributeOrEmpty("title");

			var markdown = string.Format(@"![{0}]({1}{2})", alt, src, (title.Length > 0) ? string.Format(" \"{0}\"", title) : "");

			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceAnchor(HtmlNode node)
		{
			var linkText = node.InnerHtml;
			var href = node.Attributes.GetAttributeOrEmpty("href");
			var title = node.Attributes.GetAttributeOrEmpty("title");

			var markdown = "";

			if (!IsEmptyLink(linkText, href))
			{
				markdown = string.Format(@"[{0}]({1}{2})", linkText, href,
											(title.Length > 0) ? string.Format(" \"{0}\"", title) : "");

				markdown = MarkdownNode(markdown).OuterHtml;
			}

			return markdown;
		}

		public static string ReplaceCode(HtmlNode node)
		{
			var content = node.InnerHtml;
			string markdown;
			if (IsSingleLineCodeBlock(node))
			{
				markdown = string.Format("`{0}`", content);
			}
			else
			{
				markdown = IndentLines(content).TrimEnd() + Environment.NewLine + Environment.NewLine;
				markdown = string.Format("{0}    {1}", Environment.NewLine, TabsToSpaces(markdown));
			}

			markdown = MarkdownNode(markdown).OuterHtml;
			return markdown;
		}

		private static bool IsSingleLineCodeBlock(HtmlNode node)
		{
			return !node.InnerHtml.Contains("\n");
		}

		public static string ReplaceBlockquote(HtmlNode node)
		{
			var quote = node.InnerHtml;
			var lines = quote.TrimStart().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var markdown = "";

			lines.ToList().ForEach(line =>
				{
					markdown += string.Format("> {0}{1}", line.TrimEnd(), Environment.NewLine);
				});

			markdown = Regex.Replace(markdown, @"(>\s\r\n)+$", "");

			markdown = Environment.NewLine + Environment.NewLine + markdown + Environment.NewLine + Environment.NewLine;

			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceStrong(HtmlNode node)
		{
			var boldText = node.InnerHtml;
			var markdown = string.Format("**{0}**", boldText);
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceBreak(HtmlNode node)
		{
			var markdown = "  " + Environment.NewLine;
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceEntities(string html)
		{
			return WebUtility.HtmlDecode(html);
		}

		public static string ReplaceEmphasis(HtmlNode node)
		{
			var italicText = node.InnerHtml;
			var markdown = string.Format("*{0}*", italicText);
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceHeading(HtmlNode node)
		{
			var heading = node.InnerHtml;
			var tagName = node.Name;
			var headerNumber = CalculateHeaderNumber(tagName);
			var markdown = Environment.NewLine + Environment.NewLine + string.Format("{0} {1}", new String('#', headerNumber), heading) + Environment.NewLine + Environment.NewLine;
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string HorizontalRule(HtmlNode node)
		{
			var markdown = Environment.NewLine + Environment.NewLine + "* * *" + Environment.NewLine;
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string ReplaceParagraph(HtmlNode node)
		{
			var paragraph = node.InnerHtml;
			var markdown = Environment.NewLine + Environment.NewLine + paragraph + Environment.NewLine;
			return MarkdownNode(markdown).OuterHtml;
		}

		public static string RemoveTagLeaveChildren(HtmlNode node)
		{
			var body = node.InnerHtml;
			return MarkdownNode(body).OuterHtml;
		}

		public static string RemoveTag(HtmlNode node)
		{
			return "";
		}

		public static string RemoveComments(string html)
		{
			var regex = new Regex(@"<!--[^-]+-->");

			return regex.Replace(html, "");
		}

		public static string RemoveDoctype(string html)
		{
			var regex = new Regex(@"<!DOCTYPE[^>]*>");

			return regex.Replace(html, "");
		}

		private static int CalculateHeaderNumber(string tagName)
		{
			return Int32.Parse(tagName.Substring(1));
		}

		private static HtmlNode MarkdownNode(string markdown)
		{
			return HtmlNode.CreateNode(markdown);
		}

		private static bool IsEmptyLink(string linkText, string href)
		{
			var length = linkText.Length + href.Length;
			return length == 0;
		}

		private static HtmlDocument GetHtmlDocument(string html)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			return doc;
		}

		private static void ReplaceNode(HtmlNode node, string markdown)
		{
			var markdownNode = HtmlNode.CreateNode(markdown);
			if (string.IsNullOrEmpty(markdown))
			{
				node.ParentNode.RemoveChild(node);
			}
			else
			{
				node.ParentNode.ReplaceChild(markdownNode.ParentNode, node);
			}
			
		}

		private static string IndentLines(string content)
		{
			var lines = content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			return lines.Aggregate("", (current, line) => current + IndentLine(line));
		}

		private static string IndentLine(string line)
		{
			if (line.Trim().Equals(string.Empty)) return "";
			return line + Environment.NewLine + "    ";
		}

		private static string GetCodeContent(string code)
		{
			var match = Regex.Match(code, @"<code[^>]*?>([^<]*?)</code>");
			var groups = match.Groups;
			return groups[1].Value;
		}
	}
}
