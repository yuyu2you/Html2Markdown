namespace Html2Markdown.Replacement
{
	public class Anchor: IConverter
	{
		public string TagName {
			get {
				return "a";
			}
		}
	}
}