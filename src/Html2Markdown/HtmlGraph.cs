using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace Html2Markdown {
	public class HtmlGraph : IContentGraph
	{
		private IHtmlDocument _document;

		public HtmlGraph(string html)
		{
			var parser = new HtmlParser();
			this._document = parser.Parse(html);

			if (HasChildren()) {
				ChildNodes = new ContentNodeList(_document.Body.ChildNodes);
			}
		}

		public ContentNodeList ChildNodes { get; private set; }
		public bool HasChildren()
		{
			return _document.Body.HasChildNodes;
		}
	}
}