using RSSReader.BusinessLogic.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RSSReader.WPF.Windows
{
	/// <summary>
	/// Interaction logic for ConfigWindow.xaml
	/// </summary>
	public partial class ConfigWindow : Window
	{

		public string UpdateFeedIntervalMS { get; set; }

		public Config InitialConfig;
		public Config UpdatedConfig;

		public ConfigWindow(Config config)
		{
			InitializeComponent();

			DataContext = this;

			InitialConfig = config;

			UpdateFeedIntervalMS = InitialConfig.UpdateFeedIntervalMS.ToString();
		}

		private void NumericPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			UpdatedConfig = new Config()
			{
				ConfigId = InitialConfig.ConfigId,
				UpdateFeedIntervalMS = InitialConfig.UpdateFeedIntervalMS
			};

			if (int.TryParse(UpdateFeedIntervalMS, out int UpdatedUpdateFeedIntervalMS) &&
				UpdatedUpdateFeedIntervalMS > 0)
			{
				UpdatedConfig.UpdateFeedIntervalMS = UpdatedUpdateFeedIntervalMS;
			}
		}
	}
}
