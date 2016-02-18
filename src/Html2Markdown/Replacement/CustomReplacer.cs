using System;
using HtmlAgilityPack;

namespace Html2Markdown.Replacement
{
	internal class CustomReplacer : IReplacer
	{
		public Func<HtmlNode, bool, string> CustomAction { get; set; }

		public string Replace(HtmlNode element, bool containsList)
		{
			return CustomAction.Invoke(element, containsList);
		}
	}
}
