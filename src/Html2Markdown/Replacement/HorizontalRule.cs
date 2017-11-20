namespace Html2Markdown.Replacement
{
	public class HorizontalRule: IConverter
	{
		public string TagName {
			get {
				return "hr";
			}
		}
	}
}