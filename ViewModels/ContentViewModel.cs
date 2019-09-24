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
        public ICommand BackButtonCommand { get; set; }

        public ContentViewModel(string typeName, List<FileModel> fileModels)
        {
            Heading = "All the " + typeName + " files are as follows:";
            FileModelsList = fileModels;
            SelectedItemCommand = new Command(selectedItemAction, canExecuteItem);
            BackButtonCommand=new Command(backButtonAction,canExecuteItem);
        }

        private void backButtonAction(object obj)
        {
            MessageBox.Show("backed button pressed");
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