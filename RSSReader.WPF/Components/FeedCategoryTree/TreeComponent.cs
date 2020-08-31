﻿using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Channels;
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
		public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

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

				if (Item is Category)
				{
					var category = Item as Category;
					return category.NewFeedItemsCount;
				}

				if (Item == null && SubComponents != null)
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
		public object Item { get; set; }


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
		//protected
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

		private void SubComponents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(NameWithNumberOfNewItems));
			OnPropertyChanged(nameof(NewFeedItemsCount));
			//OnPropertyChanged(nameof(SubComponents));
		}
	}


	// https://stackoverflow.com/questions/1427471/observablecollection-not-noticing-when-item-in-it-changes-even-with-inotifyprop
	public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
		where T : INotifyPropertyChanged
	{
		public TrulyObservableCollection()
		{
			CollectionChanged += FullObservableCollectionCollectionChanged;
		}

		public TrulyObservableCollection(IEnumerable<T> pItems) : this()
		{
			foreach (var item in pItems)
			{
				this.Add(item);
			}
		}

		private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Object item in e.NewItems)
				{
					((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
				}
			}
			if (e.OldItems != null)
			{
				foreach (Object item in e.OldItems)
				{
					((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
				}
			}
		}

		private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
			OnCollectionChanged(args);
		}
	}
}