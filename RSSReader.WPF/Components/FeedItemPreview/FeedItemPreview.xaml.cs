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
