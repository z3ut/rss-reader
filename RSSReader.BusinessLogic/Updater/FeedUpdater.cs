using AutoMapper;
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
		private readonly FeedContext _feedContext;
		private readonly IFeedLoader _feedLoader;
		private readonly IMapper _mapper;

		public FeedUpdater(FeedContext feedContext, IFeedLoader feedLoader, IMapper mapper)
		{
			_feedContext = feedContext;
			_feedLoader = feedLoader;
			_mapper = mapper;
		}

		public async Task UpdateFeedAsync(int channelId)
		{
			var channel = _feedContext.Channels.Find(channelId);

			if (channel == null)
			{
				return;
			}

			await UpdateChannelFeedAsync(channelId, channel.Link);
		}

		public async Task UpdateFeedsAsync()
		{
			var channels = _feedContext.Channels.ToList();
			var tasks = channels.Select(channel =>
					UpdateChannelFeedAsync(channel.ChannelId, channel.Link))
				.ToList();
			//var tasks = _feedContext.Channels.Select(channel =>
			//		UpdateChannelFeedAsync(channel.ChannelId, channel.Link))
			//	.ToList();

			await Task.WhenAll(tasks);
		}

		private async Task UpdateChannelFeedAsync(int channelId, string link)
		{
			try
			{
				var currentFeedItems = await _feedLoader.LoadFeedAsync(link);
				var newFeedtems = currentFeedItems
					.Where(item => !_feedContext.FeedItems
						.Any(fi => fi.ChannelId == channelId &&
							fi.FeedItemId == item.FeedItemId))
					.Select(newItem =>
					{
						newItem.ChannelId = channelId;
						newItem.IsRead = false;
						return _mapper.Map<DataAccess.Models.FeedItem>(newItem);
					});

				_feedContext.FeedItems.AddRange(newFeedtems);
				_feedContext.SaveChanges();
			}
			catch (Exception e)
			{
				// TODO: log errors
			}
		}
	}
}
