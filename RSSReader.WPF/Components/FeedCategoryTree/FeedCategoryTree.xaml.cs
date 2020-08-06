using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Channels;
using RSSReader.WPF.Components.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RSSReader.WPF.Components.FeedCategoryTree
{
	/// <summary>
	/// Interaction logic for FeedCategoryTree.xaml
	/// </summary>
	public partial class FeedCategoryTree : UserControl
	{
		public FeedCategoryTree()
		{
			InitializeComponent();

			ItemToContextMenuConverter.FeedPopup
				= this.Resources["FeedPopup"] as ContextMenu;
			ItemToContextMenuConverter.SubscriptionsPopup
				= this.Resources["SubscriptionsPopup"] as ContextMenu;
			ItemToContextMenuConverter.CategoryPopup
				= this.Resources["CategoryPopup"] as ContextMenu;
			ItemToContextMenuConverter.ChannelPopup
				= this.Resources["ChannelPopup"] as ContextMenu;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			tree.Items.Refresh();
		}

		public delegate void SelectItemHandler(TreeComponent item);
		public event SelectItemHandler SelectItem;

		public delegate void DeleteItemHandler(TreeComponent item);
		public event DeleteItemHandler DeleteItem;

		public delegate void CreateCategoryHandler(NewCategoryData newCategoryData);
		public event CreateCategoryHandler CreateCategory;

		public delegate void CreateChannelHandler(NewChannelData newChannelData);
		public event CreateChannelHandler CreateChannel;

		public static readonly DependencyProperty CategoriesTreeComponents =
			DependencyProperty.Register(nameof(TreeComponents), typeof(IEnumerable<TreeComponent>),
				typeof(FeedCategoryTree), new FrameworkPropertyMetadata(new List<TreeComponent>()));

		public ObservableCollection<TreeComponent> TreeComponents
		{
			get { return GetValue(CategoriesTreeComponents) as ObservableCollection<TreeComponent>; }
			set { SetValue(CategoriesTreeComponents, value); }
		}

		private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			SelectItem?.Invoke(e.NewValue as TreeComponent);
		}

		private void SubscriptionsPopupNewCategory(object sender, RoutedEventArgs e)
		{
			var treeComponent = (e.OriginalSource as MenuItem)?.DataContext as TreeComponent;

			if (treeComponent == null)
			{
				return;
			}

			var promptDialog = new PromptWindow("Create new category", "Category name");
			if (promptDialog.ShowDialog() == true)
			{
				var category = new Category()
				{
					Title = promptDialog.Result,
					ParentCategoryId = (treeComponent.Item as Category)?.CategoryId
				};
				var newCategoryData = new NewCategoryData()
				{
					Category = category,
					ParentTreeComponent = treeComponent
				};

				CreateCategory?.Invoke(newCategoryData);
			}
		}

		private void CategoryPopupNewCategory(object sender, RoutedEventArgs e)
		{
			SubscriptionsPopupNewCategory(sender, e);
		}

		private void CategoryPopupNewChannel(object sender, RoutedEventArgs e)
		{
			var treeComponent = (e.OriginalSource as MenuItem)?.DataContext as TreeComponent;

			if (treeComponent == null)
			{
				return;
			}

			var editChannelDialog = new EditChannelWindow();
			if (editChannelDialog.ShowDialog() == true)
			{
				var channel = new Channel()
				{
					Title = editChannelDialog.ChannelTitle,
					Link = editChannelDialog.ChannelLink,
					CategoryId = (treeComponent.Item as Category)?.CategoryId
				};
				var newCategoryData = new NewChannelData()
				{
					Channel = channel,
					ParentTreeComponent = treeComponent
				};

				CreateChannel?.Invoke(newCategoryData);
			}
		}

		private void CategoryPopupDelete(object sender, RoutedEventArgs e)
		{
			var treeComponent = (e.OriginalSource as MenuItem)?.DataContext as TreeComponent;

			if (treeComponent == null)
			{
				return;
			}

			DeleteItem?.Invoke(treeComponent);
		}

		private void ChannelPopupDelete(object sender, RoutedEventArgs e)
		{
			var treeComponent = (e.OriginalSource as MenuItem)?.DataContext as TreeComponent;

			if (treeComponent == null)
			{
				return;
			}

			DeleteItem?.Invoke(treeComponent);
		}
	}

	public class NewCategoryData
	{
		public Category Category { get; set; }
		public TreeComponent ParentTreeComponent { get; set; }
	}

	public class NewChannelData
	{
		public Channel Channel { get; set; }
		public TreeComponent ParentTreeComponent { get; set; }
	}
}
