using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BusinessLogic.Feeds
{
	public class FeedService : IFeedService
	{
		private readonly DbContextOptions<FeedContext> _feedContextOptions;
		private readonly IMapper _mapper;

		public FeedService(DbContextOptions<FeedContext> feedContextOptions, IMapper mapper)
		{
			_feedContextOptions = feedContextOptions;
			_mapper = mapper;
		}

		public FeedItem CreateFeedItem(FeedItem feedItem)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var feedItemDA = _mapper.Map<DataAccess.Models.FeedItem>(feedItem);
			feedContext.FeedItems.Add(feedItemDA);
			feedContext.SaveChanges();

			var savedFeeditem = feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == feedItemDA.FeedItemId);

			return _mapper.Map<FeedItem>(feedItemDA);
		}

		public void DeleteFeedItem(int feedItemId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var feedItem = feedContext.FeedItems
				.SingleOrDefault(c => c.FeedItemId == feedItemId);

			if (feedItem == null)
			{
				return;
			}

			feedContext.FeedItems.Remove(feedItem);
			feedContext.SaveChanges();
		}

		public FeedItem GetFeedItem(int feedItemId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<FeedItem>(feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == feedItemId));
		}

		public IEnumerable<FeedItem> GetFeedItems()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<IEnumerable<FeedItem>>(feedContext.FeedItems
				.Include(fi => fi.Channel)
				.ToList());
		}

		public IEnumerable<FeedItem> GetFeedItems(int channelId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<IEnumerable<FeedItem>>(feedContext.FeedItems
				.Include(fi => fi.Channel)
				.Where(item => item.ChannelId == channelId)
				.ToList());
		}

		public int GetNewFeedItemsCount()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return feedContext.FeedItems
				.Where(fi => fi.IsRead == false)
				.Count();
		}

		public int GetNewFeedItemsCount(int channelId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return feedContext.FeedItems
				.Where(fi => fi.ChannelId == channelId && fi.IsRead == false)
				.Count();
		}

		public void MarkFeedItemsAsRead(IEnumerable<int> feedItemIds)
		{
			using var feedContext = new FeedContext(_feedContextOptions);

			foreach (var feedItem in feedContext.FeedItems
				.Where(fi => feedItemIds.Contains(fi.FeedItemId)))
			{
				feedItem.IsRead = true;
			}

			feedContext.SaveChanges();
		}

		public async Task MarkFeedItemsAsReadAsync(IEnumerable<int> feedItemIds)
		{
			using var feedContext = new FeedContext(_feedContextOptions);

			foreach (var feedItem in feedContext.FeedItems
				.Where(fi => feedItemIds.Contains(fi.FeedItemId)))
			{
				feedItem.IsRead = true;
			}

			await feedContext.SaveChangesAsync();
		}

		public FeedItem UpdateFeedItem(FeedItem feedItem)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var savedFeedItem = feedContext.FeedItems.Find(feedItem.FeedItemId);
			var newFeedItem = _mapper.Map<DataAccess.Models.FeedItem>(feedItem);
			feedContext.Entry(savedFeedItem).CurrentValues.SetValues(newFeedItem);
			feedContext.SaveChanges();

			var savedFeeditem = feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == savedFeedItem.FeedItemId);

			return _mapper.Map<FeedItem>(savedFeedItem);
		}
	}
}
