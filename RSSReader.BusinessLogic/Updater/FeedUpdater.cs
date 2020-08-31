using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RSSReader.BusinessLogic.Loader;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BusinessLogic.Updater
{
	public class FeedUpdater : IFeedUpdater
	{
		private readonly DbContextOptions<FeedContext> _feedContextOptions;
		private readonly IFeedLoader _feedLoader;
		private readonly IMapper _mapper;

		public FeedUpdater(DbContextOptions<FeedContext> feedContextOptions,
			IFeedLoader feedLoader, IMapper mapper)
		{
			_feedContextOptions = feedContextOptions;
			_feedLoader = feedLoader;
			_mapper = mapper;
		}

		public async Task UpdateFeedAsync(int channelId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var channel = feedContext.Channels.Find(channelId);

			if (channel == null)
			{
				return;
			}

			await UpdateChannelFeedAsync(channelId, channel.Link);
		}

		public async Task UpdateFeedsAsync()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var channels = feedContext.Channels.ToList();
			var tasks = channels.Select(channel =>
					UpdateChannelFeedAsync(channel.ChannelId, channel.Link))
				.ToList();
			await Task.WhenAll(tasks);
		}

		private async Task UpdateChannelFeedAsync(int channelId, string link)
		{
			try
			{
				using var feedContext = new FeedContext(_feedContextOptions);
				var currentFeedItems = await _feedLoader.LoadFeedAsync(link);
				var newFeedtems = currentFeedItems
					.Where(item => !feedContext.FeedItems
						.Any(fi => fi.ChannelId == channelId &&
							fi.FeedItemId == item.FeedItemId))
					.Select(newItem =>
					{
						newItem.ChannelId = channelId;
						newItem.IsRead = false;
						return _mapper.Map<DataAccess.Models.FeedItem>(newItem);
					});

				feedContext.FeedItems.AddRange(newFeedtems);
				feedContext.SaveChanges();
			}
			catch (Exception e)
			{
				// TODO: log errors
			}
		}
	}
}
