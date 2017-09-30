using System;
using AngleSharp.Dom;

namespace Html2Markdown
{
	public class ContentNodeList
	{
		private INodeList _childNodes;

		public ContentNodeList(INodeList childNodes)
		{
			this._childNodes = childNodes;
		}

		public int Size()
		{
			return _childNodes.Length;
		}
	}
}