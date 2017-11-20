namespace Html2Markdown.Replacement
{
	public class OrderedList: IConverter
	{
		public string TagName {
			get {
				return "ol";
			}
		}
	}
}