using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace RSSReader.WPF.Components.FeedCategoryTree
{
	[ValueConversion(typeof(TreeComponent), typeof(ContextMenu))]
	public class ItemToContextMenuConverter : IValueConverter
	{
		public static ContextMenu FeedPopup;
		public static ContextMenu SubscriptionsPopup;
		public static ContextMenu CategoryPopup;
		public static ContextMenu ChannelPopup;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TreeComponent item = value as TreeComponent;
			if (item == null) return null;

			switch (item.PopupType)
			{
				case PopupType.FeedPopup:
					return FeedPopup;
				case PopupType.SubscriptionsPopup:
					return SubscriptionsPopup;
				case PopupType.CategoryPopup:
					return CategoryPopup;
				case PopupType.ChannelPopup:
					return ChannelPopup;
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
