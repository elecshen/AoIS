using System.Windows;

namespace ClientWPFApp.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			VM.InstanceVM.frame = frame;
		}
	}
}
