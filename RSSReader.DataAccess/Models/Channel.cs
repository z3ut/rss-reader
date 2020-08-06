using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RSSReader.DataAccess.Models
{
	public class Channel
	{
		public int ChannelId { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public int CategoryId { get; set; }

		//[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		//public int NewFeedItemsCount
		//{
		//	get
		//	{
		//		return (FeedItems == null) ? 0 :
		//			FeedItems.Where(item => item.IsRead == false).Count();
		//	}
		//}

		//public int NewFeedItemsCount { get; set; }

		public Category Category { get; set; }
		public List<FeedItem> FeedItems { get; set; }
	}
}
