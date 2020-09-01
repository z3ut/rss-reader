using Autofac;
using Microsoft.EntityFrameworkCore;
using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Channels;
using RSSReader.BusinessLogic.Feeds;
using RSSReader.BusinessLogic.Updater;
using RSSReader.DataAccess;
using RSSReader.WPF.Components.FeedCategoryTree;
using RSSReader.WPF.Components.FeedList;
using RSSReader.WPF.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;


namespace RSSReader.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ICategoryService _categoryService;
		private IChannelService _channelService;
		private IFeedService _feedService;
		private IFeedUpdater _feedUpdater;

		private IContainer _container;

		private TreeComponent _feedComponent;
		private TreeComponent _subscriptionsComponent;

		private int _updateFeedIntervalMS = 60 * 60 * 1000;
		private Timer _updateFeedTimer;

		public MainWindow()
		{
			InitializeComponent();

			DataContext = this;

			InitializeDI();
			InitializeFeed();
			UpdateChannelNewItemsCount();
			SetUpdateFeedTimer();

			Task.Run(async () => await UpdateFeed());
		}

		public ObservableCollection<TreeComponent> TreeComponents { get; set; }
		public ObservableCollection<FeedListItem> FeedListItems { get; set; }

		public static readonly DependencyProperty FeedItemProperty =
			DependencyProperty.Register(nameof(FeedItem), typeof(FeedItem),
				typeof(MainWindow), new FrameworkPropertyMetadata(new FeedItem()));

		public FeedItem FeedItem
		{
			get { return GetValue(FeedItemProperty) as FeedItem; }
			set { SetValue(FeedItemProperty, value); }
		}

		private void InitializeDI()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new WPFModule("Data Source=feed.db"));
			_container = builder.Build();

			var feedContext = _container.Resolve<FeedContext>();
			feedContext.Database.Migrate();


			_categoryService = _container.Resolve<ICategoryService>();
			_channelService = _container.Resolve<IChannelService>();
			_feedService = _container.Resolve<IFeedService>();
			_feedUpdater = _container.Resolve<IFeedUpdater>();
		}

		private void InitializeFeed()
		{
			_feedComponent = new TreeComponent()
			{
				Name = "Feed",
				PopupType = PopupType.FeedPopup,
				TreeComponentType = TreeComponentType.Feed,
				NewFeedItemsCountManual = 0
			};

			var categories = _categoryService.GetCategories();

			_subscriptionsComponent = new TreeComponent(
				ConvertCategoriesToTreeComponents(categories))
			{
				Name = "Subscriptions",
				PopupType = PopupType.SubscriptionsPopup,
				TreeComponentType = TreeComponentType.Subscriptions
			};

			TreeComponents = new ObservableCollection<TreeComponent>()
			{
				_feedComponent,
				_subscriptionsComponent
			};

			FeedListItems = new ObservableCollection<FeedListItem>();
		}

		private void UpdateChannelNewItemsCount()
		{
			_feedComponent.NewFeedItemsCountManual = _feedService.GetNewFeedItemsCount();

			var channelNewItemsCount = _channelService.GetChannelNewItemsCount();

			FiilChannelNewItemsInTreeComponents(channelNewItemsCount,
				_subscriptionsComponent.SubComponents);
		}

		private void SetUpdateFeedTimer()
		{
			_updateFeedTimer = new Timer(_updateFeedIntervalMS);
			_updateFeedTimer.Elapsed += _updateFeedTimer_Elapsed;
			_updateFeedTimer.AutoReset = true;
			_updateFeedTimer.Enabled = true;
		}

		private async Task UpdateFeed()
		{
			await _feedUpdater.UpdateFeedsAsync();

			App.Current.Dispatcher.Invoke(delegate
			{
				UpdateChannelNewItemsCount();
			});
		}

		private async void _updateFeedTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			await UpdateFeed();
		}

		private void FiilChannelNewItemsInTreeComponents(
			IEnumerable<ChannelNewItemsCount> channelNewItemsCounts,
			IEnumerable<TreeComponent> treeComponents)
		{
			foreach (var component in treeComponents)
			{
				FiilChannelNewItemsInTreeComponents(channelNewItemsCounts, component.SubComponents);

				if (component.TreeComponentType == TreeComponentType.Channel)
				{
					var channelNewFeedItemsCount = channelNewItemsCounts
						.FirstOrDefault(c => c.ChannelId == (component.Item as Channel)?.ChannelId)
						?.NewItemsCounts;
					component.NewFeedItemsCountManual = channelNewFeedItemsCount.GetValueOrDefault(0);
				}
			}
		}

		private void FillChannelNewItemsCount(IEnumerable<Category> categories,
			IEnumerable<ChannelNewItemsCount> channelNewItemsCounts)
		{
			foreach (var category in categories)
			{
				FillChannelNewItemsCount(category.ChildCategories, channelNewItemsCounts);

				foreach (var channel in category.Channels)
				{
					var channelCount = channelNewItemsCounts
						.FirstOrDefault(c => c.ChannelId == channel.ChannelId);

					if (channelCount != null)
					{
						channel.NewFeedItemsCount = channelCount.NewItemsCounts;
					}
				}
			}
		}

		private IEnumerable<TreeComponent> ConvertCategoriesToTreeComponents(IEnumerable<Category> categories)
		{
			return categories?.Select(category =>
			{
				var subComponents = ConvertCategoriesToTreeComponents(
						category.ChildCategories)
					.Concat(ConvertChannelsToTreeComponents(category.Channels));
				return new TreeComponent(category, subComponents);
			}) ?? new List<TreeComponent>();
		}

		private IEnumerable<TreeComponent> ConvertChannelsToTreeComponents(IEnumerable<Channel> channels)
		{
			return channels?.Select(channel => new TreeComponent(channel)) ?? new List<TreeComponent>();
		}

		private async void FeedList_MarkFeedsAsRead(IEnumerable<FeedListItem> items)
		{
			await _feedService.MarkFeedItemsAsReadAsync(items.Select(fi => fi.FeedItemId));

			foreach (var item in items)
			{
				item.IsRead = true;
				var feedItem = item.FeedItem;
				if (feedItem != null && feedItem.IsRead == false)
				{
					feedItem.IsRead = true;
					//_feedService.UpdateFeedItem(feedItem);

					var channelTreeComponent = FindChannel(TreeComponents, feedItem.ChannelId);
					var channel = channelTreeComponent.Item as Channel;

					channel.NewFeedItemsCount--;
					channelTreeComponent.NewFeedItemsCountManual--;
					_feedComponent.NewFeedItemsCountManual--;

					channelTreeComponent.OnPropertyChanged(nameof(TreeComponent.NewFeedItemsCount));
					channelTreeComponent.OnPropertyChanged(nameof(TreeComponent.NewFeedItemsCountManual));
					channelTreeComponent.OnPropertyChanged(nameof(TreeComponent.NameWithNumberOfNewItems));
				}
			}
		}

		private void FeedCategoryTree_CreateCategory(NewCategoryData newCategoryData)
		{
			var category = newCategoryData.Category;
			
			if (category == null)
			{
				return;
			}

			var createdCategory = _categoryService.CreateCategory(category);

			var treeComponent = new TreeComponent(createdCategory);

			var parentComponent = newCategoryData.ParentTreeComponent ?? _subscriptionsComponent;

			parentComponent.SubComponents.Add(treeComponent);
			parentComponent.IsExpanded = true;
		}

		private void FeedCategoryTree_SelectItem(TreeComponent item)
		{
			if (item == null)
			{
				return;
			}

			if (item.Item is Channel)
			{
				var channel = item.Item as Channel;
				var feedItems = _feedService.GetFeedItems(channel.ChannelId);

				SetFeedItems(feedItems, channel.Title);
			}

			if (item.Item is Category)
			{
				
			}

			if (item.TreeComponentType == TreeComponentType.Feed)
			{
				var feedItems = _feedService.GetFeedItems();
				SetFeedItems(feedItems);
			}

			if (item.TreeComponentType == TreeComponentType.Subscriptions)
			{

			}
		}

		private async void FeedCategoryTree_CreateChannel(NewChannelData newChannelData)
		{
			var channel = newChannelData.Channel;
			var parentComponent = newChannelData.ParentTreeComponent;
			var category = (parentComponent?.Item as Category);

			if (channel == null || parentComponent == null || category == null)
			{
				return;
			}

			channel.CategoryId = category.CategoryId;

			var createdChannel =_channelService.CreateChannel(channel);

			var treeComponent = new TreeComponent(createdChannel);

			parentComponent.SubComponents.Add(treeComponent);
			parentComponent.IsExpanded = true;

			await _feedUpdater.UpdateFeedAsync(createdChannel.ChannelId);

			UpdateChannelStats(createdChannel.ChannelId, treeComponent);
		}

		private void FeedCategoryTree_DeleteItem(TreeComponent item)
		{
			switch (item.TreeComponentType)
			{
				case TreeComponentType.Category:
					var category = item.Item as Category;

					var deleteCategoryResult = MessageBox.Show(
						$"Are you sure you want to delete category {category.Title}?",
						"Delete category", MessageBoxButton.YesNo);

					if (deleteCategoryResult == MessageBoxResult.No)
					{
						return;
					}

					_categoryService.DeleteCategory(category.CategoryId);
					RemoveSubcomponentWithItem(_subscriptionsComponent, category);

					break;
				case TreeComponentType.Channel:
					var channel = item.Item as Channel;

					var deleteChannelResult = MessageBox.Show(
						 $"Are you sure you want to delete channel {channel.Title}?",
						"Delete channel",  MessageBoxButton.YesNo);

					if (deleteChannelResult == MessageBoxResult.No)
					{
						return;
					}

					_channelService.DeleteChannel(channel.ChannelId);
					RemoveSubcomponentWithItem(_subscriptionsComponent, channel);

					break;
				default:
					break;
			}
		}

		private void SetFeedItems(IEnumerable<FeedItem> feedItems, string channelTitle = "")
		{
			FeedListItems.Clear();

			foreach (var feedItem in feedItems.OrderByDescending(fi => fi.DateTime))
			{
				FeedListItems.Add(new FeedListItem(feedItem, feedItem?.Channel?.Title ?? channelTitle));
			}
		}

		private void RemoveSubcomponentWithItem(TreeComponent treeComponent, object itemToRemove)
		{
			foreach (var subComponent in treeComponent.SubComponents)
			{
				if (subComponent.Item == itemToRemove)
				{
					treeComponent.SubComponents.Remove(subComponent);
					return;
				}

				RemoveSubcomponentWithItem(subComponent, itemToRemove);
			}
		}

		private TreeComponent FindChannel(IEnumerable<TreeComponent> treeComponents, int channelId)
		{
			foreach (var c in treeComponents)
			{
				if (c.Item is Channel)
				{
					var channel = c.Item as Channel;
					if (channel.ChannelId == channelId)
					{
						return c;
					}
				}

				var foundChannel = FindChannel(c.SubComponents, channelId);

				if (foundChannel != null)
				{
					return foundChannel;
				}
			}

			return null;
		}

		private void UpdateChannelStats(int channelId, TreeComponent treeComponent)
		{
			var channelNewFeedItemsCount = _feedService.GetNewFeedItemsCount(channelId);

			_feedComponent.NewFeedItemsCountManual += channelNewFeedItemsCount;
			treeComponent.NewFeedItemsCountManual = channelNewFeedItemsCount;
		}

		private void FeedList_SelectFeedItem(FeedListItem feedListItem)
		{
			FeedItem = feedListItem?.FeedItem;
		}
	}
}
