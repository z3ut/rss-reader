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
		private readonly FeedContext _feedContext;
		private readonly IMapper _mapper;

		public ChannelService(FeedContext feedContext, IMapper mapper)
		{
			_feedContext = feedContext;
			_mapper = mapper;
		}

		public Channel CreateChannel(Channel channel)
		{
			var channelDA = _mapper.Map<DataAccess.Models.Channel>(channel);
			//channelDA.ChannelId = Guid.NewGuid().ToString();
			_feedContext.Channels.Add(channelDA);
			_feedContext.SaveChanges();
			return _mapper.Map<Channel>(channelDA);
		}

		public void DeleteChannel(int channelId)
		{
			var channel = _feedContext.Channels
				.SingleOrDefault(c => c.ChannelId == channelId);

			if (channel == null)
			{
				return;
			}

			_feedContext.Channels.Remove(channel);
			_feedContext.SaveChanges();
		}

		public Channel GetChannel(int channelId)
		{
			return _mapper.Map<Channel>(_feedContext.Channels.Find(channelId));
		}

		public IEnumerable<Channel> GetChannels()
		{
			return _mapper.Map<IEnumerable<Channel>>(_feedContext.Channels.ToList());
		}

		public IEnumerable<Channel> GetChannelsWithFeeds()
		{
			return _mapper.Map<IEnumerable<Channel>>(_feedContext.Channels
				.Include(channel => channel.FeedItems)
				.ToList());
		}

		public Channel UpdateChannel(Channel channel)
		{
			var savedChannel = _feedContext.Channels.Find(channel.ChannelId);

			var newChannel = _mapper.Map<DataAccess.Models.Channel>(channel);
			_feedContext.Entry(savedChannel).CurrentValues.SetValues(newChannel);
			_feedContext.SaveChanges();
			_feedContext.Entry(savedChannel).Reload();

			return _mapper.Map<Channel>(savedChannel);
		}
	}
}
