using System.Collections.Generic;
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

		public delegate void SelectFeedItemHandler(FeedListItem feedListItem);
		public event SelectFeedItemHandler SelectFeedItem;

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
			SelectFeedItem?.Invoke(item);
		}


		private void ListViewMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var feedListItems = subscriptionListView.SelectedItems.Cast<FeedListItem>().ToList();

			MarkFeedsAsRead?.Invoke(feedListItems);
		}
	}
}
