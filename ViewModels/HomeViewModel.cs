using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;
using System.Threading.Tasks;
using System.IO;
public class HomeViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    public const string AllFiles = "All";
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
            NotifyProertyChanged("CurrentDrive");
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
            NotifyProertyChanged("ImageVisibility");
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
            NotifyProertyChanged("WindowVisibility");
        }
    }
    public ICommand AnalyzeCommand { get; set; }
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
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyProertyChanged(string propName)
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

    public async void AnalyzeAction(object paramater)
    {
        WindowVisibility = false;
        ImageVisibility = true;
        //logic to find files in paramater object passed
        string basePath = @"D:\git";//CurrentDrive;

        (string, string)[] extensions = { ("Videos", "*.exe, *.mkv, *.flv"), ("Images", "*.jpg, *.png, *.gif"), ("Audios", "*.mp3"), ("Docs", "*.pdf, *.doc, *.docx"), (AllFiles, "") };
        await Task.Run(() =>
        {
            Dictionary<string, (string, decimal)> dict = GetFileSizesDictionary(extensions, basePath);
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
            DriveCollection.Add(drive.Replace("\\", ""));
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
                    foreach (string file in Directory.GetFiles(basePath, extension))
                    {
                        FileInfo f = new FileInfo(file);
                        intermediate += f.Length;
                    }
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