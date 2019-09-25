using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;
using System.Threading.Tasks;
using System.IO;
using SpaceAnalyzer.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;

public class HomeViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    public string currentDrive = string.Empty;
    public string CurrentDrive
    {
        get
        {
            return currentDrive;
        }
        set
        {
            currentDrive = value;
            NotifyPropertyChanged("CurrentDrive");
        }
    }
    public delegate void EventDelegate(Dictionary<string, (string, decimal)> dict);
    public event EventDelegate windowChange;
    private bool imageVisibility = false;
    public bool ImageVisibility
    {
        get
        {
            return imageVisibility;
        }
        set
        {
            imageVisibility = value;
            NotifyPropertyChanged("ImageVisibility");
        }
    }
    public string ImagePath { get; set; }
    private bool windowVisibility = true;
    public bool WindowVisibility
    {
        get
        {
            return windowVisibility;
        }
        set
        {
            windowVisibility = value;
            NotifyPropertyChanged("WindowVisibility");
        }
    }
    public ICommand AnalyzeCommand { get; set; }
    public ICommand FolderSelector { get; set; }
    public ObservableCollection<string> DriveCollection { get; set; }
    public string Error
    {
        get
        {
            return null;
        }
    }
    public string this[string columnName]
    {
        get
        {
            if (columnName == "CurrentDrive")
            {
                if (CurrentDrive == string.Empty)
                {
                    return "Drive Selection cannot be left blank";
                }
                return "";
            }
            else
            {
                return "";
            }

        }
    }

    public HomeViewModel()
    {
        DriveCollection = GetDrivesList();
        AnalyzeCommand = new Command(AnalyzeAction, AnalyzeButtonVisibility);
        ImagePath = @"D:\SpaceAnalyzer\assets\octopus.gif";
        FolderSelector = new Command(folderSelectorDialogAction, folderSelectorCheck);
    }

    private bool folderSelectorCheck(object arg)
    {
        return true;
    }

    private void folderSelectorDialogAction(object obj)
    {
        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        dialog.InitialDirectory = "C:\\";
        dialog.IsFolderPicker = true;
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            string basepath = dialog.FileName;
            CommonCallingAction(basepath);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
    public bool AnalyzeButtonVisibility(object paramater)
    {
        if (paramater == null || (string)paramater == string.Empty)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void AnalyzeAction(object paramater)
    {
        string basepath = (string)paramater;
        CommonCallingAction(basepath);
    }
    public async void CommonCallingAction(string basePath)
    {
        WindowVisibility = false;
        ImageVisibility = true;
        //logic to find files in paramater object passed        
        CurrentSelectedDrive.Path = basePath;
        await Task.Run(() =>
        {
            Dictionary<string, (string, decimal)> dict = GetFileSizesDictionary(ExtensionsSupported.extensions, basePath);
            if (windowChange != null)
            {
                windowChange.Invoke(dict);
            }
        });
    }
    public ObservableCollection<string> GetDrivesList()
    {
        //logic to return drives
        DriveCollection = new ObservableCollection<string>();
        foreach (string drive in Environment.GetLogicalDrives())
        {
            DriveCollection.Add(drive.Replace("\\", @"\"));
        }
        return DriveCollection;
    }
    private Dictionary<string, (string, decimal)> GetFileSizesDictionary((string, string)[] arrayOfExtensions, string basePath)
    {
        Dictionary<string, (string, decimal)> fileSizes = new Dictionary<string, (string, decimal)>();
        foreach ((string, string) extensions in arrayOfExtensions)
        {
            decimal filesSize = 0;
            GetAllFiles(basePath, extensions.Item2.Split(","), ref filesSize);
            fileSizes[extensions.Item1] = (extensions.Item2, filesSize);
        }
        return fileSizes;
    }
    public static void GetAllFiles(string basePath, string[] extensions, ref decimal sizeVideos)
    {
        try
        {
            decimal intermediate = 0;
            Parallel.ForEach(extensions, (extension) =>
            {
                try
                {
                    extension = extension.Trim();
                    Parallel.ForEach(Directory.GetFiles(basePath, extension), (file) =>
                    {
                        {
                            FileInfo f = new FileInfo(file);
                            intermediate += f.Length;
                        }
                    });
                }
                catch (Exception)
                {

                }
            });
            sizeVideos += intermediate;
            foreach (string dir in Directory.GetDirectories(basePath))
            {
                GetAllFiles(dir, extensions, ref sizeVideos);
            }
        }
        catch (Exception)
        {
            // Console.WriteLine("some exception of type:" + e.Message);
        }
    }
}