using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Channels
{
	public class ChannelNewItemsCount
	{
		public int ChannelId { get; set; }
		public int NewItemsCounts { get; set; }

		public ChannelNewItemsCount(int channelId, int newItemsCount)
		{
			ChannelId = channelId;
			NewItemsCounts = newItemsCount;
		}
	}
}
