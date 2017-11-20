namespace Html2Markdown.Replacement
{
	public class Link: IConverter
	{
		public string TagName {
			get {
				return "link";
			}
		}
	}
}