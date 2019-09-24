using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SpaceAnalyzer.ViewModels;

namespace SpaceAnalyzer.Views
{
    public partial class ContentScreen : Window
    {
        public ContentViewModel ContentViewModel { get; set; }
        public ContentScreen(string typeName, List<FileModel> fileModels)
        {
            InitializeComponent();
            ContentViewModel = new ContentViewModel(typeName, fileModels);
            this.DataContext = ContentViewModel;
        }
    }
}