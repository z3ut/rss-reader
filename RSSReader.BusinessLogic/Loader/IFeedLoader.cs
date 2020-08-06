using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BusinessLogic.Loader
{
	public interface IFeedLoader
	{
		IEnumerable<FeedItem> LoadFeed(string url);
		Task<IEnumerable<FeedItem>> LoadFeedAsync(string url);
	}
}
