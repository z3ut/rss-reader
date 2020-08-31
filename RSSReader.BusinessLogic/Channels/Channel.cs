using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.BusinessLogic.Channels
{
	public class Channel
	{
		public int ChannelId { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public int? CategoryId { get; set; }

		public int NewFeedItemsCount { get; set; }

		public Category Category { get; set; }
		public IEnumerable<FeedItem> FeedItems { get; set; }
	}
}
