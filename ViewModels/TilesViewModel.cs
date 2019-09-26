using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpaceAnalyzer.Commands;
using SpaceAnalyzer.ViewModels;

public class TilesViewModel : INotifyPropertyChanged
{
    public delegate void listOfFileModels(string typeName, List<FileModel> fileModels);
    public event listOfFileModels fileModelEventHandler;
    public delegate void backButtonDelegate();
    public event backButtonDelegate BackButtonEvent;
    public Dictionary<string, List<string>> ValueDict { get; set; }
    public string ImagePath { get { return @"D:\SpaceAnalyzer\assets\octopus.gif"; } }
    public Dictionary<string, float> FileTypePercentageValue { get; set; }
    public ICommand TileClickCommand { get; set; }
    public string BackButtonLocation { get; set; }
    public ICommand BackButtonCommand { get; set; }
    public string FolderSelected { get; set; }

    public TilesViewModel(Dictionary<string, (string, decimal)> fileSizesData)
    {
        ConvertDictToList(fileSizesData);
        TileClickCommand = new Command(tileClickAction, tileClickCheck);
        BackButtonLocation = @"D:\SpaceAnalyzer\assets\back.png";
        BackButtonCommand = new Command(backButtonAction, backButtonClickCheck);
        FolderSelected = "Selected Folder is: "+ CurrentSelectedDrive.Path;
    }
    private bool backButtonClickCheck(object arg)
    {
        return true;
    }

    private void backButtonAction(object obj)
    {
        if (BackButtonEvent != null)
        {
            BackButtonEvent.Invoke();
        }
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
        bool isImageOrVideos = false;
        await Task.Run(() =>
        {
            string extensions = string.Empty;
            if (name.Equals(ExtensionsSupported.Images) || name.Equals(ExtensionsSupported.Videos))
            {
                isImageOrVideos = true;
            }
            foreach (var tup in ExtensionsSupported.extensions)
            {
                if (tup.Item1.Equals(name))
                {
                    extensions = tup.Item2;
                    break;
                }
            }

            GetAllFiles(CurrentSelectedDrive.Path, extensions.Split(","), fileModels, isImageOrVideos);
            //use the extensions to find the files
            fileModelEventHandler(typeName, fileModels);
            TilesVisibility = false;
            GridVisibility = true;
        });
    }

    //remove this duplicate method
    public static void GetAllFiles(string basePath, string[] extensions, List<FileModel> fileModelsList, bool isImageOrVideos)
    {
        try
        {
            Parallel.ForEach(extensions, (extension) =>
            {
                try
                {
                    extension = extension.Trim();
                    Parallel.ForEach(Directory.GetFiles(basePath, extension), (filePath) =>
                   {
                       {
                           FileInfo f = new FileInfo(filePath);
                           FileModel model = new FileModel();
                           model.Name = f.Name;
                           model.Size = convertBytesToRelevantSize(f.Length);
                           model.ImagePath = filePath;
                           //convert the image to bitmap image
                           model.ImageBitmap = setImageBitmap(model.ImageBitmap, isImageOrVideos, filePath);
                           fileModelsList.Add(model);
                       }
                   });
                }
                catch (Exception)
                {

                }
            });
            Parallel.ForEach(Directory.GetDirectories(basePath), (dir) =>
            {
                GetAllFiles(dir, extensions, fileModelsList, isImageOrVideos);
            });
        }
        catch (Exception)
        {
            // Console.WriteLine("some exception of type:" + e.Message);
        }
    }

    private static BitmapImage setImageBitmap(BitmapImage imageBitmap, bool isImageOrVideos, string filePath)
    {
        if (isImageOrVideos)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(filePath);
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            myBitmapImage.Freeze();
            return myBitmapImage;
        }
        else
        {
            Icon icon = SystemIcons.WinLogo;
            icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            return ToBitmapImage(icon.ToBitmap());
        }
    }
    public static BitmapImage ToBitmapImage(Bitmap bitmap)
    {
        using (var memory = new MemoryStream())
        {
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
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
                float sizeInPercent = (float)totalSize != 0.0f ? (float)((pair.Value.Item2 / totalSize) * 100) : 0;
                FileTypePercentageValue.Add(pair.Key, sizeInPercent);
                usedSize += sizeInPercent;
            }
        }
        if (usedSize != 0.0f)
        {
            FileTypePercentageValue.Add("Others", (100.0f - usedSize));
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