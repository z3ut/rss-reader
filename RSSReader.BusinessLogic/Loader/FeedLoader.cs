using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSSReader.BusinessLogic.Loader
{
	public class FeedLoader : IFeedLoader
	{
		public IEnumerable<FeedItem> LoadFeed(string url)
		{
			using (var reader = XmlReader.Create(url))
			{
				var feed = SyndicationFeed.Load(reader);
				return feed.Items?.Select(item => new FeedItem()
				{
					RSSFeedId = item.Id,
					Title = item.Title?.Text,
					Link = item.Links?.First()?.Uri?.ToString(),
					DateTime = item.PublishDate.UtcDateTime
				});
			}
		}

		public Task<IEnumerable<FeedItem>> LoadFeedAsync(string url)
		{
			return Task.FromResult(LoadFeed(url));
		}
	}
}
