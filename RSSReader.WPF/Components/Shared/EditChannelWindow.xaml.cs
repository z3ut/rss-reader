using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RSSReader.WPF.Components.Shared
{
	/// <summary>
	/// Interaction logic for EditChannelWindow.xaml
	/// </summary>
	public partial class EditChannelWindow : Window
	{
		public string ChannelTitle { get; set; }
		public string ChannelLink { get; set; }

		public EditChannelWindow()
		{
			InitializeComponent();

			DataContext = this;

			if (String.IsNullOrEmpty(Title))
			{
				Title = "Create channel";
			}

			TitleTextBox.Focus();
		}

		public EditChannelWindow(string title, string link): this()
		{
			ChannelTitle = title;
			ChannelLink = link;
			Title = $"Edit channel {title}";
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
