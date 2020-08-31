﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RSSReader.WPF.Components.FeedList
{
	/// <summary>
	/// Interaction logic for FeedList.xaml
	/// </summary>
	public partial class FeedList : UserControl
	{
		public FeedList()
		{
			InitializeComponent();
		}

		public delegate void MarkFeedAsReadHandler(IEnumerable<FeedListItem> items);
		public event MarkFeedAsReadHandler MarkFeedsAsRead;

		public static readonly DependencyProperty FeedItemsProperty =
			DependencyProperty.Register(nameof(FeedItems), typeof(IEnumerable<FeedListItem>),
				typeof(FeedList), new FrameworkPropertyMetadata(new List<FeedListItem>()));

		public ObservableCollection<FeedListItem> FeedItems
		{
			get { return GetValue(FeedItemsProperty) as ObservableCollection<FeedListItem>; }
			set { SetValue(FeedItemsProperty, value); }
		}

		private void subscriptionListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var item = (sender as ListView).SelectedItem as FeedListItem;

			if (item == null)
			{
				return;
			}

			MarkFeedsAsRead?.Invoke(new List<FeedListItem>() { item });
		}


		private void ListViewMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var feedListItems = subscriptionListView.SelectedItems.Cast<FeedListItem>().ToList();

			MarkFeedsAsRead?.Invoke(feedListItems);
		}

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			OpenBrowser(e.Uri.AbsoluteUri);
			e.Handled = true;
		}

		// https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
		public static void OpenBrowser(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}
	}
}