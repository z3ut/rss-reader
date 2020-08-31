using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.BusinessLogic.Channels
{
	public class ChannelService : IChannelService
	{
		private readonly DbContextOptions<FeedContext> _feedContextOptions;
		private readonly IMapper _mapper;

		public ChannelService(DbContextOptions<FeedContext> feedContextOptions, IMapper mapper)
		{
			_feedContextOptions = feedContextOptions;
			_mapper = mapper;
		}

		public Channel CreateChannel(Channel channel)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var channelDA = _mapper.Map<DataAccess.Models.Channel>(channel);
			feedContext.Channels.Add(channelDA);
			feedContext.SaveChanges();
			return _mapper.Map<Channel>(channelDA);
		}

		public void DeleteChannel(int channelId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var channel = feedContext.Channels
				.SingleOrDefault(c => c.ChannelId == channelId);

			if (channel == null)
			{
				return;
			}

			feedContext.Channels.Remove(channel);
			feedContext.SaveChanges();
		}

		public Channel GetChannel(int channelId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<Channel>(feedContext.Channels.Find(channelId));
		}

		public IEnumerable<Channel> GetChannels()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<IEnumerable<Channel>>(feedContext.Channels.ToList());
		}

		public IEnumerable<Channel> GetChannelsWithFeeds()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<IEnumerable<Channel>>(feedContext.Channels
				.Include(channel => channel.FeedItems)
				.ToList());
		}

		public Channel UpdateChannel(Channel channel)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var savedChannel = feedContext.Channels.Find(channel.ChannelId);

			var newChannel = _mapper.Map<DataAccess.Models.Channel>(channel);
			feedContext.Entry(savedChannel).CurrentValues.SetValues(newChannel);
			feedContext.SaveChanges();
			feedContext.Entry(savedChannel).Reload();

			return _mapper.Map<Channel>(savedChannel);
		}
	}
}
