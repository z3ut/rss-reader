using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Channels;
using RSSReader.WPF.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RSSReader.WPF.Components.FeedCategoryTree
{
	public class TreeComponent : INotifyPropertyChanged
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(NameWithNumberOfNewItems));
			}
		}

		public string NameWithNumberOfNewItems
		{
			get
			{
				var newFeedItemsCount = NewFeedItemsCount;
				if (newFeedItemsCount > 0)
				{
					return $"{Name} (+{newFeedItemsCount})";
				}
				return Name;
			}
		}

		private int _newFeedItemsCountManual { get; set; }
		public int NewFeedItemsCountManual
		{
			get { return _newFeedItemsCountManual; }
			set
			{
				_newFeedItemsCountManual = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(NameWithNumberOfNewItems));
				OnPropertyChanged(nameof(NewFeedItemsCount));
			}
		}

		public int NewFeedItemsCount
		{
			get
			{
				if (NewFeedItemsCountManual != 0)
				{
					return NewFeedItemsCountManual;
				}

				if (Item is Channel)
				{
					var channel = Item as Channel;
					return channel.NewFeedItemsCount;
				}

				if ((Item == null || Item is Category) && SubComponents != null)
				{
					return SubComponents
						.Select(c => c.NewFeedItemsCount)
						.Sum();
				}

				return 0;
			}
		}

		public PopupType PopupType { get; set; }
		public TreeComponentType TreeComponentType { get; set; }

		private object _item { get; set; }
		public object Item
		{
			get { return _item; }
			set
			{
				_item = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(NameWithNumberOfNewItems));
				OnPropertyChanged(nameof(NewFeedItemsCount));
			}
		}


		private bool _isExpanded { get; set; }
		public bool IsExpanded
		{
			get
			{
				return _isExpanded;
			}
			set
			{
				_isExpanded = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IconPath));
				OnPropertyChanged(nameof(IsIconVisible));
			}
		}

		public string IconPathExpanded { get; set; }
		public string IconPathCollapsed { get; set; }
		public string IconPath { get { return IsExpanded ? IconPathExpanded : IconPathCollapsed; } }
		public bool IsIconVisible => !String.IsNullOrWhiteSpace(IsExpanded ? IconPathExpanded : IconPathCollapsed);

		public ObservableCollection<TreeComponent> SubComponents { get; private set; }


		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public TreeComponent(IEnumerable<TreeComponent> subComponents = null)
		{
			SubComponents = new TrulyObservableCollection<TreeComponent>(subComponents ?? new List<TreeComponent>());
			SubComponents.CollectionChanged += SubComponents_CollectionChanged;
		}

		public TreeComponent(Category category, IEnumerable<TreeComponent> subComponents = null)
		{
			Item = category;
			Name = category.Title;
			PopupType = PopupType.CategoryPopup;
			TreeComponentType = TreeComponentType.Category;
			IconPathCollapsed = "/assets/img/FolderClosed_16x.png";
			IconPathExpanded = "/assets/img/FolderOpened_16x.png";
			SubComponents = new TrulyObservableCollection<TreeComponent>(subComponents ?? new List<TreeComponent>());

			SubComponents.CollectionChanged += SubComponents_CollectionChanged;
		}

		public TreeComponent(Channel channel)
		{
			Item = channel;
			Name = channel.Title;
			PopupType = PopupType.ChannelPopup;
			TreeComponentType = TreeComponentType.Channel;
			SubComponents = new TrulyObservableCollection<TreeComponent>();
			NewFeedItemsCountManual = channel.NewFeedItemsCount;

			SubComponents.CollectionChanged += SubComponents_CollectionChanged;
		}

		private void SubComponents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(NameWithNumberOfNewItems));
			OnPropertyChanged(nameof(NewFeedItemsCount));
			OnPropertyChanged(nameof(SubComponents));
		}
	}
}
