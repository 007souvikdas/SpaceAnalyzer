using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SpaceAnalyzer.Commands;

namespace SpaceAnalyzer.ViewModels
{
    public class ContentViewModel
    {
        public string Heading { get; set; }
        public List<FileModel> FileModelsList { get; set; }
        public ICommand SelectedItemCommand { get; set; }
        public bool LabelVisibility { get; set; } = false;
        public bool ListVisibility { get; set; } = true;
        public ContentViewModel(string typeName, List<FileModel> fileModels)
        {
            Heading = "All the " + typeName + " files are as follows:";
            FileModelsList = fileModels;
            if (fileModels.Count == 0)
            {
                LabelVisibility = true;
                ListVisibility = false;
            }
            SelectedItemCommand = new Command(selectedItemAction, canExecuteItem);
        }

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