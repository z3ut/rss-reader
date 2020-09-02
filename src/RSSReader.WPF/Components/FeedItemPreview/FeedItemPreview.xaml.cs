using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RSSReader.WPF.Components.FeedItemPreview
{
	/// <summary>
	/// Interaction logic for FeedItemPreview.xaml
	/// </summary>
	public partial class FeedItemPreview : UserControl
	{
		public FeedItemPreview()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty FeedItemProperty =
			DependencyProperty.Register(nameof(FeedItem), typeof(FeedItem),
				typeof(FeedItemPreview), new FrameworkPropertyMetadata(new FeedItem()));

		public FeedItem FeedItem
		{
			get { return GetValue(FeedItemProperty) as FeedItem; }
			set { SetValue(FeedItemProperty, value); }
		}
	}
}
