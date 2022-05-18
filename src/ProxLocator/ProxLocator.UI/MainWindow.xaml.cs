using System;
using System.Collections.Generic;
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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using ProxLocator.UI.Model;

namespace ProxLocator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ProxLocatorViewModel ViewModel { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new ProxLocatorViewModel();
            DataContext = this.ViewModel;
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.StartProcessing();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.StopProcessing();
        }
    }
}
