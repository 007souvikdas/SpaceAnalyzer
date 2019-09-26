using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;

namespace SpaceAnalyzer.ViewModels
{
    public class ContentViewModel : INotifyPropertyChanged
    {
        public string Heading { get; set; }
        public List<FileModel> FileModelsList { get; set; }
        private List<FileModel> originalFileModelsList { get; set; }
        private List<FileModel> searchFileModelsList { get; set; }
        public ICommand SelectedItemCommand { get; set; }
        public bool LabelVisibility { get; set; } = false;
        public bool ListVisibility { get; set; } = true;
        string searchBox="Search";
        public string SearchBox
        {
            get
            {
                return searchBox;
            }
            set
            {
                searchBox = value;
                //modify the file models
                ModifyFileModels(searchBox);
                SetPropertyChanged("SearchBox");
            }
        }

        private async void ModifyFileModels(string searchBoxString)
        {
            await Task.Run(() =>
            {
                searchFileModelsList = new List<FileModel>();
                foreach (var model in originalFileModelsList)
                {
                    if (model.Name.Contains(searchBoxString,StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchFileModelsList.Add(model);
                    }
                }
                FileModelsList = searchFileModelsList;
                if (FileModelsList.Count == 0)
                {
                    LabelVisibility = true;
                    ListVisibility = false;
                    SetPropertyChanged("LabelVisibility");
                    SetPropertyChanged("ListVisibility");
                }
                else
                {
                    LabelVisibility = false;
                    ListVisibility = true;
                    SetPropertyChanged("LabelVisibility");
                    SetPropertyChanged("ListVisibility");
                }
                SetPropertyChanged("FileModelsList");

            });
        }

        public void SetPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ContentViewModel(string typeName, List<FileModel> fileModels)
        {
            Heading = "All the " + typeName + " files are as follows:";
            FileModelsList = fileModels;
            originalFileModelsList = fileModels;
            if (fileModels.Count == 0)
            {
                LabelVisibility = true;
                ListVisibility = false;
            }
            SelectedItemCommand = new Command(selectedItemAction, canExecuteItem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void selectedItemAction(object obj)
        {
            string filepath = (string)obj;
            if (!string.IsNullOrEmpty(filepath))
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(filepath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }
        private bool canExecuteItem(object arg)
        {
            return true;
        }
    }
}