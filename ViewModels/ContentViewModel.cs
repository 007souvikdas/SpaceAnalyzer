using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;
using System.Collections.ObjectModel;
using System.IO;

namespace SpaceAnalyzer.ViewModels
{
    public class ContentViewModel : INotifyPropertyChanged
    {
        public string Heading { get; set; }
        public ObservableCollection<FileModel> FileModelsList { get; set; }
        private ObservableCollection<FileModel> originalFileModelsList { get; set; }
        public ICommand SelectedItemCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand FolderNavigationCommand { get; set; }
        public bool LabelVisibility { get; set; } = false;
        public bool ListVisibility { get; set; } = true;
        public ObservableCollection<ContextMenuItem> ContextMenuItems { get; set; }
        string searchBox = "Search";
        public string SearchBox
        {
            get
            {
                return searchBox;
            }
            set
            {
                searchBox = value;
                //Todo: make the below call fast, it is already asynchronous, why then time?
                ModifyFileModels(searchBox);
                SetPropertyChanged("SearchBox");
            }
        }

        private async void ModifyFileModels(string searchBoxString)
        {
            await Task.Run(() =>
            {
                FileModelsList = new ObservableCollection<FileModel>();
                foreach (var model in originalFileModelsList)
                {
                    if (model.Name.Contains(searchBoxString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        FileModelsList.Add(model);
                    }
                }
                CallNotifyChanged();
            });
        }
        public void CallNotifyChanged()
        {
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
            FileModelsList = new ObservableCollection<FileModel>(fileModels);
            originalFileModelsList = new ObservableCollection<FileModel>(fileModels);
            if (fileModels.Count == 0)
            {
                LabelVisibility = true;
                ListVisibility = false;
            }
            SelectedItemCommand = new Command(selectedItemAction, canExecuteItem);
            DeleteCommand = new Command(RightClickAction, canShowRightClickView);
            FolderNavigationCommand = new Command(FolderNavigationAction, canShowRightClickView);
            ContextMenuItems = new ObservableCollection<ContextMenuItem>();
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Open", ContextCommand = SelectedItemCommand });
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Delete", ContextCommand = DeleteCommand });
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Go to File location", ContextCommand = FolderNavigationCommand });
        }

        private void FolderNavigationAction(object obj)
        {
            FileModel path = (FileModel)obj;
            if (!string.IsNullOrEmpty(path.ImagePath))
            {
                try
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(Path.GetDirectoryName(path.ImagePath))
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Some error occured while opening the file");
                }
            }
        }

        private void RightClickAction(object obj)
        {
            try
            {
                FileModel path = (FileModel)obj;
                if (!string.IsNullOrEmpty(path.ImagePath))
                {
                    File.Delete(path.ImagePath);
                    originalFileModelsList.Remove(path);
                    FileModelsList.Remove(path);
                    // originalFileModelsList.Remove(model => model.ImagePath.Equals(path));
                    // FileModelsList.RemoveAll(model => model.ImagePath.Equals(path));
                    CallNotifyChanged();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Some Error occured while deleting the file.");
            }
        }

        private bool canShowRightClickView(object arg)
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void selectedItemAction(object obj)
        {
            string filepath = (string)obj;
            if (!string.IsNullOrEmpty(filepath))
            {
                try
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(filepath)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Some error occured while opening the file");
                }
            }
        }
        private bool canExecuteItem(object arg)
        {
            return true;
        }
    }
}