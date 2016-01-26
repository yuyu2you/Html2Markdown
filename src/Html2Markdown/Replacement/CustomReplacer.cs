using System;
using HtmlAgilityPack;

namespace Html2Markdown.Replacement
{
	internal class CustomReplacer : IReplacer
	{
		public string Replace(HtmlNode element)
		{
			return CustomAction.Invoke(element);
		}

		public Func<HtmlNode, string> CustomAction { get; set; }
	}
}
