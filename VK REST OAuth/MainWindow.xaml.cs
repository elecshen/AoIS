using System.Windows;

namespace VK_REST_OAuth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((VM)DataContext).WebBrowser = wb;
        }
    }
}
