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
		private readonly FeedContext _feedContext;
		private readonly IMapper _mapper;

		public FeedService(FeedContext feedContext, IMapper mapper)
		{
			_feedContext = feedContext;
			_mapper = mapper;
		}

		public FeedItem CreateFeedItem(FeedItem feedItem)
		{
			var feedItemDA = _mapper.Map<DataAccess.Models.FeedItem>(feedItem);
			//feedItemDA.FeedItemId = Guid.NewGuid().ToString();
			_feedContext.FeedItems.Add(feedItemDA);
			_feedContext.SaveChanges();

			var savedFeeditem = _feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == feedItemDA.FeedItemId);
			//_feedContext.Entry(feedItemDA).Reload();
			return _mapper.Map<FeedItem>(feedItemDA);
		}

		public void DeleteFeedItem(int feedItemId)
		{
			var feedItem = _feedContext.FeedItems
				.SingleOrDefault(c => c.FeedItemId == feedItemId);

			if (feedItem == null)
			{
				return;
			}

			_feedContext.FeedItems.Remove(feedItem);
			_feedContext.SaveChanges();
		}

		public FeedItem GetFeedItem(int feedItemId)
		{
			return _mapper.Map<FeedItem>(_feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == feedItemId));
		}

		public IEnumerable<FeedItem> GetFeedItems()
		{
			return _mapper.Map<IEnumerable<FeedItem>>(_feedContext.FeedItems
				.Include(fi => fi.Channel)
				.ToList());
		}

		public IEnumerable<FeedItem> GetFeedItems(int channelId)
		{
			return _mapper.Map<IEnumerable<FeedItem>>(_feedContext.FeedItems
				.Include(fi => fi.Channel)
				.Where(item => item.ChannelId == channelId)
				.ToList());
		}

		public int GetNewFeedItemsCount()
		{
			return _feedContext.FeedItems
				.Where(fi => fi.IsRead == false)
				.Count();
		}

		public int GetNewFeedItemsCount(int channelId)
		{
			return _feedContext.FeedItems
				.Where(fi => fi.ChannelId == channelId && fi.IsRead == false)
				.Count();
		}

		public void MarkFeedItemsAsRead(IEnumerable<int> feedItemIds)
		{
			foreach (var feedItem in _feedContext.FeedItems
				.Where(fi => feedItemIds.Contains(fi.FeedItemId)))
			{
				feedItem.IsRead = true;
			}

			_feedContext.SaveChanges();
		}

		public async Task MarkFeedItemsAsReadAsync(IEnumerable<int> feedItemIds)
		{
			foreach (var feedItem in _feedContext.FeedItems
				.Where(fi => feedItemIds.Contains(fi.FeedItemId)))
			{
				feedItem.IsRead = true;
			}

			await _feedContext.SaveChangesAsync();
		}

		public FeedItem UpdateFeedItem(FeedItem feedItem)
		{
			var savedFeedItem = _feedContext.FeedItems.Find(feedItem.FeedItemId);

			var newFeedItem = _mapper.Map<DataAccess.Models.FeedItem>(feedItem);
			_feedContext.Entry(savedFeedItem).CurrentValues.SetValues(newFeedItem);
			_feedContext.SaveChanges();

			var savedFeeditem = _feedContext.FeedItems
				.Include(fi => fi.Channel)
				.SingleOrDefault(fi => fi.FeedItemId == savedFeedItem.FeedItemId);
			//_feedContext.Entry(savedFeedItem).Reload();

			return _mapper.Map<FeedItem>(savedFeedItem);
		}
	}
}
