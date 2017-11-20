namespace Html2Markdown.Replacement
{
	public class Image: IConverter
	{
		public string TagName {
			get {
				return "img";
			}
		}
	}
}