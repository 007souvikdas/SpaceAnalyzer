using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SpaceAnalyzer.ViewModels;

namespace SpaceAnalyzer.Views
{
    public partial class ContentScreen : Window
    {
        public string Heading { get; set; }
        public List<FileModel> list { get; set; }
        public ContentScreen(string typeName, List<FileModel> fileModels)
        {
            InitializeComponent();
            Heading = "All the " + typeName + "files are as follows:";
            list = fileModels;
            // list = new List<FileModel>();
            // list.Add(new FileModel { Name = "one", Size = "20MB", ImagePath = @"C:\Users\souvikdas01\Downloads\batmanIcon.jpg" });
            // list.Add(new FileModel { Name = "Two", Size = "20MB", ImagePath = @"C:\Users\souvikdas01\Downloads\batmanIcon.jpg" });
            // list.Add(new FileModel { Name = "Three", Size = "20MB", ImagePath = @"C:\Users\souvikdas01\Downloads\batmanIcon.jpg" });
            // list.Add(new FileModel { Name = "Four", Size = "20MB", ImagePath = @"C:\Users\souvikdas01\Downloads\batmanIcon.jpg" });

            this.DataContext = this;

        }

    }
}