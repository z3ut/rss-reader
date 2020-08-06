using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BusinessLogic.Feeds
{
	public interface IFeedService
	{
		FeedItem CreateFeedItem(FeedItem feedItem);
		FeedItem GetFeedItem(int feedItemId);
		FeedItem UpdateFeedItem(FeedItem feedItem);
		void DeleteFeedItem(int feedItemId);

		int GetNewFeedItemsCount();
		int GetNewFeedItemsCount(int channelId);

		void MarkFeedItemsAsRead(IEnumerable<int> feedItemIds);
		Task MarkFeedItemsAsReadAsync(IEnumerable<int> feedItemIds);

		IEnumerable<FeedItem> GetFeedItems();
		IEnumerable<FeedItem> GetFeedItems(int channelId);
	}
}
