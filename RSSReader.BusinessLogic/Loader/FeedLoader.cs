using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RSSReader.BusinessLogic.Loader
{
	public class FeedLoader : IFeedLoader
	{
		public IEnumerable<FeedItem> LoadFeed(string url)
		{
			using (var reader = XmlReader.Create(url))
			{
				var feed = SyndicationFeed.Load(reader);
				return feed?.Items?.Select(item => new FeedItem()
				{
					RSSFeedId = item.Id,
					Title = item.Title?.Text,
					Link = item.Links?.First()?.Uri?.ToString(),
					DateTime = item.PublishDate.UtcDateTime
				});
			}
		}

		public async Task<IEnumerable<FeedItem>> LoadFeedAsync(string url)
		{
			var feed = await CodeHollow.FeedReader.FeedReader.ReadAsync(url);
			return feed?.Items?.Select(item =>
			{
				string imageUrl = GetDescendantElementWithName(item, "thumbnail")
					?.Attribute("url")?.Value;

				string description = item.Description ??
					GetDescendantElementWithName(item, "description")?.Value;

				return new FeedItem()
				{
					RSSFeedId = item.Id,
					Title = item.Title,
					Link = item.Link,
					DateTime = item.PublishingDate ?? DateTime.UtcNow,
					Description = description,
					ImageUrl = imageUrl
				};
			});
		}

		private XElement GetDescendantElementWithName(
			CodeHollow.FeedReader.FeedItem item, string name)
		{
			return item.SpecificItem.Element.Descendants()
				.FirstOrDefault(x => x.Name.LocalName == name);
		}
	}
}
