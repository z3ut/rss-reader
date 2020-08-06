using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Channels
{
	public interface IChannelService
	{
		Channel CreateChannel(Channel channel);
		Channel GetChannel(int channelId);
		Channel UpdateChannel(Channel channel);
		void DeleteChannel(int channelId);

		IEnumerable<Channel> GetChannels();
		IEnumerable<Channel> GetChannelsWithFeeds();
	}
}
