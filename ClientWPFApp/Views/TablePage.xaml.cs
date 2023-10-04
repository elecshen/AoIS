using System.Windows.Controls;

namespace ClientWPFApp.Views
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public DataGrid TableG { get; set; }
        public DataGrid TableE { get; set; }
        public TablePage()
        {
            InitializeComponent();
			TableG = TableGrid;
            TableE = EditGrid;
		}
		void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			e.Row.Header = (e.Row.GetIndex()+1).ToString();
		}
	}
}
