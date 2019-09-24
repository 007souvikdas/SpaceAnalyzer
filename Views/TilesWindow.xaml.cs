using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using SpaceAnalyzer.ViewModels;

namespace SpaceAnalyzer.Views
{
    public partial class TilesWindow : Window
    {
        public TilesWindow(Dictionary<string, (string, decimal)> fileSizeData)
        {
            InitializeComponent();
            TilesViewModel tilesViewModel = new TilesViewModel(fileSizeData);
            this.DataContext = tilesViewModel;
            tilesViewModel.fileModelEventHandler += filesListEventhandler;
            tilesViewModel.BackButtonEvent += backButtonHandler;
        }

        private void backButtonHandler()
        {
            this.Dispatcher.Invoke(() =>
            {
                HomeWindow homeWindow = new HomeWindow();
                homeWindow.Show();
                this.Close();
            });
        }

        private void filesListEventhandler(string typeName, List<FileModel> fileModels)
        {
            this.Dispatcher.Invoke(() =>
            {
                ContentScreen contentScreen = new ContentScreen(typeName, fileModels);
                contentScreen.Show();
                //not closing the current window so that we can use it
            });
        }
    }
}
