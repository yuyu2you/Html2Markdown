using System;
using NUnit.Framework;

namespace Html2Markdown {

	[TestFixture]
	public class HtmlContentGraphTest
	{
		[Test]
		public void HtmlContentGraph_ImplementsIContentGraph() {
			var graph = CreateGraph("");

			Assert.That(graph, Is.InstanceOf<IContentGraph>());
		}

		[Test]
		public void HasChildren_WhenHtmlLoadedThatHasChildren_ThenShouldReturnTrue() {
			const string html = @"<p>some content</p>";
			var graph = CreateGraph(html);

			Assert.That(graph.HasChildren(), Is.True);
		}

		[Test]
		public void ChildNodes_WhenHtmlLoadedHasChildren_ThenShouldReturnNodes() {
			const string html = @"<p>some content</p>";
			var graph = CreateGraph(html);

			Assert.That(graph.ChildNodes, Is.InstanceOf<ContentNodeList>());
		}

		[Test]
		public void ChildNodes_WhenHtmlLoadedHasOneChild_ThenChildNodesSizeShouldBeOne() {
			const string html = @"<p>some content</p>";
			var graph = CreateGraph(html);

			Assert.That(graph.ChildNodes.Size(), Is.EqualTo(1));
		}

		[Test]
		public void ChildNodes_WhenHtmlLoadedHasTwoChild_ThenChildNodesSizeShouldBeTwo() {
			const string html = @"<p>some content</p><p>some more content</p>";
			var graph = CreateGraph(html);

			Assert.That(graph.ChildNodes.Size(), Is.EqualTo(2));
		}

		private IContentGraph CreateGraph(string html)
		{
			return new HtmlGraph(html);
		}
	}
}