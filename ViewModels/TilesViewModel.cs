using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;

public class TilesViewModel
{
    public Dictionary<string, List<string>> ValueDict { get; set; }
    public Dictionary<string, float> FileTypePercentageValue { get; set; }
    public ICommand TileClickCommand;

    public TilesViewModel(Dictionary<string, (string, decimal)> fileSizesData)
    {
        ConvertDictToList(fileSizesData);
        TileClickCommand = new Command(tileClickAction, tileClickCheck);
    }

    private bool tileClickCheck(object arg)
    {
        if (arg == null)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    private void tileClickAction(object obj)
    {
        MessageBox.Show("Clicked");
    }

    public Dictionary<string, List<string>> ConvertDictToList(Dictionary<string, (string, decimal)> fileSizesData)
    {
        ValueDict = new Dictionary<string, List<string>>();
        FileTypePercentageValue = new Dictionary<string, float>();
        float totalSize = (float)(fileSizesData[HomeViewModel.AllFiles].Item2 / (1024 * 1024));
        float usedSize = 0;
        foreach (KeyValuePair<string, (string, decimal)> pair in fileSizesData)
        {
            float sizeMB = (float)(pair.Value.Item2 / (1024 * 1024));
            ValueDict.Add(pair.Key, new List<string> { pair.Key, pair.Value.Item1, string.Format("{0:0.00 MB}", sizeMB) });
            if (pair.Key != HomeViewModel.AllFiles)
            {
                float sizeInPercent = (sizeMB / totalSize) * 100;
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
}