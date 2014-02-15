using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Turkey
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private NetManager m_netMananger;

		public MainWindow()
		{
			InitializeComponent();

			m_netMananger = new NetManager();
			m_netMananger.Connect("195.67.190.174", 10900);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			m_netMananger.SendInput(consoleInputBox.Text);
		}
	}
}
