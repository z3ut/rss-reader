using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RSSReader.WPF.Components.FeedList
{
	public class FeedListItem : INotifyPropertyChanged
	{
		public int FeedItemId { get; set; }
		public string RSSFeedId { get; set; }
		public int ChannelId { get; set; }
		public string ChannelTitle { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public DateTime DateTime { get; set; }
		public FeedItem FeedItem { get; set; }

		private bool _isRead;
		public bool IsRead { get { return _isRead; } set { _isRead = value; OnPropertyChanged(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public FeedListItem() { }

		public FeedListItem(FeedItem feedItem, string channelTitle = "")
		{
			FeedItemId = feedItem.FeedItemId;
			RSSFeedId = feedItem.RSSFeedId;
			ChannelId = feedItem.ChannelId;
			ChannelTitle = feedItem.Channel?.Title ?? channelTitle;
			Title = feedItem.Title;
			Link = feedItem.Link;
			DateTime = feedItem.DateTime;
			IsRead = feedItem.IsRead;
			FeedItem = feedItem;
		}
	}
}
