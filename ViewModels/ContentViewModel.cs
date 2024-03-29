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
using System.Threading;
using System.Windows.Threading;

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
        public ICommand TextBoxChangeCommand { get; set; }
        public bool LabelVisibility { get; set; } = false;
        public bool ListVisibility { get; set; } = true;
        public ObservableCollection<ContextMenuItem> ContextMenuItems { get; set; }
        string searchBox = "Search";
        string folderSelected;
        public string FolderSelected
        {
            get
            {
                return folderSelected;
            }
            set
            {
                folderSelected = value;
                SetPropertyChanged("FolderSelected");
            }
        }

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

        // private async void ModifyFileModels(string searchBoxString)
        // {
        //     await Task.Run(() =>
        //     {
        //         FileModelsList = new ObservableCollection<FileModel>();
        //         foreach (var model in originalFileModelsList)
        //         {
        //             if (model.Name.Contains(searchBoxString, StringComparison.InvariantCultureIgnoreCase))
        //             {
        //                 FileModelsList.Add(model);
        //             }
        //         }
        //         CallNotifyChanged();
        //     });
        // }
        private void ModifyFileModels(string searchBoxString)
        {
            Thread thread = new Thread(() =>
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
            thread.Start();
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
            TextBoxChangeCommand = new Command(ChangeTextBoxAction, canShowRightClickView);
            ContextMenuItems = new ObservableCollection<ContextMenuItem>();
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Open", ContextCommand = SelectedItemCommand });
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Delete", ContextCommand = DeleteCommand });
            ContextMenuItems.Add(new ContextMenuItem() { Name = "Go to File location", ContextCommand = FolderNavigationCommand });
            FolderSelected = "Intial selected folder is: " + CurrentSelectedDrive.Path;
        }

        private void ChangeTextBoxAction(object obj)
        {
            try
            {
                FileModel path = (FileModel)obj;
                if (!string.IsNullOrEmpty(path.ImagePath))
                {
                    FolderSelected = "Selected file's folder is: " + Path.GetDirectoryName(path.ImagePath);
                }
            }
            catch (Exception)
            {
                //log the exception if needed
            }
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
                catch (Exception)
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
            catch (Exception)
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
            try
            {
                FileModel fileModel = (FileModel)obj;
                if (!string.IsNullOrEmpty(fileModel.ImagePath))
                {

                    new Process
                    {
                        StartInfo = new ProcessStartInfo(fileModel.ImagePath)
                        {
                            UseShellExecute = true
                        }
                    }.Start();

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Some error occured while opening the file");
            }
        }
        private bool canExecuteItem(object arg)
        {
            return true;
        }
    }
}