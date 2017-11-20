namespace Html2Markdown.Replacement
{
	public class Blockquote: IConverter
	{
		public string TagName {
			get {
				return "blockquote";
			}
		}
	}
}