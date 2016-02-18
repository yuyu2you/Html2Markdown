using HtmlAgilityPack;

namespace Html2Markdown.Replacement
{
	internal interface IReplacer
	{
		string Replace(HtmlNode element, bool containsList);
	}
}
