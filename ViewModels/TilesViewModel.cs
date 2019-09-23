using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;
using SpaceAnalyzer.ViewModels;

public class TilesViewModel : INotifyPropertyChanged
{
    public delegate void listOfFileModels(string typeName, List<FileModel> fileModels);
    public event listOfFileModels fileModelEventHandler;
    public Dictionary<string, List<string>> ValueDict { get; set; }
    public string ImagePath { get { return @"D:\SpaceAnalyzer\assets\octopus.gif"; } }
    public Dictionary<string, float> FileTypePercentageValue { get; set; }
    public ICommand TileClickCommand { get; set; }
    public TilesViewModel(Dictionary<string, (string, decimal)> fileSizesData)
    {
        ConvertDictToList(fileSizesData);
        TileClickCommand = new Command(tileClickAction, tileClickCheck);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
    private bool tilesVisibility = false;
    public bool TilesVisibility
    {
        get
        {
            return tilesVisibility;
        }
        set
        {
            tilesVisibility = value;
            NotifyPropertyChanged("TilesVisibility");
        }
    }
    private bool gridVisibility = true;
    public bool GridVisibility
    {
        get
        {
            return gridVisibility;
        }
        set
        {
            gridVisibility = value;
            NotifyPropertyChanged("GridVisibility");
        }
    }
    private bool tileClickCheck(object arg)
    {
        if (arg == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private async void tileClickAction(object name)
    {
        TilesVisibility = true;
        GridVisibility = false;
        string typeName = (string)name;
        List<FileModel> fileModels = new List<FileModel>();
        await Task.Run(() =>
        {
            string extensions = string.Empty;
            foreach (var tup in ExtensionsSupported.extensions)
            {
                if (tup.Item1.Equals(name))
                {
                    extensions = tup.Item2;
                    break;
                }
            }
            
            GetFileModels(extensions,fileModels);
            
            //use the extensions to find the files
            fileModelEventHandler(typeName, fileModels);
        });
    }
    private IEnumerable<FileModel> GetFileModels(string extensions,List<FileModel> fileModelsList)
    {
        GetAllFiles(CurrentSelectedDrive.Path, extensions.Split(","), fileModelsList);
        return fileModelsList;
    }
    //remove this duplicate method
    public static void GetAllFiles(string basePath, string[] extensions, List<FileModel> fileModelsList)
    {
        try
        {
            Parallel.ForEach(extensions, (extension) =>
            {
                try
                {
                    foreach (string file in Directory.GetFiles(basePath, extension))
                    {
                        FileInfo f = new FileInfo(file);
                        FileModel model = new FileModel();
                        model.Name = f.Name;
                        model.Size = convertBytesToRelevantSize(f.Length);
                        model.ImagePath = file;
                        fileModelsList.Add(model);
                    }
                }
                catch (Exception)
                {

                }
            });
            foreach (string dir in Directory.GetDirectories(basePath))
            {
                GetAllFiles(dir, extensions, fileModelsList);
            }
        }
        catch (Exception)
        {
            // Console.WriteLine("some exception of type:" + e.Message);
        }
    }

    private Dictionary<string, List<string>> ConvertDictToList(Dictionary<string, (string, decimal)> fileSizesData)
    {
        ValueDict = new Dictionary<string, List<string>>();
        FileTypePercentageValue = new Dictionary<string, float>();
        decimal totalSize = fileSizesData[ExtensionsSupported.AllFiles].Item2;
        float usedSize = 0;
        foreach (KeyValuePair<string, (string, decimal)> pair in fileSizesData)
        {
            string size = convertBytesToRelevantSize(pair.Value.Item2);
            ValueDict.Add(pair.Key, new List<string> { pair.Key, pair.Value.Item1, size });
            if (pair.Key != ExtensionsSupported.AllFiles)
            {
                float sizeInPercent = (float)((pair.Value.Item2 / totalSize) * 100);
                FileTypePercentageValue.Add(pair.Key, sizeInPercent);
                usedSize += sizeInPercent;
            }
            else
            {
                FileTypePercentageValue.Add("Others", (100.0f - usedSize));
            }
        }
        return ValueDict;
    }

    private static string convertBytesToRelevantSize(decimal item2)
    {
        List<string> sizes = new List<string> { "bytes", "KB", "MB", "GB", "TB", "PB" };
        int index = 0;
        while (item2 / 1024 >= 1)
        {
            item2 /= 1024;
            index++;
        }
        return string.Format("{0:0.00} ", item2) + sizes[index];
    }
}