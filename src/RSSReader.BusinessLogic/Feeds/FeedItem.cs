﻿using RSSReader.BusinessLogic.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Feeds
{
	public class FeedItem
	{
		public int FeedItemId { get; set; }
		public string RSSFeedId { get; set; }
		public int ChannelId { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public DateTime DateTime { get; set; }
		public bool IsRead { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }

		public Channel Channel { get; set; }
	}
}
