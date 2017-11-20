namespace Html2Markdown.Replacement
{
	public class Break: IConverter
	{
		public string TagName {
			get {
				return "br";
			}
		}
	}
}