namespace Html2Markdown.Replacement
{
	public class UnorderedList: IConverter
	{
		public string TagName {
			get {
				return "ul";
			}
		}
	}
}