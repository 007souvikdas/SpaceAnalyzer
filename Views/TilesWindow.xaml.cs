using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace SpaceAnalyzer.Views
{
    public partial class TilesWindow : Window
    {
        public TilesWindow(Dictionary<string, (string, decimal)> fileSizeData)
        {
            InitializeComponent();
            TilesViewModel tilesViewModel = new TilesViewModel(fileSizeData);
            this.DataContext = tilesViewModel;
        }
    }
}
