namespace Html2Markdown.Replacement
{
	public class Heading1: IConverter
	{
		public string TagName {
			get {
				return "h1";
			}
		}
	}
}