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
	/// Interaction logic for PromptWindow.xaml
	/// </summary>
	public partial class PromptWindow : Window
	{
		public string Label { get; set; }

		public string Result => result.Text;

		public PromptWindow(string title, string label)
		{
			InitializeComponent();

			Title = title;
			Label = label;

			DataContext = this;

			result.Focus();
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
