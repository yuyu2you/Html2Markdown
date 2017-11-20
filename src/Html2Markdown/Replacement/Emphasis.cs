namespace Html2Markdown.Replacement
{
	public class Emphasis: IConverter
	{
		public string TagName {
			get {
				return "em";
			}
		}
	}
}