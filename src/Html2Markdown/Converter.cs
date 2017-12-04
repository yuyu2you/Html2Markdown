using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Html2Markdown.Scheme;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;

namespace Html2Markdown
{
	/// <summary>
	/// An Html to Markdown converter.
	/// </summary>
	public class Converter
	{
		
		/// <summary>
		/// Create a Converter with the standard Markdown conversion scheme
		/// </summary>
		public Converter() {

		}

		/// <summary>
		/// Create a converter with a custom conversion scheme
		/// </summary>
		/// <param name="scheme">Conversion scheme to control conversion</param>
		public Converter(IScheme scheme)
		{
			
		}

		/// <summary>
		/// Converts Html contained in a file to a Markdown string
		/// </summary>
		/// <param name="path">The path to the file which is being converted</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string ConvertFile(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open)) {
				using (var reader = new StreamReader(stream))
				{
					var html = reader.ReadToEnd();
					return Convert(html);
				}
			}
		}

		/// <summary>
		/// Converts an Html string to a Markdown string
		/// </summary>
		/// <param name="html">The Html string you wish to convert</param>
		/// <returns>A Markdown representation of the passed in Html</returns>
		public string Convert(string html)
		{
			var parser = new HtmlParser();
			var document = parser.Parse(html);
			var body = document.Body;
			ParseChildren(body);

			return html;
		}

		private bool ElementHasChildren(IHtmlElement element)
		{
			return element.HasChildNodes;
		}

		private void ParseChildren(IHtmlElement element)
		{
			
		}
	}
}