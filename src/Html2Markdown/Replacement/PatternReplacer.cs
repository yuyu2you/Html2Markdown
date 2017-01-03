using HtmlAgilityPack;

namespace Html2Markdown.Replacement
{
	internal class PatternReplacer : IReplacer
	{
		public string Pattern { get; set; }

		public string Replacement { get; set; }

		public string Replace(HtmlNode element)
		{
			throw new System.NotImplementedException();
		}
	}
}
