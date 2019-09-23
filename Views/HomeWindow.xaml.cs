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

namespace SpaceAnalyzer.Views
{
    public partial class HomeWindow : Window
    {
        HomeViewModel homeViewModel;
        public HomeWindow()
        {
            InitializeComponent();
            homeViewModel = new HomeViewModel();
            this.DataContext = homeViewModel;
            homeViewModel.windowChange += ListWindowEvent;
        }
        public void ListWindowEvent(Dictionary<string, (string, decimal)> dict)
        {
            this.Dispatcher.Invoke(() =>
            {
                TilesWindow tilesWindow = new TilesWindow(dict);
                tilesWindow.Show();
                this.Close();
            });
        }
    }
}