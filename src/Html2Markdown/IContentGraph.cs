namespace Html2Markdown {
	public interface IContentGraph
	{
		ContentNodeList ChildNodes { get; }

		bool HasChildren();
	}
}