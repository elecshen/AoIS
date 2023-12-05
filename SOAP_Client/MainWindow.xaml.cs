using CalcSrv;
using MySoapCalcService;
using System;
using System.Windows;

namespace SOAP_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SoapServiceClient calculator;

        public MainWindow()
        {
            InitializeComponent();
            calculator = new(SoapServiceClient.EndpointConfiguration.BasicHttpBinding_ISoapService_soap);
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получим результат метода
                int res = await calculator.AddAsync(int.Parse(AddA.Text), int.Parse(AddB.Text));
                // Выведем результат с помощью MessageBox
                MessageBox.Show($"Add result is {res}", "Calculation result", MessageBoxButton.OK);
            } catch (Exception ex)
            {
                MessageBox.Show($"Error in AddClick: {ex.Message}", "Calculation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ButtonSubtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int res = await calculator.SubtractAsync(int.Parse(SubtractA.Text), int.Parse(SubtractB.Text));
                MessageBox.Show($"Subtract result is {res}", "Calculation result", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in SubtractClick: {ex.Message}", "Calculation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int res = await calculator.MultiplyAsync(int.Parse(MultiplyA.Text), int.Parse(MultiplyB.Text));
                MessageBox.Show($"Multiply result is {res}", "Calculation result", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MultiplyClick: {ex.Message}", "Calculation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int res = await calculator.DivideAsync(int.Parse(DivideA.Text), int.Parse(DivideB.Text));
                MessageBox.Show($"Divide result is {res}", "Calculation result", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DivideClick: {ex.Message}", "Calculation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
