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

namespace RSSReader.WPF.Components.Shared
{
	/// <summary>
	/// Interaction logic for WebBrowserLink.xaml
	/// </summary>
	public partial class WebBrowserLink : UserControl
	{
		public WebBrowserLink()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty LinkProperty =
			DependencyProperty.Register(nameof(Link), typeof(string),
				typeof(WebBrowserLink), new FrameworkPropertyMetadata(""));

		public string Link
		{
			get { return GetValue(LinkProperty) as string; }
			set { SetValue(LinkProperty, value); }
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
